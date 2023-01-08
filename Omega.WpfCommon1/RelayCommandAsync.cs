using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Omega.WpfCommon1;

public class RelayCommandAsync : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool isExecuting;

    public RelayCommandAsync(Func<Task> execute) : this(execute, null) { }

    public RelayCommandAsync(Func<Task> execute, Func<bool>? canExecute)
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

    public async void Execute(object? _)
    {
        isExecuting = true;
        try { await _execute(); }
        finally { isExecuting = false; }
    }
}
