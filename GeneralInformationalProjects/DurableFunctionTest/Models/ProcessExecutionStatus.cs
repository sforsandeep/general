using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DurableFunctionTest.Models
{
    public class ExecutionSequence
    {
        public int sequence { get; set; }
        public string service { get; set; }
        public int? disabled { get; set; }
    }
    public enum ExecutionStatus
    {
        NotStarted,
        //Completed,
        Failed,
        PartiallyCompleted,
        Success
    }

    public class ExecutionDetail
    {
        public int Attempt { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public TimeSpan Duration => EndsAt - StartsAt;
        public ExecutionStatus Status { get; set; }
    }

    public class ActivityExecution
    {
        public string Name { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public TimeSpan Duration => EndsAt - StartsAt;
        public ExecutionStatus Status { get; set; }
        public string Remarks { get; set; }
        public List<ExecutionDetail> ExecutionDetails { get; set; } = new List<ExecutionDetail>();
    }

    public class ProcessExecution
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public TimeSpan Duration => EndsAt - StartsAt;
        public ExecutionStatus Status { get; set; }
        public List<ActivityExecution> Activities { get; set; } = new List<ActivityExecution>();
    }


    public class ActivityRequest
    {
        public DateTime StartsAt { get; set; }
        public int Attempt { get; set; }
    }


    public class ActivityResponse
    {
        public DateTime StartsAt { get; set; }
        public DateTime EndAt { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public string ServiceName { get; set; }
    }
}
