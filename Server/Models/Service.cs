namespace API.Models
{
    public class Service
    {
        public Service(string name, string description, string[] optionsToSelect) =>
        (Name, Description, OptionsToSelect) = (name, description, optionsToSelect);
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string[] OptionsToSelect { get; set; }

        public string? SelectedOption { get; set; }

        public string? ExpireDate { get; set; }
    }
}