using Steamworks;
using System.Collections.Generic;
using Tavstal.TLibrary.Compatibility.Interfaces.Economy;
using System.Threading.Tasks;

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
        bool HasTransactionSystem();
        /// <summary>
        /// Checks if the plugin has built in bank card system
        /// </summary>
        /// <returns></returns>
        bool HasBankCardSystem();
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
        void UpdateBankCard(string cardId, decimal limitUsed, bool isActive);

        /// <summary>
        /// Deletes the player's bank card
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="index"></param>
        /// <param name="isReversed"></param>
        void RemoveBankCard(string cardId);

        /// <summary>
        /// Gets the player's bank card list
        /// </summary>
        /// <param name="steamID"></param>
        /// <returns></returns>
        List<IBankCard> GetBankCardsByPlayer(CSteamID steamID);

        /// <summary>
        /// Gets the player's bank card based on index
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        IBankCard GetBankCardById(string cardId);

        /// <summary>
        /// Deducts money from the player's balance based on method
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<decimal> WithdrawAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Adds money to the player's balance based on method
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<decimal> DepositAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Gets the player's balance based on method
        /// </summary>
        /// <param name="player"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<decimal> GetBalanceAsync(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Checks the player's balance based on method
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<bool> HasAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Adds a transaction to the player's transaction list
        /// </summary>
        /// <param name="player"></param>
        /// <param name="transaction"></param>
        Task AddTransactionAsync(CSteamID player, ITransaction transaction);

        /// <summary>
        /// Gets the player's transaction list
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        Task<List<ITransaction>> GetTransactionsAsync(CSteamID player);

        /// <summary>
        /// Creates a new bank card for the player
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="newCard"></param>
        Task AddBankCardAsync(CSteamID steamID, IBankCard newCard);

        /// <summary>
        /// Updates the player's bank card
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="id"></param>
        /// <param name="newData"></param>
        Task UpdateBankCardAsync(string cardId, decimal limitUsed, bool isActive);

        /// <summary>
        /// Deletes the player's bank card
        /// </summary>
        /// <param name="steamID"></param>
        /// <param name="index"></param>
        /// <param name="isReversed"></param>
        Task RemoveBankCardAsync(string cardId);

        /// <summary>
        /// Gets the player's bank card list
        /// </summary>
        /// <param name="steamID"></param>
        /// <returns></returns>
        Task<List<IBankCard>> GetBankCardsByPlayerAsync(CSteamID steamID);

        /// <summary>
        /// Gets the player's bank card based on index
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        Task<IBankCard> GetBankCardByIdAsync(string cardId);
    }
}
