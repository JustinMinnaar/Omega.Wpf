using Omega.WpfModels1.Profiling;
using static Omega.WpfControllers1.ProfilingController;
using System;
using System.Threading;

namespace Omega.WpfControllers1
{
    public class ProfilingJobController : AbstractBackgroundJobController
    {
        private readonly MainController main;

        public ProfilingJobController(MainController main)
        {
            this.main = main;
        }

        protected override void DoWork()
        {
            for (int i = 0; i < 1000; i++)
            {
                if (IsCancellationPending) break;

                ReportProgress(i / 10, "Step " + i);
                Thread.Sleep(10);
            }
        }

        protected override void DoCompleted()
        {
            if (!JobCancelled)
             JobProgress = 100;

            JobStatus = null;
        }
    }
}