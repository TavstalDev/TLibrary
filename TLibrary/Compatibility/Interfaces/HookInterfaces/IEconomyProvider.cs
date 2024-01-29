using Steamworks;
using System.Collections.Generic;
using Tavstal.TLibrary.Compatibility.Interfaces.Economy;

namespace Tavstal.TLibrary.Compatibility.Economy
{
    /// <summary>
    /// Basic economy plugin provider interface used for hooks
    /// </summary>
    public interface IEconomyProvider : IPluginProvider
    {
        /// <summary>
        /// Checks if the plugin has built in transaction system
        /// </summary>
        /// <returns></returns>
        bool HasBuiltInTransactionSystem();
        /// <summary>
        /// Checks if the plugin has built in bank card system
        /// </summary>
        /// <returns></returns>
        bool HasBuiltInBankCardSystem();
        /// <summary>
        /// Gets the currancy name of the plugin
        /// </summary>
        /// <returns></returns>
        string GetCurrencyName();

        /// <summary>
        /// Deducts money from the player's balance based on method
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        decimal Withdraw(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Adds money to the player's balance based on method
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        decimal Deposit(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Gets the player's balance based on method
        /// </summary>
        /// <param name="player"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        decimal GetBalance(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Checks the player's balance based on method
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        bool Has(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Adds a transaction to the player's transaction list
        /// </summary>
        /// <param name="player"></param>
        /// <param name="transaction"></param>
        void AddTransaction(CSteamID player, ITransaction transaction);

        /// <summary>
        /// Gets the player's transaction list
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        List<ITransaction> GetTransactions(CSteamID player);

        /// <summary>
        /// Creates a new bank card for the player
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="newCard"></param>
        void AddBankCard(CSteamID steamID, IBankCard newCard);

        /// <summary>
        /// Updates the player's bank card
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="id"></param>
        /// <param name="newData"></param>
        void UpdateBankCard(CSteamID steamID, string id, IBankCard newData);

        /// <summary>
        /// Deletes the player's bank card
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="index"></param>
        /// <param name="isReversed"></param>
        void RemoveBankCard(CSteamID steamID, int index, bool isReversed = false);

        /// <summary>
        /// Gets the player's bank card list
        /// </summary>
        /// <param name="steamID"></param>
        /// <returns></returns>
        List<IBankCard> GetPlayerCards(CSteamID steamID);

        /// <summary>
        /// Gets the player's bank card based on index
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        IBankCard GetPlayerCard(CSteamID steamID, int index);

        /// <summary>
        /// Gets the player's bank card based on id
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IBankCard GetPlayerCard(CSteamID steamID, string id);
    }
}
