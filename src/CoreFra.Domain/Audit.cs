using System;

namespace CoreFra.Domain
{
    public class Audit
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public int ExecutionTime { get; set; }
    }
}