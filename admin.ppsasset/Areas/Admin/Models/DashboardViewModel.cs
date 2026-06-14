using System;

namespace PPSAssetAdmin.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int YtdLeads { get; set; }
        public int LeadsThisMonth { get; set; }
        public int LeadsThisWeek { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<ChartData> LeadsByWeek { get; set; } = new();
        public List<ChartData> LeadsByProject { get; set; } = new();
    }

    public class ChartData
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public DateTime? Timestamp { get; set; } // For sorting
    }
}
