using System.Threading.Tasks;
using Quartz;

namespace CoreFra.Agent
{
    public abstract class CustomJob : QuartzJobAgent
    {
        public override Task Execute(IJobExecutionContext context)
        {
            return ExecuteAgent();
        }
    }
}