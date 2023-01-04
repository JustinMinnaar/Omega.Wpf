namespace Jem.CommonLibrary22;

public abstract class CNotifyPropertyChanged : INotifyPropertyChanged
{
#pragma warning disable CS0067
    public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
}

//public abstract class CNotifyPropertyChanged : INotifyPropertyChanged
//{
//    private readonly Dictionary<string, MethodInfo>? onPropertyNameChanged;

//    public CNotifyPropertyChanged()
//    {
//        var type = GetType();
//        var properties = type.GetPublicProperties();
//        var methods = type.GetPublicMethods(includeNonPublic: true);
//        foreach (var method in methods)
//        {
//            foreach (var property in properties)
//            {
//                var name = "On" + property.Name + "Changed";
//                if (method.Name == name)
//                {
//                    if (onPropertyNameChanged == null) onPropertyNameChanged = new Dictionary<string, MethodInfo>();
//                    onPropertyNameChanged.Add(property.Name, method);
//                }
//            }
//        }
//    }

//#pragma warning disable 0067

//    public event PropertyChangedEventHandler? PropertyChanged;

//#pragma warning restore 067

//    /// <summary>Set to true when any property is changed. Used internally to handle sync.</summary>
//    public bool Changed;

//    public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null!)
//    {
//        if (propertyName == null) return;

//        Changed = true;

//        VerifyPropertyName(propertyName);

//        if (onPropertyNameChanged != null)
//            if (onPropertyNameChanged.TryGetValue(propertyName, out var method))
//            {
//                method.Invoke(this, null);
//            }

//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }

//    [Conditional("DEBUG")]
//    [DebuggerStepThrough]
//    public void VerifyPropertyName(string? propertyName)
//    {
//        if (propertyName == null) return;

//        // Verify that the property name matches a real, public, instance property on this object.
//        if (TypeDescriptor.GetProperties(this)[propertyName] == null)
//        {
//            string msg = "Invalid property name: " + propertyName;
//            Debug.Fail(msg);
//        }
//    }
//}