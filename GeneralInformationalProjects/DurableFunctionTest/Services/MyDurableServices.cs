using DurableFunctionTest.Interfaces;
using DurableFunctionTest.Models;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionTest.Services
{
    public class MyDurableServices : IMyDurableServices
    {
        public MyDurableServices()
        {

        }
        public async Task<List<ExecutionSequence>> GetExecutionSequenceAsync()
        {
            return new List<ExecutionSequence>
                {
                    new ExecutionSequence { sequence = 1, service = "Activity1" },
                    new ExecutionSequence { sequence = 2, service = "Activity2" },
                    new ExecutionSequence { sequence = 2, service = "Activity3" },
                    new ExecutionSequence { sequence = 3, service = "Activity4", disabled = 1 },
                    new ExecutionSequence { sequence = 4, service = "Activity1" },
                    new ExecutionSequence { sequence = 5, service = "Activity5" }
                };
        }

        public async Task<RetryOptions> GetRetryOptionsAsync()
        {
            return new RetryOptions(
                firstRetryInterval: TimeSpan.FromSeconds(5),
                maxNumberOfAttempts: 3
            )
            {
                Handle = (exception) => exception is HttpRequestException || exception is SocketException,
                MaxRetryInterval = TimeSpan.FromSeconds(10),
                BackoffCoefficient = 2.0 // Adjust this value to control the backoff behavior
            };
        }



    }
}
