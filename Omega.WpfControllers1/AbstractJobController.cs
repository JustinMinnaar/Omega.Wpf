using Jem.CommonLibrary22;

using Omega.WpfCommon1;

using System.Windows.Input;

namespace Omega.WpfControllers1
{
    public  class AbstractJobController : CNotifyPropertyChanged
    {
        public bool IsJobRunning { get; protected set; }

        #region Commands

        RelayCommand? _JobBeginCommand, _JobEndCommand;
        public ICommand JobBeginCommand => _JobBeginCommand ??= new RelayCommand(JobBegin, CanJobBegin);
        public ICommand JobEndCommand => _JobEndCommand ??= new RelayCommand(JobEnd, CanJobEnd);

        public virtual bool CanJobBegin() { return !IsJobRunning; }
        public bool JobBeginEnabled => CanJobBegin();
        public virtual void JobBegin() => IsJobRunning = true;

        public virtual bool CanJobEnd() => IsJobRunning;
        public bool JobEndEnabled => CanJobEnd();
        public virtual void JobEnd() => IsJobRunning = false;

        #endregion
    }
}