using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
{
    public interface IEconomyProvider : IPluginProvider
    {
        bool HasBuiltInTransactionSystem();
        bool HasBuiltInBankCardSystem();
        string GetCurrencyName();

        decimal Withdraw(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK);

        decimal Deposit(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK);

        decimal GetBalance(UnturnedPlayer player, EPaymentMethod method = EPaymentMethod.BANK);

        bool Has(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK);

        void AddTransaction(UnturnedPlayer player, Transaction transaction);

        decimal Withdraw(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK);

        decimal Deposit(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK);

        decimal GetBalance(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK);

        bool Has(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK);

        void AddTransaction(CSteamID player, Transaction transaction);

        List<Transaction> GetTransactions(UnturnedPlayer player);

        void AddPlayerCard(CSteamID steamID, BankCard newCard);

        void UpdatePlayerCard(CSteamID steamID, string id, BankCardDetails newData);

        void RemovePlayerCard(CSteamID steamID, int index, bool isReversed = false);

        List<BankCard> GetPlayerCards(CSteamID steamID);

        BankCard GetPlayerCard(CSteamID steamID, int index);

        BankCard GetPlayerCard(CSteamID steamID, string id);
    }
}
