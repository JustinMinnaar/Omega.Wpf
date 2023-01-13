using Jem.CommonLibrary22;

using Omega.WpfCommon1;

using System.Windows.Input;

namespace Omega.WpfControllers1
{
    public abstract class AbstractJobController : CNotifyPropertyChanged
    {
        #region Commands

        RelayCommand? _JobBeginCommand, _JobEndCommand;

        public ICommand JobBeginCommand => _JobBeginCommand ??= new RelayCommand(JobBegin, CanJobBegin);
        public ICommand JobEndCommand => _JobEndCommand ??= new RelayCommand(JobEnd, CanJobEnd);

        public bool JobRunning { get; private set; }

        private bool CanJobBegin() { return !JobRunning; }
        public bool JobBeginEnabled => CanJobBegin();
        public virtual void JobBegin()
        {
            JobRunning = true;
        }

        public bool CanJobEnd() => JobRunning;
        public bool JobEndEnabled => CanJobEnd();
        public virtual void JobEnd()
        {
            JobRunning = false;
        }

        #endregion
    }
}