using System;

namespace CoreFra.Domain
{
    public class Audit
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        public string Input { get; set; }
        public string OutPut { get; set; }
        public string TargetTypeFullName { get; set; }
        public int ExecutionTime { get; set; }
    }
}