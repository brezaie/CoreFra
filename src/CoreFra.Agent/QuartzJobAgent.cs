using System;
using System.Threading.Tasks;
using Quartz;

namespace CoreFra.Agent
{
    public abstract class QuartzJobAgent : IJob
    {
        public abstract Task Execute(IJobExecutionContext context);
        public abstract Task ExecuteAgent();
    }
}
