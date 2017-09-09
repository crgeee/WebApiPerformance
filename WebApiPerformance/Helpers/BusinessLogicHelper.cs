using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Antlr.Runtime;
using Jil;
using Newtonsoft.Json.Linq;
using WebApiPerformance.Models;

namespace WebApiPerformance.Helpers
{
    public static class BusinessLogicHelper
    {
        /// <summary>
        /// Gets TAKE parameter from a request payload
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int GetTake(Dictionary<string, object> request)
        {
            int take = 100;
            object param;
            if (request.TryGetValue("take", out param))
            {
                int parseResult = 0;
                if (Int32.TryParse(param.ToString(), out parseResult))
                {
                    take = parseResult;
                }
            }
            return take;
        }

        /// <summary>
        /// Gets SKIP parameter from a request payload
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int GetSkip(Dictionary<string, object> request)
        {
            int skip = 0;
            object param;
            if (request.TryGetValue("skip", out param))
            {
                int parseResult = skip;
                if (int.TryParse(param.ToString(), out parseResult))
                {
                    skip = parseResult;
                }
            }
            return skip;
        }

        /// <summary>
        /// Gets ORDERBY parameter from a request payload
        /// </summary>
        /// <param name="request"></param>
        /// <param name="defaultOrder"></param>
        /// <returns></returns>
        public static string GetOrderBy(Dictionary<string, object> request, string defaultOrder = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            object param;
            string retVal = "";

            request.TryGetValue("sort", out param);
            if (param != null)
            {
                var sortByToken = JToken.Parse(param.ToString());

                foreach (var token in sortByToken)
                {
                    stringBuilder.Append(string.Format("{0} {1}", token["field"].ToString(), token["dir"].ToString())).Append(',');
                }

                retVal = stringBuilder.ToString().TrimEnd((new char[] { ',' }));

            }
            if (retVal.Length == 0)
            {
                retVal = !string.IsNullOrWhiteSpace(defaultOrder) ? defaultOrder : "ACFP_ID asc";
            }
            return retVal;
        }

        /// <summary>
        /// Parse column filters
        /// </summary>
        /// <param name="columnFilters"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        private static string ParseFilter(JToken columnFilters, Type objType = null)
        {
            string sql = "";
            string logic = "";
            List<string> sqlList = new List<string>();
            string retVal = "";
            var filters = JObject.FromObject(columnFilters).ToObject<JToken>()["filters"];

            if (filters != null)
            {
                logic = JObject.FromObject(columnFilters).ToObject<JToken>()["logic"].ToString();

                //Add spaces on both ends
                logic = string.Format(" {0} ", logic);
                foreach (var f in filters)
                {
                    if (objType != null)
                    {
                        // Use the filter's field name to get the PropertyInfo object so we can get the correct propery type for this field.
                        string fieldName = JObject.FromObject(f).ToObject<JToken>()["field"].ToString();
                        PropertyInfo myPropInfo = objType.GetProperty(fieldName);
                        string typeName = myPropInfo.PropertyType.FullName;

                        var x = f.ToObject<DataFilter>();
                        sql = Parser.ParseParameter(x.Field, x.Operator, x.Value, typeName.Substring(typeName.IndexOf('.') + 1));
                        sqlList.Add(sql);
                    }
                    else
                    {
                        var x = f.ToObject<DataFilter>();
                        sql = Parser.ParseParameter(x.Field, x.Operator, x.Value);
                        sqlList.Add(sql);
                    }
                }
                retVal = string.Format("({0})", string.Join(logic, sqlList.ToArray()));
            }
            else
            {
                var x = columnFilters.ToObject<DataFilter>();

                if (objType != null)
                {
                    // Use the filter's field name to get the PropertyInfo object so we can get the correct propery type for this field.
                    PropertyInfo myPropInfo = objType.GetProperty(x.Field);
                    string typeName = myPropInfo.PropertyType.FullName;

                    sql = Parser.ParseParameter(x.Field, x.Operator, x.Value, typeName.Substring(typeName.IndexOf('.') + 1));
                    sqlList.Add(sql);
                }
                else
                {
                    sql = Parser.ParseParameter(x.Field, x.Operator, x.Value);
                    sqlList.Add(sql);
                }
                retVal = string.Format("{0}", string.Join(logic, sqlList.ToArray()));
            }

            return retVal;
        }

