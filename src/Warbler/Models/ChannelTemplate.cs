namespace Warbler.Models
{
    public class ChannelTemplate
    {
        /// <summary>
        ///   Parameterless ctor required by EF.
        /// </summary>
        public ChannelTemplate() {}

        public ChannelTemplate(string name, string description)
            => (Name, Description) = (name, description);

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
