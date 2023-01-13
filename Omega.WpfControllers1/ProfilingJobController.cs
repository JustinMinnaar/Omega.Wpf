using Omega.WpfModels1.Profiling;
using static Omega.WpfControllers1.ProfilingController;
using System;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace Omega.WpfControllers1
{
    public class ProfilingJobController : AbstractBackgroundJobController
    {
        #region class

        [SetsRequiredMembers]
        public ProfilingJobController(MainController main)
        {
            this.Main = main;
        }

        public required MainController Main { get; init; }

        #endregion

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