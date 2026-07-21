namespace Tavstal.TLibrary.Models.Config
{
    public interface IConfiguration
    {
        string GetFileName();

        string GetFilePath();

        GeneralConfig GetGeneral();
        
        void LoadDefaults();
        
        bool Verify();

        T? ReadConfig<T>() where T : class;

        void Save();
    }
}
