namespace Framework.Interfaces
{
    public interface ISettingsBusiness
    {
        string GetSetting(string name, string defaultValue = null);
        void SetSetting(string name, string value);
    }
}
