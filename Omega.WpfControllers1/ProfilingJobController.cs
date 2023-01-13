using Omega.WpfModels1.Profiling;
using static Omega.WpfControllers1.ProfilingController;
using System;

namespace Omega.WpfControllers1
{
    public class ProfilingJobController : AbstractJobController
    {
        public override void JobBegin()
        {
            base.JobBegin();
        }

        public override void JobEnd()
        {
            base.JobEnd();
        }
    }
}