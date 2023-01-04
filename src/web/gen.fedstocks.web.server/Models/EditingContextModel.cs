namespace gen.fedstocks.web.server.Models;

public class EditingContextModel
{
    public EditingContextModel(string componentId, bool onGettingFocus)
    {
        ComponentId = componentId;
        OnGettingFocus = onGettingFocus;
    }

    public string ComponentId { get; }

    public bool OnGettingFocus { get; }
}