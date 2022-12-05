
namespace ApiController.Examples.Models
{
    public class SetDisplayNameCommand
    {
        public SetDisplayNameCommand(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; set; }
    }
}