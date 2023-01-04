using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace gen.fedstocks.web.server.Components;

public partial class DragDropFileComponent : ComponentBase
{
    [Parameter] public string? Path { get; set; }

    [Inject] public IJSRuntime JS { get; set; }

    protected override Task OnParametersSetAsync()
    {
        StateHasChanged();

        return Task.CompletedTask;
    }

    private async Task InputFileChanged(InputFileChangeEventArgs args)
    {
        var image = await args.File.RequestImageFileAsync("image/*", 600, 600);
        await using var imageStream = image.OpenReadStream(1024 * 10244 * 10);
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);

        Path = $"data:image/*;base64,{Convert.ToBase64String(ms.ToArray())}";

        StateHasChanged();
    }
}

public class ImageDragDrop
{
    public string Path { get; set; }

    public Stream Stream { get; set; }
}