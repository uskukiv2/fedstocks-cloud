using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace gen.fedstocks.web.Client.Application.Abstract;

public abstract class BaseViewModel : IReactiveObject
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    public bool IsInitialized { get; protected set; }

    public virtual async Task InitAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        IsInitialized = true;
    }

    public void DoPropertyChanged([CallerMemberName] string propertyName = "")
    {
        RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    public void RaisePropertyChanging(PropertyChangingEventArgs args)
    {
        PropertyChanging?.Invoke(this, args);
    }

    public void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke(this, args);
    }
}