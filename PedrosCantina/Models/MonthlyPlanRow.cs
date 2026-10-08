namespace PedrosCantina.Models
{
    public class MonthlyPlanRow
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string EmployeeName { get; set; } = "";
        public bool IsManager { get; set; }
    }
}
