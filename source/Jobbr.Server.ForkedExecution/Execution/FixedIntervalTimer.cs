﻿using System;
using System.Threading;

namespace Jobbr.Server.ForkedExecution.Execution
{
    internal class FixedIntervalTimer : IPeriodicTimer, IDisposable
    {
        private Action callback;

        private Timer timer;
        private TimeSpan interval;

        public FixedIntervalTimer()
        {
            this.timer = new Timer(state => this.callback());
        }

        ~FixedIntervalTimer()
        {
            this.Dispose(false);
        }

        public void Setup(Action value, long intervalInSeconds)
        {
            this.callback = value;
            this.interval = TimeSpan.FromSeconds(intervalInSeconds);
        }

        public void Start()
        {
            this.timer.Change(this.interval, this.interval);
        }

        public void Stop()
        {
            this.timer.Change(int.MaxValue, int.MaxValue);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = null;
                }
            }
        }
    }
}