using LibAmiibo.Data.AppData;

namespace LibAmiibo.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AppDataInitializationTitleIDAttribute : Attribute
    {
        public Title TitleID { get; }

        public AppDataInitializationTitleIDAttribute(string titleId)
        {
            this.TitleID = Title.FromTitleID(titleId);
        }
    }
}
