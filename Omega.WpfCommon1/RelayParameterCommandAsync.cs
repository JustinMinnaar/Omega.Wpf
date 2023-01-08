using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Omega.WpfCommon1;

public class RelayParameterCommandAsync : ICommand
{
    private readonly Func<object?, Task> _execute;
    private readonly Func<object?, bool>? _canExecute;
    private bool isExecuting;

    public RelayParameterCommandAsync(Func<object?, Task> execute) : this(execute, null) { }

    public RelayParameterCommandAsync(Func<object?, Task> execute, Func<object?, bool>? canExecute)
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

    public async void Execute(object? parameter)
    {
        isExecuting = true;
        try { await _execute(parameter); }
        finally { isExecuting = false; }
    }
}