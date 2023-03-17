using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Logger = Tavstal.TLibrary.Helpers.LoggerHelper;

namespace Tavstal.TLibrary.Compatibility.Hooks
{
    public class TEconomyHook : Hook, IEconomyProvider
    {
        private RocketPlugin main => LibraryMain.Instance;

        public string GetCurrencyName()
        {
            string value = "Credits";
            try
            {
                value = GetConfigValue<string>("MoneyNameFull").ToString();
            }
            catch { }
            return value;
        }

        private MethodInfo _getBalanceMethod;
        private MethodInfo _getCashBalanceMethod;
        private MethodInfo _getBankBalanceMethod;
        private MethodInfo _getCryptoBalanceMethod;
        private MethodInfo _increaseBalanceMethod;
        private MethodInfo _increaseCashBalanceMethod;
        private MethodInfo _increaseCryptoBalanceMethod;
        private MethodInfo _increaseBankBalanceMethod;
        private MethodInfo _addTransactionMethod;
        private MethodInfo _getPlayerTransactionMethod;
        private MethodInfo _getBankCard;
        private MethodInfo _addBankCard;
        private MethodInfo _updateBankCard;
        private MethodInfo _removeBankCard;
        private MethodInfo _generateCardId;
        private MethodInfo _getTranslation;
        internal EventInfo _OnPlayerPayEvent;
        internal EventInfo _OnBalanceUpdateEvent;
        internal EventInfo _OnTransactionUpdateEvent;
        internal EventInfo _OnCardUpdateEvent;
        private object _databaseInstance;
        private object _pluginInstance;
        private Type helperType;
        private object teconomyConfig;

        public TEconomyHook() : base("teconomy", false) { }

        public override void OnLoad()
        {
            string section = "start";
            try
            {
                Logger.Log("Loading TEconomy hook...");


                var teconomyPlugin = R.Plugins.GetPlugins().FirstOrDefault(c => c.Name.EqualsIgnoreCase("teconomy"));
                var teconomyType = teconomyPlugin.GetType().Assembly.GetType("Tavstal.TEconomy.TEconomy");

                _pluginInstance =
                    teconomyType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(teconomyPlugin);

                var uconomyConfigInst = teconomyType.GetProperty("Configuration").GetValue(teconomyPlugin);
                teconomyConfig = uconomyConfigInst.GetType().GetProperty("Instance").GetValue(uconomyConfigInst);
                _databaseInstance = _pluginInstance.GetType().GetProperty("Database").GetValue(_pluginInstance);

                section = "database";
                _getBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "GetBalance", new[] { typeof(string) });

                _getCashBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "GetPlayerCash", new[] { typeof(CSteamID) });

                _getBankBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "GetPlayerBank", new[] { typeof(CSteamID) });

