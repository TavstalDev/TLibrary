using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TLibrary.Helpers
{
    internal static class UnturnedHelper
    {
        private static string Translate(string key, params object[] args) => LibraryMain.Instance.Translate(key, args);

        public static void SendChatMessage(string text, string icon = null, SteamPlayer fromPlayer = null, SteamPlayer toPlayer = null, EChatMode mode = EChatMode.GLOBAL)
        => ChatManager.serverSendMessage(text.Replace("((", "<").Replace("))", ">"), Color.white, fromPlayer, toPlayer, mode, icon, true);

        public static void SendChatMessage(SteamPlayer toPlayer, string translation, params object[] args)
        {
            string icon = "";
            SendChatMessage(Translate(translation, args).Replace("((", "<").Replace("))", ">"), icon, null, toPlayer, EChatMode.GLOBAL);
        }

        public static void SendChatMessage(string translation, params object[] args)
        {
            string icon = "";
            SendChatMessage(Translate(translation, args).Replace("((", "<").Replace("))", ">"), icon, null, null, EChatMode.GLOBAL);
        }
    }
}
