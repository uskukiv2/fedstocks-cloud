namespace gen.fed.ui.Models;

public class ViewModelChangedArgs : EventArgs
{
    public ViewModelChangedArgs(string name, string localizedName)
    {
        Name = name;
        LocalizedName = localizedName;
    }

    public string Name { get; }

    public string LocalizedName { get; }
}