                _getCryptoBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "GetPlayerCrypto", new[] { typeof(CSteamID) });

                _increaseBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "IncreaseBalance", new[] { typeof(string), typeof(decimal) });

                _increaseBankBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "IncreasePlayerBank", new[] { typeof(CSteamID), typeof(decimal) });

                _increaseCashBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "IncreasePlayerCash", new[] { typeof(CSteamID), typeof(decimal) });

                _increaseCryptoBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "IncreasePlayerCrypto", new[] { typeof(CSteamID), typeof(decimal) });

                _addTransactionMethod = _databaseInstance.GetType().GetMethod(
                    "AddPlayerTransaction", new[] { typeof(CSteamID), typeof(string) });

                _getPlayerTransactionMethod = _databaseInstance.GetType().GetMethod(
                    "FindPlayerAccount", new[] { typeof(CSteamID) });

                _addBankCard = _databaseInstance.GetType().GetMethod(
                    "AddPlayerCard", new[] { typeof(CSteamID), typeof(string) });

                _updateBankCard = _databaseInstance.GetType().GetMethod(
                    "UpdatePlayerCard", new[] { typeof(CSteamID), typeof(string), typeof(string) });

                _removeBankCard = _databaseInstance.GetType().GetMethod(
                    "RemovePlayerCard", new[] { typeof(CSteamID), typeof(int), typeof(bool) });

                _getBankCard = _databaseInstance.GetType().GetMethod(
                    "GetBankCard", new[] { typeof(string) });
                section = "helper";

                helperType = teconomyPlugin.GetType().Assembly.GetType("Tavstal.TEconomy.Managers.Helper");
                _generateCardId = helperType.GetMethod("GenerateNewCardID");

                _getTranslation = _pluginInstance.GetType().GetMethod("Translate", new[] { typeof(string), typeof(object[]) });

                /*var tphoneType = main.GetType().Assembly.GetType("Tavstal.TPhone.TPhoneMain");
                var tphoneInstance = tphoneType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(main);

                section = "uc";

                var ucPlugin = _pluginInstance.GetType().GetProperty("uconomyPlugin", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_pluginInstance);
                var ucType = ucPlugin.GetType().Assembly.GetType("fr34kyn01535.Uconomy.Uconomy");
                var ucInstance = ucType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(ucPlugin);

                // Player Pay
                try
                {
                    _OnPlayerPayEvent = ucInstance.GetType().GetEvent("OnPlayerPay");
                    Delegate handler = Delegate.CreateDelegate(_OnPlayerPayEvent.EventHandlerType, main, tphoneInstance.GetType().GetMethod("Event_Uconomy_OnPlayerPay"), true);
                    _OnPlayerPayEvent.AddEventHandler(ucInstance, handler);

                }
                catch (Exception ex)
                {
                    Logger.LogError("TEconomy hook 1 -> ");
                    Logger.LogError(ex.ToString());
                }

                // Player Balance Updated
                try
                {
                    _OnBalanceUpdateEvent = ucInstance.GetType().GetEvent("OnBalanceUpdate");
                    Delegate handler = Delegate.CreateDelegate(_OnBalanceUpdateEvent.EventHandlerType, main, tphoneInstance.GetType().GetMethod("Event_Uconomy_OnPlayerBalanceUpdate"), true);
                    _OnBalanceUpdateEvent.AddEventHandler(ucInstance, handler);
                }
                catch (Exception ex)
                {
                    Logger.LogError("TEconomy hook 2 -> ");
                    Logger.LogError(ex.ToString());
                }

                // Player Transactions Updated
                try
                {
                    _OnTransactionUpdateEvent = _databaseInstance.GetType().GetEvent("OnTransactionUpdated");
                    Delegate handler = Delegate.CreateDelegate(_OnTransactionUpdateEvent.EventHandlerType, main, tphoneInstance.GetType().GetMethod("Event_TEconomy_OnTransactionUpdated"), true);
                    _OnTransactionUpdateEvent.AddEventHandler(_databaseInstance, handler);

                }
                catch (Exception ex)
                {
                    Logger.LogError("TEconomy hook 3 -> ");
                    Logger.LogError(ex.ToString());
                }

                // Player Card Updated
                try
                {
                    _OnCardUpdateEvent = _databaseInstance.GetType().GetEvent("OnCardUpdated");
                    Delegate handler = Delegate.CreateDelegate(_OnCardUpdateEvent.EventHandlerType, main, tphoneInstance.GetType().GetMethod("Event_TEconomy_OnCardUpdated"), true);
                    _OnCardUpdateEvent.AddEventHandler(_databaseInstance, handler);
                }
                catch (Exception ex)
                {
                    Logger.LogError("TEconomy hook 4 -> ");
                    Logger.LogError(ex.ToString());
                }*/

                Logger.Log("TEconomy hook loaded.");
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to load TEconomy hook. Section: " + section);
                Logger.LogError(e.ToString());
            }
        }

        public override void OnUnload() { }

        public override bool CanBeLoaded()
        {
            return R.Plugins.GetPlugins().Any(c => c.Name.EqualsIgnoreCase("teconomy"));
        }

        public T GetConfigValue<T>(string VariableName)
        {
            try
            {
                return (T)Convert.ChangeType(teconomyConfig.GetType().GetField(VariableName).GetValue(teconomyConfig), typeof(T));
            }
            catch
            {
                try
                {
                    return (T)Convert.ChangeType(teconomyConfig.GetType().GetProperty(VariableName).GetValue(teconomyConfig), typeof(T));
                }
                catch
                {
                    Logger.LogError($"Failed to get '{VariableName}' variable!");
                    return default;
                }
            }
        }

        public JObject GetConfig()
        {
            try
            {
                return JObject.FromObject(teconomyConfig.GetType());
            }
            catch
            {
                Logger.LogError($"Failed to get config jobj.");
                return null;
            }
        }

        public bool HasBuiltInTransactionSystem() { return true; }
        public bool HasBuiltInBankCardSystem() { return true; }

        public decimal Withdraw(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return Withdraw(player.CSteamID, amount, method);
        }

        public decimal Deposit(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return Deposit(player.CSteamID, amount, method);
        }

        public decimal GetBalance(UnturnedPlayer player, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return GetBalance(player.CSteamID, method);
        }

        public bool Has(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (GetBalance(player) - amount) >= 0;
        }

        public void AddTransaction(UnturnedPlayer player, Transaction transaction)
        {
            AddTransaction(player.CSteamID, transaction);
        }

        public decimal Withdraw(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            switch (method)
            {
                case EPaymentMethod.BANK:
                    {
                        return (decimal)_increaseBankBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player, -amount });
                    }
                case EPaymentMethod.CRYPTO:
                    {
                        return (decimal)_increaseCryptoBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player, -amount });
                    }
                case EPaymentMethod.WALLET:
                    {
                        return (decimal)_increaseCashBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player, -amount });
                    }
                default:
                    {
                        return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player.m_SteamID.ToString(), -amount });
                    }
            }
        }

        public decimal Deposit(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            switch (method)
            {
                case EPaymentMethod.BANK:
                    {
                        return (decimal)_increaseBankBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player, amount });
                    }
                case EPaymentMethod.CRYPTO:
                    {
                        return (decimal)_increaseCryptoBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player, amount });
                    }
                case EPaymentMethod.WALLET:
                    {
                        return (decimal)_increaseCashBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player, amount });
                    }
                default:
                    {
                        return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player.m_SteamID.ToString(), amount });
                    }
            }
        }

        public decimal GetBalance(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK)
        {
            switch (method)
            {
                case EPaymentMethod.BANK:
                    {
                        return (decimal)_getBankBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player});
                    }
                case EPaymentMethod.CRYPTO:
                    {
                        return (decimal)_getCryptoBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player});
                    }
                case EPaymentMethod.WALLET:
                    {
                        return (decimal)_getCashBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player});
                    }
                default:
                    {
                        return (decimal)_getBalanceMethod.Invoke(_databaseInstance, new object[] {
                            player.m_SteamID.ToString()});
                    }
            }
        }

        public bool Has(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (GetBalance(player, method) - amount) >= 0;
        }

        public void AddTransaction(CSteamID player, Transaction transaction)
        {
            _addTransactionMethod.Invoke(_databaseInstance, new object[] { player, JObject.FromObject(transaction).ToString(Formatting.None) });
        }

        public string Translate(string translationKey, params object[] placeholder)
        {
            return ((string)_getTranslation.Invoke(_pluginInstance, new object[] { translationKey, placeholder })).Replace("((", "<").Replace("))", ">");
        }

        public List<Transaction> GetTransactions(UnturnedPlayer player)
        {
            try
            {
                var result = _getPlayerTransactionMethod.Invoke(_databaseInstance, new object[] { player.CSteamID });

                if (result == null)
                    return new List<Transaction>();

                return JsonConvert.DeserializeObject<List<Transaction>>(JObject.FromObject(result)["Transactions"].ToString(Formatting.None));
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in GetTransactions(): " + ex);
                return new List<Transaction>();
            }

        }

        private JObject GetPlayerAccount(CSteamID steamID)
        {
            var result = _getPlayerTransactionMethod.Invoke(_databaseInstance, new object[] { steamID });
            return JObject.FromObject(result);
        }

        public void AddPlayerCard(CSteamID steamID, BankCard newCard)
        {
            newCard.CardID = Convert.ToString(_generateCardId.Invoke(helperType, null));
            _addBankCard.Invoke(_databaseInstance, new object[] { steamID, JObject.FromObject(newCard).ToString(Formatting.None) });
        }

        public void UpdatePlayerCard(CSteamID steamID, string id, BankCardDetails newData)
        {
            _updateBankCard.Invoke(_databaseInstance, new object[] { steamID, id, JObject.FromObject(newData).ToString(Formatting.None) });
        }

        public void RemovePlayerCard(CSteamID steamID, int index, bool isReversed = false)
        {
            _removeBankCard.Invoke(_databaseInstance, new object[] { steamID, index, isReversed });
        }

        public List<BankCard> GetPlayerCards(CSteamID steamID)
        {
            JObject account = GetPlayerAccount(steamID);
            return JsonConvert.DeserializeObject<List<BankCard>>(account["bankCards"].ToString(Formatting.None));
        }

        public BankCard GetPlayerCard(CSteamID steamID, int index)
        {
            var cards = GetPlayerCards(steamID);
            if (cards.IsValidIndex(index))
                return cards[index];
            else
                return null;
        }

        public BankCard GetPlayerCard(CSteamID steamID, string id)
        {
            return GetPlayerCards(steamID).Find(x => x.CardID == id);
        }
    }
}
