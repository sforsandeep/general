using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DurableFunctionTest.Interfaces;
using DurableFunctionTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableFunctionTest
{
    public  class Orchestration
    {
        private readonly ILogger<Orchestration> _logger;
        private readonly IMyDurableServices _myDurableServices;
        public Orchestration(IMyDurableServices myDurableServices, ILogger<Orchestration> logger)
        {
            _myDurableServices = myDurableServices;
            _logger = logger;
        }


        [FunctionName("RunOrchestrator")]
        public async Task<ProcessExecution> RunOrchestrator(
      [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var processExecution = new ProcessExecution
            {
                StartsAt = context.CurrentUtcDateTime,
                Id=context.InstanceId
            };

            var retryOptions = new RetryOptions(firstRetryInterval: TimeSpan.FromSeconds(5), maxNumberOfAttempts: 3)
            {
                BackoffCoefficient = 2.0
            };
            var executionSequences = await _myDurableServices.GetExecutionSequenceAsync();
            var groupedSequences = executionSequences.GroupBy(seq => seq.sequence).OrderBy(grp => grp.Key);

            bool anyActivityFailed = false;

            foreach (var group in groupedSequences)
            {
                foreach (var seq in group)
                {
                    if (seq.disabled.HasValue && seq.disabled.Value == 1)
                    {
                        continue;
                    }

                    var activityExecution = new ActivityExecution { Name = seq.service, StartsAt = context.CurrentUtcDateTime };
                    processExecution.Activities.Add(activityExecution);

                    // Update the activity start time right before the outer loop
                    activityExecution.StartsAt = context.CurrentUtcDateTime;

                    ActivityResponse response = null;

                    for (int attempt = 1; attempt <= retryOptions.MaxNumberOfAttempts; attempt++)
                    {
                        Console.WriteLine($"Starting attempt {attempt}");
                        var executionDetail = new ExecutionDetail { Attempt = attempt };
                        activityExecution.ExecutionDetails.Add(executionDetail);
                        executionDetail.StartsAt = context.CurrentUtcDateTime;
                        try
                        {

                            // Create and start the task inside the retry loop
                            ActivityRequest ai = new ActivityRequest { StartsAt = context.CurrentUtcDateTime, Attempt = attempt };
                            
                            response = await context.CallActivityAsync<ActivityResponse>(seq.service, ai);
                            executionDetail.EndsAt = context.CurrentUtcDateTime;

                            if (response.HttpResponseMessage.IsSuccessStatusCode)
                            {
                                executionDetail.Status = ExecutionStatus.Success;
                                activityExecution.Status = ExecutionStatus.Success;
                                break;
                            }
                            else if (response.HttpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
                            {
                                executionDetail.Status = ExecutionStatus.Failed;
                                activityExecution.Status = ExecutionStatus.Failed;
                                activityExecution.Remarks = $"Bad Request (attempt {attempt})";
                                anyActivityFailed = true;
                            }
                            else
                            {
                                executionDetail.Status = ExecutionStatus.Failed;
                            }
                        }
                        catch (Exception ex)
                        {
                            executionDetail.Status = ExecutionStatus.Failed;
                            executionDetail.EndsAt = context.CurrentUtcDateTime;
                        }

                        if (attempt < retryOptions.MaxNumberOfAttempts )
                        {
                            double delay = retryOptions.FirstRetryInterval.TotalSeconds * Math.Pow(retryOptions.BackoffCoefficient, attempt);

                            await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(delay), CancellationToken.None);
                        }
                    }

                    if (response != null && !response.HttpResponseMessage.IsSuccessStatusCode)
                    {
                        activityExecution.Status = ExecutionStatus.Failed;
                        activityExecution.Remarks = $"Failed after {retryOptions.MaxNumberOfAttempts} attempts";
                        anyActivityFailed = true;
                    }

                    activityExecution.EndsAt = context.CurrentUtcDateTime;
                }
            }

            processExecution.EndsAt = context.CurrentUtcDateTime;
            processExecution.Status = anyActivityFailed ? ExecutionStatus.PartiallyCompleted : ExecutionStatus.Success;

            Console.WriteLine(JsonSerializer.Serialize<ProcessExecution>(processExecution));
            return processExecution;
        }


        private bool IsTransientError(Exception ex)
        {
            // You can extend this logic based on specific exceptions that you consider transient
            return ex is SocketException || ex is HttpRequestException;
        }

        [FunctionName("Activity1")]
        public async Task<ActivityResponse> Activity1([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var input = context.GetInput<ActivityRequest>();

            var response = new ActivityResponse { StartsAt = input.StartsAt, ServiceName = "Activity1" };
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Acitivity 1");
            Console.ForegroundColor = ConsoleColor.DarkGray;

            response.EndAt = DateTime.UtcNow;

                response.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);


            return response;
        }
        [FunctionName("Activity2")]
        public async Task<ActivityResponse> Activity2([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var input = context.GetInput<ActivityRequest>();
            

            var response = new ActivityResponse { StartsAt = input.StartsAt, ServiceName = "Activity2" };



            response.EndAt = DateTime.UtcNow;

            // A simple counter to simulate failure for the first two attempts
            if (input.Attempt <= 2)
            {
                response.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            else
            {
                response.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Acitivity 2 Starts at: {response.StartsAt} ends at {response.EndAt}");
            Console.WriteLine("--------------------------------");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            return response;
        }


        [FunctionName("Activity3")]
        public async Task<ActivityResponse> Activity3([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var input = context.GetInput<ActivityRequest>();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Acitivity 3");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            var response = new ActivityResponse { StartsAt = input.StartsAt, ServiceName = "Activity3" };

            response.EndAt = DateTime.UtcNow;

            response.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);


            return response;
        }

        [FunctionName("Activity4")]
        public async Task<ActivityResponse> Activity4([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var input = context.GetInput<ActivityRequest>();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Acitivity 4");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            var response = new ActivityResponse { StartsAt = input.StartsAt, ServiceName = "Activity4" };

            response.EndAt = DateTime.UtcNow;

            response.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);


            return response;
        }


        [FunctionName("Activity5")]
        public async Task<ActivityResponse> Activity5([ActivityTrigger] IDurableActivityContext context, ILogger log)
        {
            var input = context.GetInput<ActivityRequest>();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Acitivity 5");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            var response = new ActivityResponse { StartsAt = DateTime.UtcNow, ServiceName = "Activity5" };

            response.EndAt = DateTime.UtcNow;

            response.HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);


            return response;
        }

        [FunctionName("Orchestration_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("RunOrchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}