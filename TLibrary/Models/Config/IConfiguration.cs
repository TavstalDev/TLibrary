using Tavstal.TLibrary.Models.Logging;

namespace Tavstal.TLibrary.Models.Config
{
    public interface IConfiguration
    {
        string GetFileName();

        string GetFilePath();
        
        ELogLevel GetLogLevel();
        
        void SetLogLevel(ELogLevel logLevel);

        string GetLocale();
        
        void SetLocale(string locale);

        bool GetDownloadLocalePacks();
        
        void SetDownloadLocalePacks(bool downloadLocalePacks);
        
        void LoadDefaults();
        
        bool Verify();

        T? ReadConfig<T>() where T : class;

        void Save();
    }
}
