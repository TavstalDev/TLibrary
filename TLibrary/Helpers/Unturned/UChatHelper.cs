using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Plugin;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    /// <summary>
    /// Unturned chat helper
    /// </summary>
    public static class UChatHelper
    {
        public static void ServerSendChatMessage(string text, string icon = null, SteamPlayer fromPlayer = null,  SteamPlayer toPlayer = null, EChatMode mode = EChatMode.GLOBAL)
        {
            // Main thread execution error fix
            MainThreadDispatcher.RunOnMainThread(() =>
            {
                try
                {
                    ChatManager.serverSendMessage(text, Color.white, fromPlayer, toPlayer, mode, icon, true);
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogException("The serverSendMessage function must be called from unity's game main thread.");
                    LoggerHelper.LogError(ex);
                }
            });
        }

        /// <summary>
        /// Send plain text chat message to a specific player
        /// </summary>
        /// <param name="toPlayer"></param>
        /// <param name="text"></param>
        public static void SendPlainChatMessage(SteamPlayer toPlayer, string text)
        {
            string icon = "";
            ServerSendChatMessage(text, icon, null, toPlayer);
        }

        /// <summary>
        /// Send chat message as command reply to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendCommandReply(this IPlugin plugin, object toPlayer, string translation, params object[] args)
        {
            string icon = "";
            if (toPlayer is SteamPlayer steamPlayer)
            {
                ServerSendChatMessage(FormatHelper.FormatTextV2(plugin.Localize(true, translation, args)), icon, null,
                    steamPlayer);
            }
            else if (toPlayer is UnturnedPlayer player)
            {
                ServerSendChatMessage(FormatHelper.FormatTextV2(plugin.Localize(true, translation, args)), icon, null,
                    player.SteamPlayer());
            }
            else
                plugin.GetLogger().RichCommand(plugin.Localize(false, translation, args));
        }

        /// <summary>
        /// Send chat message as command reply to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendPlainCommandReply(this IPlugin plugin, object toPlayer, string translation, params object[] args)
        {
            string icon = "";
            if (toPlayer is SteamPlayer steamPlayer)
                ServerSendChatMessage(FormatHelper.FormatTextV2(string.Format(translation, args)), icon, null, steamPlayer);
            else if (toPlayer is UnturnedPlayer player)
                ServerSendChatMessage(FormatHelper.FormatTextV2(string.Format(translation, args)), icon, null, player.SteamPlayer());
            else
                plugin.GetLogger().RichCommand(string.Format(translation, args));
        }

        /// <summary>
        /// Send chat message to a specific player
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="toPlayer"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendChatMessage(this IPlugin plugin, SteamPlayer toPlayer, string translation, params object[] args)
        {
            string icon = "";
           ServerSendChatMessage(FormatHelper.FormatTextV2(plugin.Localize(true, translation, args)), icon, null, toPlayer);
        }


        /// <summary>
        /// Send chat message to all players
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="translation"></param>
        /// <param name="args"></param>
        public static void SendChatMessage(this IPlugin plugin, string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(plugin.Localize(true, translation, args), icon);
        }
    }
}
