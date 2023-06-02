namespace LibAmiibo.Interfaces
{
    public interface IAmiiboInfoDataProvider
    {
        Task<bool> Refresh();

        string GetAmiiboName(long id);

        string GetGameSeriesName(int id);

        string GetCharacterName(int id);

        string GetSubCharacterName(long id);

        string GetTypeName(int id);

        string GetAmiiboSetName(int id);

        string GetAmiiboName(string hexId);

        string GetGameSeriesName(string hexId);

        string GetCharacterName(string hexId);

        string GetSubCharacterName(string hexId);

        string GetTypeName(string hexId);

        string GetAmiiboSetName(string hexId);
    }
}
