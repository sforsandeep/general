using DurableFunctionTest.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionTest.Interfaces
{
    public interface IMyDurableServices
    {
        Task<List<ExecutionSequence>> GetExecutionSequenceAsync();
        Task<RetryOptions> GetRetryOptionsAsync();
    }
}
