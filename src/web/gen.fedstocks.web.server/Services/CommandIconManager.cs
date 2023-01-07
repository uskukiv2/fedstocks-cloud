using gen.fedstocks.web.server.Models;
using MudBlazor;

namespace gen.fedstocks.web.server.Services
{
    public class CommandIconManager
    {
        private IEnumerable<CommandIcon> _icons;

        public CommandIconManager()
        {
            _icons = new List<CommandIcon>()
            {
                new()
                {
                    Name = CommandNames.AddCommandName,
                    Icon = Icons.Material.Rounded.Add
                },
                new()
                {
                    Name = CommandNames.SaveCommandName,
                    Icon = Icons.Material.Rounded.Done
                },
                new()
                {
                   Name = CommandNames.EditCommandName,
                   Icon = Icons.Material.Rounded.Edit
                },
                new()
                {
                    Name = CommandNames.CancelCommandName,
                    Icon = Icons.Material.Rounded.Close
                }
            };
        }

        public string GetIcon(string icon)
        {
            return _icons.SingleOrDefault(x => x.Name == icon).Icon;
        }
    }
}
