using PropertyChanged;

using PropertyChanging;

using System;
using System.ComponentModel;

namespace Omega.WpfModels1
{
    public class NamedModel : INotifyPropertyChanged
    {
        public override string ToString() => Name;

        public event PropertyChangedEventHandler? PropertyChanged;

        public required string Name { get; set; }

        protected virtual void OnNameChanging(string oldValue, string newValue) { if (newValue == null) throw new ArgumentNullException(nameof(Name)); }
        protected virtual void OnNameChanged(string oldValue, string newValue) { }
    }
}
