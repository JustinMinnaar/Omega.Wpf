using System;
using System.Windows.Input;

namespace Omega.WpfCommon1;

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;
    private bool isExecuting;

    public RelayCommand(Action execute) : this(execute, null) { }

    public RelayCommand(Action execute, Func<bool>? canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? _)
    {
        if (isExecuting) return false;
        if (_canExecute == null) return true;
        return (_canExecute());
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object? _)
    {
        isExecuting = true;
        try { _execute(); }
        finally { isExecuting = false; }
    }
}
