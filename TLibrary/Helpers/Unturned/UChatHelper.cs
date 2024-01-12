using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Compatibility.Interfaces;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers
{
    /// <summary>
    /// Unturned chat helper
    /// </summary>
    internal static class UChatHelper
    {
        //private static string Translate(bool addPrefix, string key, params object[] args) => TAdvancedHealthMain.Instance.Translate(addPrefix, key, args);

        public static void ServerSendChatMessage(string text, string icon = null, SteamPlayer fromPlayer = null, SteamPlayer toPlayer = null, EChatMode mode = EChatMode.GLOBAL)
        => ChatManager.serverSendMessage(text, Color.white, fromPlayer, toPlayer, mode, icon, true);

        /// <summary>
        /// Send plain text chat message to a specific player
        /// </summary>
        /// <param name="toPlayer"></param>
        /// <param name="text"></param>
        public static void SendPlainChatMessage(SteamPlayer toPlayer, string text)
        {
            string icon = "";
            ServerSendChatMessage(text, icon, null, toPlayer, EChatMode.GLOBAL);
        }

        /// <summary>
        /// Send chat message as command reply to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendCommandReply(IPlugin plugin, object toPlayer, string translation, params object[] args)
        {
            string icon = "";
            if (toPlayer is SteamPlayer steamPlayer)
                ServerSendChatMessage(FormatHelper.FormatTextV2(plugin.Localize(true, translation, args)), icon, null, steamPlayer, EChatMode.GLOBAL);
            else if (toPlayer is UnturnedPlayer player)
                ServerSendChatMessage(FormatHelper.FormatTextV2(plugin.Localize(true, translation, args)), icon, null, player.SteamPlayer(), EChatMode.GLOBAL);
            else
                plugin.GetLogger().LogRichCommand(plugin.Localize(false, translation, args));
        }

        /// <summary>
        /// Send chat message as command reply to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendPlainCommandReply(IPlugin plugin, object toPlayer, string translation, params object[] args)
        {
            string icon = "";
            if (toPlayer is SteamPlayer steamPlayer)
                ServerSendChatMessage(FormatHelper.FormatTextV2(string.Format(translation, args)), icon, null, steamPlayer, EChatMode.GLOBAL);
            else if (toPlayer is UnturnedPlayer player)
                ServerSendChatMessage(FormatHelper.FormatTextV2(string.Format(translation, args)), icon, null, player.SteamPlayer(), EChatMode.GLOBAL);
            else
                plugin.GetLogger().LogRichCommand(string.Format(translation, args));
        }

        /// <summary>
        /// Send chat message to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendChatMessage(IPlugin plugin, SteamPlayer toPlayer, string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(FormatHelper.FormatTextV2(plugin.Localize(true, translation, args)), icon, null, toPlayer, EChatMode.GLOBAL);
        }


        /// <summary>
        /// Send chat message to all players
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendChatMessage(IPlugin plugin, string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(plugin.Localize(true, translation, args), icon, null, null, EChatMode.GLOBAL);
        }
    }
}
