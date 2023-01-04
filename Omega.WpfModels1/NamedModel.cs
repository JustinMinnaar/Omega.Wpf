using Omega.WpfCommon1;

using System;
using System.ComponentModel;

namespace Omega.WpfModels1
{
    public class CNotifyPropertyChanged: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

    }
    public class IdNamedModel : CNotifyPropertyChanged
    {
        public override string ToString() => Name;

        public required Guid Id { get; set; }    

        public required string Name { get; set; }

        protected virtual void OnNameChanging(string oldValue, string newValue) { if (newValue == null) throw new PropertyNullException(nameof(Name)); }
        protected virtual void OnNameChanged(string oldValue, string newValue) { }
    }
}
