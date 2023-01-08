using System;
using System.Windows.Input;

namespace Omega.WpfCommon1;

public class RelayParameterCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?,bool>? _canExecute;
    private bool isExecuting;

    public RelayParameterCommand(Action<object?> execute) : this(execute, null) { }

    public RelayParameterCommand(Action<object?> execute, Func<object?, bool>? canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        if (isExecuting) return false;
        if (_canExecute == null) return true;
        return (_canExecute(parameter));
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object? parameter)
    {
        isExecuting = true;
        try { _execute(parameter); }
        finally { isExecuting = false; }
    }
}
