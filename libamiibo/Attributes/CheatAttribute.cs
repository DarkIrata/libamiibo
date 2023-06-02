namespace LibAmiibo.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CheatAttribute : Attribute
    {
        public CheatType DisplayType { get; }

        public string Section { get; }

        public string Name { get; }

        public string Description { get; set; }

        public uint Min { get; set; }

        public uint Max { get; set; }

        public CheatAttribute(CheatType displayType, string section, string name)
        {
            this.DisplayType = displayType;
            this.Section = section;
            this.Name = name;
        }
    }
}
