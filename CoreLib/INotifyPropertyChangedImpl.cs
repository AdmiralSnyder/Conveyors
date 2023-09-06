using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CoreLib;

public abstract class INotifyPropertyChangedImpl : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected internal void OnPropertyChanged([CallerMemberName] string name = "INVALID") => PropertyChanged?.Invoke(this, new(name));
}
