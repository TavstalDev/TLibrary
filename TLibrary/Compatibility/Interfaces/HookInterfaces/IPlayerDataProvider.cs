using System;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility.Interfaces.HookInterfaces
{
    public interface IPlayerDataProvider
    {
        #region  Sync
        bool IsExists(ulong steamId);

        bool IsOnline(ulong steamId);

        void AddPlayerData(ulong steamId, string steamName, string characterName);

        void UpdatePlayerData(ulong steamId, string newSteamName, string newCharacterName, DateTime newLastLogin);

        void RemovePlayerData(ulong steamId);

        string GetSteamName(ulong steamId);

        string GetCharacterName(ulong steamId);

        DateTime GetFirstLogin(ulong steamId);

        DateTime GetLastLogin(ulong steamId);
        #endregion
        #region  Async
        Task<bool> IsExistsAsync(ulong steamId);

        Task AddPlayerDataAsync(ulong steamId, string steamName, string characterName);

        Task UpdatePlayerDataAsync(ulong steamId, string newSteamName, string newCharacterName, DateTime newLastLogin);

        Task RemovePlayerDataAsync(ulong steamId);

        Task<string> GetSteamNameAsync(ulong steamId);

        Task<string> GetCharacterNameAsync(ulong steamId);

        Task<DateTime> GetFirstLoginAsync(ulong steamId);

        Task<DateTime> GetLastLoginAsync(ulong steamId);
        #endregion
    }
}