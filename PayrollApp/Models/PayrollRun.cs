namespace PayrollApp.Models
{
    public class PayrollRun
    {
        public int RunId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime RunDate { get; set; }
        public bool IsFinalized { get; set; }
    }
}