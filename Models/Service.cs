namespace API.Models
{
    public class Service
    {
        public Service(string name, string description, string[] OptionsToSelect)
        {
            this.Name = name;
            this.Description = description;
            this.OptionsToSelect = OptionsToSelect;
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string[] OptionsToSelect { get; set; }

        public string? SelectedOption { get; set; }

    }
}