using System;

namespace Omega.WpfModels1;

public class JobTotals
{
    private object padlock = new();

    private DateTime dateStarted = DateTime.Now;
    public TimeSpan Elapsed => DateTime.Now - dateStarted;

    private int countJobs;
    private int indexJobs;
    public int CountJobs { get => countJobs; init => countJobs = value; }
    public int IndexJobs { get => indexJobs; }
    public void IncrementIndexJobs(int value = 1) { lock (this.padlock) { indexJobs += value; } }

    public override string ToString()
    {
        var elapsedSeconds = Elapsed.TotalSeconds;
        return
            $"{indexJobs}/{countJobs} " +
            $"in {(int)(elapsedSeconds / 60)}:{(int)(elapsedSeconds % 60)} " +
            $"at {(double)indexJobs / elapsedSeconds:N1}/sec";
    }
}