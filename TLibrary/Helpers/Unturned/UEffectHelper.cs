using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TLibrary.Helpers.Unturned
{
    public static class UEffectHelper
    {
        public static void UpdatePagination<T>(ushort effectId, UnturnedPlayer player, string uiName, int arrayIndex, int page, int maxPage) where T : IPlayerComponent =>
            UpdatePagination<T>((short)effectId, player, uiName, arrayIndex, page, maxPage);

        public static void UpdatePagination<T>(short effectId, UnturnedPlayer player, string uiName, int arrayIndex, int page, int maxPage) where T : IPlayerComponent
        {
            T comp = player.GetComponent<T>();

            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#prev", page != 1);

            if (page == maxPage)
                EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#next", false);
            else
                EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#next", maxPage > 1);

            // Pages between 1 and 5
            if (maxPage <= 5)
            {
                EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#left", false);
                EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#centerleft", false);
                EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#centerright", false);
                EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#right", false);
                for (int i = 0; i < 5; i++)
                {
                    int uiIndex = i + 1;
                    if (uiIndex > maxPage)
                    {
                        EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#{uiIndex}", false);
                        comp.PageIndexes[arrayIndex][i] = -1;
                    }
                    else
                    {
                        if (page == uiIndex)
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#{uiIndex}", $"<color=#6C757D>{uiIndex}");
                        else
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#{uiIndex}", (uiIndex).ToString());
                        EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#{uiIndex}", true);
                        comp.PageIndexes[arrayIndex][i] = uiIndex;
                    }
                }
            }
            // Pages after 5
            else
            {
                // First Page Button
                if (page == 1)
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#1", $"<color=#6C757D>1");
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#1", true);
                    // Disable button
                    comp.PageIndexes[arrayIndex][0] = -1;
                }
                else
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#1", "1");
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#1", true);
                    comp.PageIndexes[arrayIndex][0] = 1;
                }

                // Button After First Page 
                if (page - 2 == 1 || page == 1)
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", "2");
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#left", false);
                    comp.PageIndexes[arrayIndex][1] = 2;
                }
                else if (page - 1 == 1)
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", "<color=#6C757D>2");
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#left", false);
                    comp.PageIndexes[arrayIndex][1] = -1;
                    // Disable button
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#left", true);
                }

                // Center 
                if (maxPage >= 6)
                {
                    if (maxPage == 6)
                    {
                        // DO NOTHING, JUST PANIC
                        if (page == 3)
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", $"<color=#6C757D>{page}");
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                            comp.PageIndexes[arrayIndex][2] = -1;
                        }
                        else if (page == 4)
                        {

                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", (page - 1).ToString());
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                            comp.PageIndexes[arrayIndex][1] = page - 1;

                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", $"<color=#6C757D>{page}");
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                            comp.PageIndexes[arrayIndex][2] = -1;

                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#4", (page + 1).ToString());
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#4", true);
                            comp.PageIndexes[arrayIndex][3] = page + 1;
                        }
                        else if (page == 5)
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", $"{page - 1}");
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                            comp.PageIndexes[arrayIndex][2] = page - 1;

                        }
                    }
                    else
                    {
                        if (page - 3 >= 1 && page + 3 <= maxPage)
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", (page - 1).ToString());
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                            comp.PageIndexes[arrayIndex][1] = page - 1;

                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", $"<color=#6C757D>{page}");
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                            comp.PageIndexes[arrayIndex][2] = -1;

                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#4", (page + 1).ToString());
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#4", true);
                            comp.PageIndexes[arrayIndex][3] = page + 1;
                        }
                        else
                        {
                            if (page <= 4)
                            {
                                if (page == 3)
                                {
                                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", $"<color=#6C757D>3");
                                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                                    // Disable button
                                    comp.PageIndexes[arrayIndex][2] = -1;
                                }
                                else
                                {
                                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", "3");
                                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                                    comp.PageIndexes[arrayIndex][2] = 3;
                                }

                                if (page == 4)
                                {
                                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#4", $"<color=#6C757D>4");
                                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#4", true);
                                    // Disable button
                                    comp.PageIndexes[arrayIndex][3] = -1;
                                }
                                else
                                {
                                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#4", "4");
                                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#4", true);
                                    comp.PageIndexes[arrayIndex][3] = 4;
                                }

                                EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#5", false);
                                comp.PageIndexes[arrayIndex][4] = -1;
                            }
                            else
                            {
                                if (page == maxPage - 3)
                                {
                                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", $"<color=#6C757D>{maxPage - 3}");
                                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                                    // Disable button
                                    comp.PageIndexes[arrayIndex][1] = -1;
                                }
                                else
                                {
                                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", (maxPage - 3).ToString());
                                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                                    comp.PageIndexes[arrayIndex][1] = maxPage - 3;
                                }

                                if (page == maxPage - 2)
                                {
                                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", $"<color=#6C757D>{maxPage - 2}");
                                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                                    // Disable button
                                    comp.PageIndexes[arrayIndex][2] = -1;
                                }
                                else
                                {
                                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", (maxPage - 2).ToString());
                                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                                    comp.PageIndexes[arrayIndex][2] = maxPage - 2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (page <= 4)
                    {
                        if (page == 3)
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", $"<color=#6C757D>3");
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                            // Disable button
                            comp.PageIndexes[arrayIndex][1] = -1;
                        }
                        else
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", "3");
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                            comp.PageIndexes[arrayIndex][1] = 3;
                        }

                        if (page == 4)
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", $"<color=#6C757D>4");
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                            // Disable button
                            comp.PageIndexes[arrayIndex][2] = -1;
                        }
                        else
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", "4");
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                            comp.PageIndexes[arrayIndex][2] = 4;
                        }

                        EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#4", true);
                    }
                    else
                    {
                        if (page == maxPage - 3)
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", (maxPage - 3).ToString());
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                            // Disable button
                            comp.PageIndexes[arrayIndex][1] = -1;
                        }
                        else
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#2", (maxPage - 3).ToString());
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#2", true);
                            comp.PageIndexes[arrayIndex][1] = maxPage - 3;
                        }

                        if (page == maxPage - 2)
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", (maxPage - 2).ToString());
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                            // Disable button
                            comp.PageIndexes[arrayIndex][2] = -1;
                        }
                        else
                        {
                            EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#3", (maxPage - 2).ToString());
                            EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#3", true);
                            comp.PageIndexes[arrayIndex][2] = maxPage - 2;
                        }
                    }
                }

                // Button before Last Page
                if (page + 2 == maxPage || page == maxPage)
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#4", (maxPage - 1).ToString());
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#4", true);
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#right", false);
                    comp.PageIndexes[arrayIndex][3] = maxPage - 1;
                }
                else if (page + 1 == maxPage)
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#4", $"<color=#6C757D>{page}");
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#4", true);
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#right", false);
                    comp.PageIndexes[arrayIndex][3] = -1;
                }
                else if (page - 3 <= 0)
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#4", "4");
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#4", true);
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#right", true);
                    comp.PageIndexes[arrayIndex][3] = 4;
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#dots#right", true);
                }

                // Last Page Button 
                if (page == maxPage)
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#5", $"<color=#6C757D>{maxPage}");
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#5", true);
                    // Disable button
                    comp.PageIndexes[arrayIndex][4] = -1;
                }
                else
                {
                    EffectManager.sendUIEffectText(effectId, player.SteamPlayer().transportConnection, true, $"tb_{uiName}#page#5", maxPage.ToString());
                    EffectManager.sendUIEffectVisibility(effectId, player.SteamPlayer().transportConnection, true, $"bt_{uiName}#page#5", true);
                    comp.PageIndexes[arrayIndex][4] = maxPage;
                }
            }
        }
    
        public static void SendRichUIText(short effectId, ITransportConnection connection, bool reliable, string uiName, string value)
        {
            EffectManager.sendUIEffectText(effectId, connection, reliable, uiName, FormatHelper.FormatTextV2(value));
        }
    }
}