        /// <summary>
        /// Gets WHERE condition from request payload
        /// </summary>
        /// <param name="request"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static string GetWhereCondition(Dictionary<string, object> request, Type objType = null)
        {
            object param = null;
            string sql = "";

            //Default value of logic
            string logic = "";
            List<string> sqlList = new List<string>();
            request.TryGetValue("filter", out param);
            if (param != null)
            {
                var filters = JObject.FromObject(param).ToObject<JToken>()["filters"];
                logic = JObject.FromObject(param).ToObject<JToken>()["logic"].ToString();

                //Add spaces on both ends
                logic = string.Format(" {0} ", logic);
                foreach (var f in filters)
                {
                    if (objType != null)
                        sql = ParseFilter(f, objType);
                    else
                        sql = ParseFilter(f);

                    sqlList.Add(sql);
                }
            }
            if (!sqlList.Any())
            {
                sqlList.Add("1=1");
            }
            return string.Join(logic, sqlList.ToArray());
        }

        #region Parser
        /// <summary>
        /// Parser class fro creating where clauses
        /// </summary>
        public static class Parser
        {
            private static string _value;
            private static string _field;
            private static string _operator;
            private static string _fieldType;
            private static string _where;
            private static string _formattedValue;

            /// <summary>
            /// Parse parameters to form a where clause
            /// </summary>
            /// <param name="field"></param>
            /// <param name="op"></param>
            /// <param name="value"></param>
            /// <param name="fieldType"></param>
            /// <returns></returns>
            public static string ParseParameter(string field, string op, string value, string fieldType = null)
            {
                _value = value ?? "";
                _field = field;
                _operator = op;
                _fieldType = fieldType;
                FormatValue();
                CreateWhereClause();

                return _where;
            }

            /// <summary>
            /// Build where clauses
            /// </summary>
            private static void CreateWhereClause()
            {
                string standardStringFormat = "{0} {1} {2}";

                switch (_operator)
                {
                    case "contains":
                        if (IsNumeric)
                        {
                            _where = string.Format("{0}.contains(\"{1}\")", _field, _formattedValue);
                        }
                        else
                        {
                            _where = string.Format("{0}.tolower().contains({1}.tolower())", _field, _formattedValue);
                        }
                        break;

                    case "doesnotcontain":
                        if (IsNumeric)
                        {
                            _where = string.Format("!{0}.contains(\"{1}\")", _field, _formattedValue);
                        }
                        else
                        {
                            _where = string.Format("!{0}.tolower().contains({1}.tolower())", _field, _formattedValue);
                        }
                        break;

                    case "endswith":
                        if (IsNumeric)
                        {
                            _where = string.Format("{0}.endswith(\"{1}\")", _field, _formattedValue);
                        }
                        else
                        {
                            _where = string.Format("{0}.tolower().endswith({1}.tolower())", _field, _formattedValue);
                        }
                        break;

                    case "eq":
                        _where = FormatWhereClause();
                        break;

                    case "isempty":
                        _where = string.Format("{0}.length==0", _field);
                        break;

                    case "isnotempty":
                        _where = string.Format("{0}.length>0", _field);
                        break;

                    case "isnotnull":
                        _where = string.Format(standardStringFormat, _field, "!=", "NULL");
                        break;

                    case "isnull":
                        _where = string.Format(standardStringFormat, _field, "=", "NULL");
                        break;

                    case "neq":
                        _where = FormatWhereClause();
                        break;

                    case "startswith":
                        if (IsNumeric)
                        {
                            _where = string.Format("{0}.startswith(\"{1}\")", _field, _formattedValue);
                        }
                        else
                        {
                            _where = string.Format("{0}.tolower().startswith({1}.tolower())", _field, _formattedValue);
                        }
                        break;

                    case "gt":
                        _where = FormatWhereClause();
                        break;

                    case "ls":
                    case "lt":
                        _where = string.Format(standardStringFormat, _field, "<", _formattedValue);
                        break;

                    case "gte":
                        _where = string.Format(standardStringFormat, _field, ">=", _formattedValue);
                        break;

                    case "lte":
                        if (IsNumeric)
                        {
                            _where = string.Format(standardStringFormat, _field, "<=", _formattedValue);
                        }
                        else
                        {
                            _where = FormatWhereClause();
                        }
                        break;

                    default:
                        _where = "1=1";
                        break;
                }
            }

