using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfLib;

public class MyCommand<T> : ICommand
{
    public MyCommand(Action<T> action, T parameter) => (Action, Parameter) = (action, parameter);

    public Action<T> Action { get; }
    public T Parameter { get; }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        Action(Parameter);
    }
}
