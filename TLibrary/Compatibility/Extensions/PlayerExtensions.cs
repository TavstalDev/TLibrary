using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Components;

namespace Tavstal.TLibrary.Compatibility
{
    public static class PlayerExtensions
    {
        public static TLibraryComponent GetPhoneComponent(this UnturnedPlayer player)
        {
            try
            {
                return player.GetComponent<TLibraryComponent>();
            }
            catch
            {
                return null;
            }
        }

        public static bool isOnline(this CSteamID id)
        {
            return Provider.clients.Any(x => x.playerID.steamID == id);
        }

        public static bool isOnline(this UnturnedPlayer player)
        {
            return Provider.clients.Any(x => x.playerID.steamID == player.CSteamID);
        }
    }
}
