using System;
using Jobbr.ComponentModel.Execution.Model;

namespace Jobbr.Server.ForkedExecution.Tests.Infrastructure
{
    public class FakeJobRunStoreTuple
    {
        public Guid UniqueId { get; set; }

        public long Id { get; set; }

        public JobRunInfo JobRunInfo { get; set; }

        public PlannedJobRun PlannedJobRun { get; set; }
    }
}