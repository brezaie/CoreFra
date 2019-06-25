using System;
using System.Threading.Tasks;
using CoreFra.Agent;

namespace CoreFra.Test.Common
{
    public class TestJob : CustomJob
    {
        public override Task ExecuteAgent()
        {
            System.IO.File.WriteAllText("hello.txt", "This is to test the agent");
            
            return Task.CompletedTask;       
        }
    }
}