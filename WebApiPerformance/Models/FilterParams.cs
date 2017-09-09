using System;
using System.Collections.Generic;

namespace WebApiPerformance.Models
{
    public class FilterParams
    {
        public FilterParams()
        {
            acfpIDs = new List<string>();
            altTypes = new List<string>();
            availDeltas = new List<string>();
            availFYs = new List<string>();
            availTypes = new List<string>();
            baselineTypes = new List<string>();
            ctns = new List<string>();
            deliverableIDs = new List<string>();
            deliverables = new List<string>();
            excursionIDs = new List<string>();
            jobIDs = new List<string>();
            jobTrackingIDs = new List<string>();
            platformIDs = new List<string>();
            pmws = new List<string>();
            programIDs = new List<string>();
            scdIDs = new List<string>();
            sfpEventIDs = new List<string>();
            siteIDs = new List<string>();
            siteTypeIDs = new List<string>();
            strikeStatuses = new List<string>();
            systemIDs = new List<string>();
        }

        // bool
        public bool showNull { get; set; }

        // datetime
        public DateTime? availEndDt { get; set; }
        public DateTime? availStartDt { get; set; }

        // int
        public int? availEnd { get; set; }
        public int? availStart { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int? strikeCandidate { get; set; }

        // list
        public List<string> acfpIDs { get; set; }
        public List<string> altTypes { get; set; }
        public List<string> availDeltas { get; set; }
        public List<string> availFYs { get; set; }
        public List<string> availTypes { get; set; }
        public List<string> baselineTypes { get; set; }
        public List<string> ctns { get; set; }
        public List<string> deliverableIDs { get; set; }
        public List<string> deliverables { get; set; }
        public List<string> excursionIDs { get; set; }
        public List<string> jobIDs { get; set; }
        public List<string> jobTrackingIDs { get; set; }
        public List<string> platformIDs { get; set; }
        public List<string> pmws { get; set; }
        public List<string> programIDs { get; set; }
        public List<string> scdIDs { get; set; }
        public List<string> sfpEventIDs { get; set; }
        public List<string> siteIDs { get; set; }
        public List<string> siteTypeIDs { get; set; }
        public List<string> strikeStatuses { get; set; }
        public List<string> systemIDs { get; set; }

        // string
        public string monthYear { get; set; }
        public string pccbBinStage { get; set; }
        public string pccbBinStudy { get; set; }
        public string pccbBinType { get; set; }
        public string pmw { get; set; }
        public string projectedDate { get; set; }
        public string strikeStatus { get; set; }
    }

}