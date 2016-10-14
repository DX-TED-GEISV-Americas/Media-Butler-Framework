using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.WorkflowStep
{
    /// <summary>
    /// Custom workflow step
    /// </summary>
    public interface ICustomStepExecution
    {
        /// <summary>
        /// Implement Custome logic Step
        /// </summary>
        /// <param name="request">Custom request</param>
        /// <returns>true</returns>
        bool execute(ICustomRequest request);
    }
}
