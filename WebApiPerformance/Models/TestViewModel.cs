using System.Linq;
using WebApiPerformance.Helpers;

namespace WebApiPerformance.Models
{
    public class TestViewModel : BaseViewModel
    {
        public TestViewModel() { }

        public TestViewModel(string value)
        {            
            Value1 = value;
            Value2 = value;
            Value3 = value;
            Value4 = value;
            Value5 = value;
            Value6 = value;
            Value7 = value;
            Value8 = value;
            Value9 = value;
            Value10 = value;
            Value11 = value;
            Value12 = value;
            Value13 = value;
            Value14 = value;
            Value15 = value;
            Value16 = value;
            Value17 = value;
            Value18 = value;
            Value19 = value;
            Value20 = value;
            Value21 = value;
            Value22 = value;
            Value23 = value;
            Value24 = value;
            Value25 = value;
            Value26 = value;
            Value27 = value;
            Value28 = value;
            Value29 = value;
            Value30 = value;
            Value31 = value;
            Value32 = value;
            Value33 = value;
            Value34 = value;
            Value35 = value;
            Value36 = value;
            Value37 = value;
            Value38 = value;
            Value39 = value;
            Value40 = value;
            Value41 = value;
            Value42 = value;
            Value43 = value;
            Value44 = value;
            Value45 = value;
            Value46 = value;
            Value47 = value;
            Value48 = value;
            Value49 = value;
            Value50 = value;
        }

        public int Id { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
        public string Value5 { get; set; }
        public string Value6 { get; set; }
        public string Value7 { get; set; }
        public string Value8 { get; set; }
        public string Value9 { get; set; }
        public string Value10 { get; set; }
        public string Value11 { get; set; }
        public string Value12 { get; set; }
        public string Value13 { get; set; }
        public string Value14 { get; set; }
        public string Value15 { get; set; }
        public string Value16 { get; set; }
        public string Value17 { get; set; }
        public string Value18 { get; set; }
        public string Value19 { get; set; }
        public string Value20 { get; set; }
        public string Value21 { get; set; }
        public string Value22 { get; set; }
        public string Value23 { get; set; }
        public string Value24 { get; set; }
        public string Value25 { get; set; }
        public string Value26 { get; set; }
        public string Value27 { get; set; }
        public string Value28 { get; set; }
        public string Value29 { get; set; }
        public string Value30 { get; set; }
        public string Value31 { get; set; }
        public string Value32 { get; set; }
        public string Value33 { get; set; }
        public string Value34 { get; set; }
        public string Value35 { get; set; }
        public string Value36 { get; set; }
        public string Value37 { get; set; }
        public string Value38 { get; set; }
        public string Value39 { get; set; }
        public string Value40 { get; set; }
        public string Value41 { get; set; }
        public string Value42 { get; set; }
        public string Value43 { get; set; }
        public string Value44 { get; set; }
        public string Value45 { get; set; }
        public string Value46 { get; set; }
        public string Value47 { get; set; }
        public string Value48 { get; set; }
        public string Value49 { get; set; }
        public string Value50 { get; set; }

        public override IQueryable<TestViewModel> ApplyFilterParams<TestViewModel>(IQueryable<TestViewModel> data, FilterParams filterParams)
        { 
            var predicate = PredicateBuilder.True<TestViewModel>();
            var ids = ParseIntList(filterParams.acfpIDs);
            predicate = this.IdPredicate(ids, predicate);
            return data.Where(predicate);
        }
    }
}