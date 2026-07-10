using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Models.Plugin;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    /// <summary>
    /// Provides helper methods for sending chat messages in Unturned.
    /// </summary>
    public static class UChatHelper
    {
        /// <summary>
        /// Sends a chat message from the server, optionally with an icon, sender, and target player.
        /// </summary>
        /// <param name="text">The message text to send.</param>
        /// <param name="icon">The URL of an icon image to show next to the message (optional).</param>
        /// <param name="fromPlayer">The player who appears as the sender (optional).</param>
        /// <param name="toPlayer">The specific player to send the message to (optional).</param>
        /// <param name="mode">The chat mode (global, local, etc.). Defaults to global.</param>
        public static void ServerSendChatMessage(string text, string? icon = null, SteamPlayer? fromPlayer = null,  SteamPlayer? toPlayer = null, EChatMode mode = EChatMode.GLOBAL)
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
                    LoggerHelper.LogError("The serverSendMessage function must be called from unity's game main thread.");
                    LoggerHelper.LogError(ex);
                }
            });
        }

        /// <summary>
        /// Sends a plain text chat message to a specific player without formatting.
        /// </summary>
        /// <param name="toPlayer">The player to receive the message.</param>
        /// <param name="text">The message text.</param>
        public static void SendPlainChatMessage(SteamPlayer toPlayer, string text)
        {
            string icon = "";
            ServerSendChatMessage(text, icon, null, toPlayer);
        }

        /// <summary>
        /// Sends a formatted command reply message to a specific player.
        /// </summary>
        /// <param name="plugin">The plugin sending the message.</param>
        /// <param name="toPlayer">The target player (SteamPlayer or UnturnedPlayer).</param>
        /// <param name="translation">The translation key to localize.</param>
        /// <param name="args">Optional format arguments for the translation.</param>
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
        /// Sends a plain (unlocalized) command reply message to a specific player.
        /// </summary>
        /// <param name="plugin">The plugin sending the message.</param>
        /// <param name="toPlayer">The target player (SteamPlayer or UnturnedPlayer).</param>
        /// <param name="translation">The raw text or format string.</param>
        /// <param name="args">Optional format arguments.</param>
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
        /// Sends a formatted and localized chat message to a specific player.
        /// </summary>
        /// <param name="plugin">The plugin sending the message.</param>
        /// <param name="toPlayer">The target player.</param>
        /// <param name="translation">The translation key to localize.</param>
        /// <param name="args">Optional format arguments for the translation.</param>
        public static void SendChatMessage(this IPlugin plugin, SteamPlayer toPlayer, string translation, params object[] args)
        {
            string icon = "";
           ServerSendChatMessage(FormatHelper.FormatTextV2(plugin.Localize(true, translation, args)), icon, null, toPlayer);
        }


        /// <summary>
        /// Sends a formatted and localized chat message to all players.
        /// </summary>
        /// <param name="plugin">The plugin sending the message.</param>
        /// <param name="translation">The translation key to localize.</param>
        /// <param name="args">Optional format arguments for the translation.</param>
        public static void SendChatMessage(this IPlugin plugin, string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(plugin.Localize(true, translation, args), icon);
        }
    }
}
