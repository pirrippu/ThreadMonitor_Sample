using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionPool
{
    interface IExecutionPool
    {
        int StartStrategy(string strategyName, string strategyPath, int type);
        int StopStrategy(string strategyName);
        int StartStrategyGroup(string groupName);
        int StopStrategyGroup(string groupName);
    }
}
