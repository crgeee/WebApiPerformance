namespace WebApiPerformance.Models
{
    public class DataFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Logic { get; set; }
    }
}