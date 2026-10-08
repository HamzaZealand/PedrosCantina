namespace PedrosCantina.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
