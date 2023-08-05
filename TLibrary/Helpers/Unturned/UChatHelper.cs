using RestSharp.Extensions;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.Util;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers
{
    internal static class UChatHelper
    {
        //private static string Translate(bool addPrefix, string key, params object[] args) => TAdvancedHealthMain.Instance.Translate(addPrefix, key, args);

        public static void ServerSendChatMessage(string text, string icon = null, SteamPlayer fromPlayer = null, SteamPlayer toPlayer = null, EChatMode mode = EChatMode.GLOBAL)
        => ChatManager.serverSendMessage(text, Color.white, fromPlayer, toPlayer, mode, icon, true);

        /*public static void SendCommandReply(object toPlayer, string translation, params object[] args)
        {
            string icon = "";
            if (toPlayer is SteamPlayer steamPlayer)
                ServerSendChatMessage(FormatHelper.FormatTextV2(Translate(true, translation, args)), icon, null, steamPlayer, EChatMode.GLOBAL);
            else
                LoggerHelper.LogRichCommand(Translate(false, translation, args));
        }

        public static void SendChatMessage(SteamPlayer toPlayer, string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(FormatHelper.FormatTextV2(Translate(true, translation, args)), icon, null, toPlayer, EChatMode.GLOBAL);
        }

        public static void SendChatMessage(string translation, params object[] args)
        {
            string icon = "";
            ServerSendChatMessage(Translate(true, translation, args), icon, null, null, EChatMode.GLOBAL);
        }*/

        public static void SendChatMessageUntranslated(SteamPlayer toPlayer, string text)
        {
            string icon = "";
            ServerSendChatMessage(text, icon, null, toPlayer, EChatMode.GLOBAL);
        }
    }
}