            /// <summary>
            /// format where clause based on type
            /// </summary>
            /// <returns></returns>
            private static string FormatWhereClause()
            {
                string retVal = null;
                string standardStringFormat = "{0} {1} {2}";

                if (IsDate)
                {
                    DateTime filterValue = DateTime.Parse(_value);
                    DateTime nextDayValue = filterValue.AddDays(1);
                    string formattedNextDay = String.Format("DateTime({0},{1},{2},00,00,1)", nextDayValue.Year, nextDayValue.Month, nextDayValue.Day);
                    string formattedFilter = String.Format("DateTime({0},{1},{2},00,00,1)", filterValue.Year, filterValue.Month, filterValue.Day);
                    if (_operator == "eq")
                    {
                        retVal = string.Format("{0}<{1} AND {2}>{3}", _field, formattedNextDay, _field, formattedFilter);
                    }
                    if (_operator == "lte")
                    {
                        retVal = string.Format("{0}<={1}", _field, formattedNextDay);
                    }
                    if (_operator == "neq")
                    {
                        retVal = string.Format("{0}<{1} OR {2}>{3}", _field, formattedFilter, _field, formattedNextDay);
                    }
                    if (_operator == "gt")
                    {
                        retVal = string.Format("{0}>{1}", _field, formattedNextDay);
                    }
                }
                else
                {
                    retVal = string.Format(standardStringFormat, _field, "=", _formattedValue);
                }
                return retVal;
            }

            /// <summary>
            /// format a value based on type
            /// </summary>
            private static void FormatValue()
            {
                if (IsNumeric)
                {
                    int retVal;
                    if (int.TryParse(_value, out retVal))
                    {
                        _formattedValue = ParseValue(retVal);
                        return;
                    }
                }

                if (IsDate)
                {
                    DateTime dtDate;
                    if (DateTime.TryParse(_value, out dtDate))
                    {
                        _formattedValue = ParseValue(dtDate);
                        return;
                    }
                }

                if (IsString)
                {
                    _formattedValue = ParseValue(_value);
                    return;
                }
            }

            /// <summary>
            /// determine if value is a number
            /// </summary>
            private static bool IsNumeric
            {
                get
                {
                    bool retVal = false;
                    if (_value != null)
                    {
                        if (_fieldType == null || (_fieldType != null && (_fieldType != "String" && _fieldType != "DateTime")))
                        {
                            retVal = _value.All(c => char.IsDigit(c) || c == '.');
                        }
                    }

                    return retVal;
                }
            }

            /// <summary>
            /// determine if value is a string
            /// </summary>
            private static bool IsString
            {
                get
                {
                    if (_fieldType != null && _fieldType == "String")
                    {
                        return true;
                    }
                    else
                    {
                        var RgxUrl = new Regex("[a-zA-Z]");
                        return RgxUrl.IsMatch(_value);
                    }
                }
            }

            /// <summary>
            /// determine if value is a date
            /// </summary>
            private static bool IsDate
            {
                get
                {
                    DateTime date;
                    return DateTime.TryParse(_value, out date);
                }
            }

            /// <summary>
            /// parse a string value
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private static string ParseValue(string value)
            {
                return String.Format("\"{0}\"", value);
            }

            /// <summary>
            /// parse a numeric value
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private static string ParseValue(int value)
            {
                return string.Format("{0}", value);
            }

            /// <summary>
            /// parse a date value
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private static string ParseValue(DateTime value)
            {
                Regex RgxUrl = new Regex("[^a-z0-9]");
                return string.Format("DateTime({0},{1},{2})", value.Year, value.Month, value.Day);
            }
        }
        #endregion Parser
    }
}