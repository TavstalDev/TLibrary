using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using Tavstal.TLibrary.Models.Economy;

namespace Tavstal.TLibrary.Models.Hooks
{
    /// <summary>
    /// Provides access to an economy plugin's balance, transaction, and bank-card systems.
    /// Extends <see cref="IPluginProvider"/> with financial operations for players.
    /// </summary>
    public interface IEconomyProvider : IPluginProvider
    {
        /// <summary>
        /// Gets whether the plugin provides a built-in transaction recording system.
        /// </summary>
        /// <returns><see langword="true"/> if transactions are supported; otherwise, <see langword="false"/>.</returns>
        bool HasTransactionSystem();

        /// <summary>
        /// Gets whether the plugin provides a built-in bank card system.
        /// </summary>
        /// <returns><see langword="true"/> if bank cards are supported; otherwise, <see langword="false"/>.</returns>
        bool HasBankCardSystem();

        /// <summary>
        /// Gets the display name of the currency used by the economy plugin (e.g. "Coins").
        /// </summary>
        /// <returns>The currency name string.</returns>
        string GetCurrencyName();
        
        /// <summary>
        /// Asynchronously deducts (withdraws) money from the player's balance using the specified payment method.
        /// </summary>
        /// <param name="player">The Steam ID of the player.</param>
        /// <param name="amount">The amount to withdraw. Must be greater than zero.</param>
        /// <param name="method">The payment method to debit from. Defaults to <see cref="EPaymentMethod.BANK_ACCOUNT"/>.</param>
        /// <returns>A task whose result is the player's remaining balance after the withdrawal.</returns>
        Task<decimal> WithdrawAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Asynchronously adds (deposits) money to the player's balance using the specified payment method.
        /// </summary>
        /// <param name="player">The Steam ID of the player.</param>
        /// <param name="amount">The amount to deposit. Must be greater than zero.</param>
        /// <param name="method">The payment method to credit. Defaults to <see cref="EPaymentMethod.BANK_ACCOUNT"/>.</param>
        /// <returns>A task whose result is the player's new balance after the deposit.</returns>
        Task<decimal> DepositAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Asynchronously gets the player's current balance for the specified payment method.
        /// </summary>
        /// <param name="player">The Steam ID of the player.</param>
        /// <param name="method">The payment method to query. Defaults to <see cref="EPaymentMethod.BANK_ACCOUNT"/>.</param>
        /// <returns>A task whose result is the player's current balance.</returns>
        Task<decimal> GetBalanceAsync(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Asynchronously checks whether the player has at least the specified amount in the given payment method.
        /// </summary>
        /// <param name="player">The Steam ID of the player.</param>
        /// <param name="amount">The amount to check for.</param>
        /// <param name="method">The payment method to query. Defaults to <see cref="EPaymentMethod.BANK_ACCOUNT"/>.</param>
        /// <returns>A task whose result is <see langword="true"/> if the player's balance is greater than or equal to <paramref name="amount"/>; otherwise, <see langword="false"/>.</returns>
        Task<bool> HasAsync(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT);

        /// <summary>
        /// Asynchronously records a new transaction in the player's transaction history.
        /// </summary>
        /// <param name="player">The Steam ID of the player.</param>
        /// <param name="transaction">The <see cref="ITransaction"/> to record.</param>
        /// <returns>A task that completes when the transaction has been persisted.</returns>
        Task AddTransactionAsync(CSteamID player, ITransaction transaction);

        /// <summary>
        /// Asynchronously gets the full list of recorded transactions for the player.
        /// </summary>
        /// <param name="player">The Steam ID of the player.</param>
        /// <returns>A task whose result is a list of <see cref="ITransaction"/> entries, or an empty list if none exist.</returns>
        Task<List<ITransaction>> GetTransactionsAsync(CSteamID player);

        /// <summary>
        /// Asynchronously creates and assigns a new bank card to the player.
        /// </summary>
        /// <param name="steamID">The Steam ID of the player.</param>
        /// <param name="newCard">The <see cref="IBankCard"/> to create.</param>
        /// <returns>A task that completes when the card has been persisted.</returns>
        Task AddBankCardAsync(CSteamID steamID, IBankCard newCard);

        /// <summary>
        /// Asynchronously updates an existing bank card's usage and active state.
        /// </summary>
        /// <param name="cardId">The unique identifier of the card to update.</param>
        /// <param name="limitUsed">The new consumed-limit value to set.</param>
        /// <param name="isActive">Whether the card should be active.</param>
        /// <returns>A task that completes when the update has been persisted.</returns>
        Task UpdateBankCardAsync(string cardId, decimal limitUsed, bool isActive);

        /// <summary>
        /// Asynchronously and permanently removes a bank card by its identifier.
        /// </summary>
        /// <param name="cardId">The unique identifier of the card to remove.</param>
        /// <returns>A task that completes when the card has been removed.</returns>
        Task RemoveBankCardAsync(string cardId);

        /// <summary>
        /// Asynchronously gets all bank cards belonging to the specified player.
        /// </summary>
        /// <param name="steamID">The Steam ID of the player.</param>
        /// <returns>A task whose result is a list of <see cref="IBankCard"/> entries, or an empty list if the player has none.</returns>
        Task<List<IBankCard>> GetBankCardsByPlayerAsync(CSteamID steamID);

        /// <summary>
        /// Asynchronously gets a single bank card by its unique identifier.
        /// </summary>
        /// <param name="cardId">The unique identifier of the card.</param>
        /// <returns>A task whose result is the matching <see cref="IBankCard"/>, or <see langword="null"/> if not found.</returns>
        Task<IBankCard> GetBankCardByIdAsync(string cardId);
    }
}
