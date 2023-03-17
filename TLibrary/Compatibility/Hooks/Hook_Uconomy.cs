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
    public class UconomyHook : Hook, IEconomyProvider
    {
        private RocketPlugin main => LibraryMain.Instance;

        public string GetCurrencyName()
        {
            string value = "Credits";
            try
            {
                value = GetConfigValue<string>("MoneyName").ToString();
            }
            catch { }
            return value;
        }

        private MethodInfo _getBalanceMethod;
        private MethodInfo _increaseBalanceMethod;
        private MethodInfo _getTranslation;
        internal EventInfo _OnPlayerPayMethod;
        internal EventInfo _OnBalanceUpdateMethod;
        private object _databaseInstance;
        private object _pluginInstance;
        private object uconomyConfig;

        public UconomyHook() : base("uconomy", true) { }

        public override void OnLoad()
        {
            try
            {
                Logger.Log("Loading Uconomy hook...");

                var uconomyPlugin = R.Plugins.GetPlugins().FirstOrDefault(c => c.Name.EqualsIgnoreCase("uconomy"));
                var uconomyType = uconomyPlugin.GetType().Assembly.GetType("fr34kyn01535.Uconomy.Uconomy");
                _pluginInstance =
                    uconomyType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(uconomyPlugin);

                var uconomyConfigInst = uconomyType.GetProperty("Configuration").GetValue(uconomyPlugin);
                uconomyConfig = uconomyConfigInst.GetType().GetProperty("Instance").GetValue(uconomyConfigInst);

                _databaseInstance = _pluginInstance.GetType().GetField("Database").GetValue(_pluginInstance);

                _getBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "GetBalance", new[] { typeof(string) });

                _increaseBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "IncreaseBalance", new[] { typeof(string), typeof(decimal) });

                _getTranslation = _pluginInstance.GetType().GetMethod("Translate", new[] { typeof(string), typeof(object[]) });

                /*var tphoneType = TPhoneMain.Instance.GetType().Assembly.GetType("Tavstal.TPhone.TPhoneMain");
                var tphoneInstance = tphoneType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(main);

                try
                {
                    _OnPlayerPayMethod = _pluginInstance.GetType().GetEvent("OnPlayerPay");
                    Delegate handler = Delegate.CreateDelegate(_OnPlayerPayMethod.EventHandlerType, main, tphoneInstance.GetType().GetMethod("Event_Uconomy_OnPlayerPay2"), true);
                    _OnPlayerPayMethod.AddEventHandler(_pluginInstance, handler);

                }
                catch (Exception ex)
                {
                    Logger.LogError("Uconomy hook 1 -> ");
                    Logger.LogError(ex.ToString());
                }

                try
                {
                    _OnBalanceUpdateMethod = _pluginInstance.GetType().GetEvent("OnBalanceUpdate");
                    Delegate handler = Delegate.CreateDelegate(_OnBalanceUpdateMethod.EventHandlerType, main, tphoneInstance.GetType().GetMethod("Event_Uconomy_OnPlayerBalanceUpdate"), true);
                    _OnBalanceUpdateMethod.AddEventHandler(_pluginInstance, handler);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Uconomy hook 2 -> ");
                    Logger.LogError(ex.ToString());
                }*/

                Logger.LogException("Currency Name >> " + GetCurrencyName());
                Logger.LogException("Initial Balance >> " + GetConfigValue<decimal>("InitialBalance").ToString());
                Logger.Log("Uconomy hook loaded.");
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to load Uconomy hook");
                Logger.LogError(e.ToString());
            }
        }

        public override void OnUnload() { }

        public override bool CanBeLoaded()
        {
            return R.Plugins.GetPlugins().Any(c => c.Name.EqualsIgnoreCase("uconomy"));
        }

        public T GetConfigValue<T>(string VariableName)
        {
            try
            {
                return (T)Convert.ChangeType(uconomyConfig.GetType().GetField(VariableName).GetValue(uconomyConfig), typeof(T));
            }
            catch
            {
                try
                {
                    return (T)Convert.ChangeType(uconomyConfig.GetType().GetProperty(VariableName).GetValue(uconomyConfig), typeof(T));
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
                return JObject.FromObject(uconomyConfig.GetType());
            }
            catch
            {
                Logger.LogError($"Failed to get config jobj.");
                return null;
            }
        }

        public bool HasBuiltInTransactionSystem() { return false; }
        public bool HasBuiltInBankCardSystem() { return false; }
        public decimal Withdraw(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.CSteamID.m_SteamID.ToString(), -amount
            });
        }

        public decimal Deposit(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.CSteamID.m_SteamID.ToString(), amount
            });
        }

        public decimal GetBalance(UnturnedPlayer player, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (decimal)_getBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.CSteamID.m_SteamID.ToString()
            });
        }

        public bool Has(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (GetBalance(player) - amount) >= 0;
        }

        public void AddTransaction(UnturnedPlayer player, Transaction transaction)
        {

        }

        public decimal Withdraw(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.m_SteamID.ToString(), -amount
            });
        }

        public decimal Deposit(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.m_SteamID.ToString(), amount
            });
        }

        public decimal GetBalance(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (decimal)_getBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.m_SteamID.ToString()
            });
        }

        public bool Has(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK)
        {
            return (GetBalance(player) - amount) >= 0;
        }

        public string Translate(string translationKey, params object[] placeholder)
        {
            return ((string)_getTranslation.Invoke(_pluginInstance, new object[] { translationKey, placeholder })).Replace("((", "<").Replace("))", ">");
        }

        public void AddTransaction(CSteamID player, Transaction transaction)
        {
            AddTransaction(UnturnedPlayer.FromCSteamID(player), transaction);
        }

        public List<Transaction> GetTransactions(UnturnedPlayer player)
        {
            return new List<Transaction>();
        }

        public void AddPlayerCard(CSteamID steamID, BankCard newCard)
        {

        }

        public void UpdatePlayerCard(CSteamID steamID, string id, BankCardDetails newData)
        {

        }

        public void RemovePlayerCard(CSteamID steamID, int index, bool isReversed = false)
        {

        }

        public List<BankCard> GetPlayerCards(CSteamID steamID)
        {
            return null;
        }

        public BankCard GetPlayerCard(CSteamID steamID, int index)
        {
            return null;
        }

        public BankCard GetPlayerCard(CSteamID steamID, string id)
        {
            return null;
        }

        public BankCard GetCard(string id)
        {
            return null;
        }
    }
}
