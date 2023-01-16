using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Omega.WpfControllers1
{
    public class AbstractBackgroundJobController : AbstractJobController
    {
        public AbstractBackgroundJobController()
        {
            bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        private readonly BackgroundWorker bw;

        public bool IsCancellationPending { get; protected set; }
        public double JobProgress { get; set; }
        protected virtual void OnJobProgressChanged() { }

        public object? JobStatus { get; protected set; }
        public object? JobResult { get; protected set; }
        public Exception? JobError { get; protected set; }
        public bool JobCancelled { get; protected set; }

        public override void JobBegin()
        {
            IsJobRunning = true;
            IsCancellationPending = false;
            bw.RunWorkerAsync();
        }

        public override bool CanJobEnd()
        {
            return IsJobRunning && !IsCancellationPending;
        }

        public override void JobEnd()
        {
            IsJobRunning = false;
            IsCancellationPending = true;
            bw.CancelAsync();
        }

        private void Bw_DoWork(object? sender, DoWorkEventArgs e) { DoWork(); }

        private void Bw_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(() =>
            {
                JobProgress = e.ProgressPercentage;
                JobStatus = e.UserState;
            });
        }

        private void Bw_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            IsJobRunning = false;
            IsCancellationPending = false;

            JobResult = e.Result;
            JobError = e.Error;
            JobCancelled = e.Cancelled;
            DoCompleted();
        }

        protected void ReportProgress(int jobProgress, object? jobState)
        {
            bw.ReportProgress(jobProgress, jobState);
        }

        protected virtual void DoCompleted() { }

        protected virtual void DoWork() { }
    }
}