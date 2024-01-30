namespace Tavstal.TLibrary.Compatibility
{
    /// <summary>
    /// Base interface for all configurations.
    /// </summary>
    public interface IConfigurationBase
    {
        /// <summary>
        /// Loads the default values for the configuration.
        /// </summary>
        void LoadDefaults();
    }
}
