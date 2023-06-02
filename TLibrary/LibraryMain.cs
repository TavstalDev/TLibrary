using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Logger = Tavstal.TLibrary.Helpers.LoggerHelper;
using Tavstal.TLibrary.Managers;
using Tavstal.TLibrary.Helpers;
using Tavstal.TLibrary.Compatibility;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TLibrary
{
    public class LibraryMain
    {
        private static System.Version _version;
        private static bool isLoaded = false;
        private static DateTime _buildDate;
        private readonly static string _githubUrl = @"https://api.github.com/repos/TavstalDev/TLibrary/releases/latest";
        public static System.Version Version { get { return _version; } }
        public static DateTime BuildDate { get { return _buildDate; } }

        public static void OnLoad()
        {
            if (isLoaded)
                return;

            _version = Assembly.GetExecutingAssembly().GetName().Version;
            _buildDate = new DateTime(2000, 1, 1).AddDays(_version.Build).AddSeconds(_version.Revision * 2);

            #region CheckUpdate
            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(_githubUrl);
                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
                var respStream = httpRequest.GetResponse().GetResponseStream();

                if (respStream == null)
                {
                    Logger.LogError("Failed to check the latest version.");
                    return;
                }

                using (var reader = new StreamReader(respStream))
                {
                    var jsonData = reader.ReadToEnd();
                    var jsonObj = JObject.Parse(jsonData);
                    var body = jsonObj.GetValue("body")?.ToString() ?? "";
                    var assets = jsonObj.GetValue("assets").Children<JObject>();
                    Version latestVersion = Version.Parse(jsonObj.GetValue("tag_name").ToString());

                    if (latestVersion == null)
                    {
                        Logger.LogError("Failed to get the latest version.");
                        return;
                    }

                    if (Version < latestVersion)
                    {
                        Logger.Log("# TLibrary has been successfully loaded.");
                        Logger.Log("# Outdated version was detected.");
                        Logger.LogWarning($"# Latest Version: {Version}");
                        Logger.Log($"# Current Version: {Version}");
                        Logger.Log($"# Current Build Date: {BuildDate}");
                        Logger.LogWarning("# Downloading latest version...");
                        DownloadUpdate(assets.First().GetValue("browser_download_url")?.ToString() ?? string.Empty, Rocket.Core.Environment.LibrariesDirectory + "/Updates");
                        return;
                    }
                }

                Logger.Log("# TLibrary has been successfully loaded.");
                Logger.Log("# The library is up to date.");
                Logger.Log($"# Version: {Version}");
                Logger.Log($"# Build Date: {BuildDate}");
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Failed to check for updates.");
                Logger.LogWarning($"Error: {ex.Message}");
                Logger.LogWarning("Try downloading manually from https://github.com/TavstalDev/TLibrary/releases");
            }
            #endregion
        }

        private static void DownloadUpdate(string downloadUrl, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = $"{path}/{downloadUrl.Substring(downloadUrl.LastIndexOf('/') + 1)}";

            if (File.Exists(filePath))
            {
                Logger.Log("# Latest release is already downloaded. Check 'YourServer/Rocket/Libraries/Updates/' folder.");
                return;
            }

            if (downloadUrl.IsNullOrEmpty())
            {
                Logger.LogWarning("# Failed to download the latest release.");
                return;
            }

            Logger.Log("# Downloading latest release...");
            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += (sender, e) =>
                {
                    Logger.Log("# Download finished. Check 'YourServer/Rocket/Libraries/Updates/' folder.");
                };

                client.DownloadProgressChanged += (sender, e) =>
                {
                    Logger.Log(
                        $"# Downloading {e.BytesReceived} of {e.TotalBytesToReceive} bytes. ({e.ProgressPercentage} %)");
                };

                client.DownloadFileAsync(new Uri(downloadUrl), filePath);
            }
        }

        public static void OnUnload()
        {
            if (!isLoaded)
                return;
        }
    }
}
