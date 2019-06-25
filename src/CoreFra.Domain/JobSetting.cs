namespace CoreFra.Domain
{
    public class JobSetting
    {
        public string JobName { get; set; }
        public string TriggerName { get; set; }
        public int IntervalInMinutes { get; set; }
    }
}