using System;
using Rocket.Unturned.Player;
using Steamworks;
using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Models.Economy
{
    /// <inheritdoc/>
    public class Transaction : ITransaction
    {
        /// <inheritdoc/>
        [SqlMember(columnType: "varchar(36)", isPrimaryKey: true)]
        public string Id { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "tinyint(1)")]
        public ETransaction Type { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "tinyint(1)")]
        public EPaymentMethod PaymentMethod { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(isNullable: true, columnType: "varchar(64)")]
        public string StoreName { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(isUnsigned: true)]
        public ulong PayerId { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(isUnsigned: true)]
        public ulong PayeeId { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        /// <inheritdoc/>
        [SqlMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Creates a new transaction with the given ID and details.
        /// </summary>
        /// <param name="id">The unique identifier of the transaction.</param>
        /// <param name="type">The type of the transaction.</param>
        /// <param name="method">The payment method used.</param>
        /// <param name="storename">The name of the store involved.</param>
        /// <param name="payer">The ID of the payer.</param>
        /// <param name="payee">The ID of the payee.</param>
        /// <param name="amount">The amount of the transaction.</param>
        /// <param name="date">The date of the transaction.</param>
        public Transaction(string id, ETransaction type, EPaymentMethod method, string storename, ulong payer, ulong payee, decimal amount, DateTime date) 
        {
            Id = id;
            Type = type; 
            PaymentMethod = method; 
            StoreName = storename; 
            PayerId = payer; 
            PayeeId = payee;
            Amount = amount; 
            Date = date; 
        }
        
        /// <summary>
        /// Creates a new transaction with an auto-generated GUID and the given details.
        /// </summary>
        /// <param name="type">The type of the transaction.</param>
        /// <param name="method">The payment method used.</param>
        /// <param name="storename">The name of the store involved.</param>
        /// <param name="payer">The ID of the payer.</param>
        /// <param name="payee">The ID of the payee.</param>
        /// <param name="amount">The amount of the transaction.</param>
        /// <param name="date">The date of the transaction.</param>
        public Transaction(ETransaction type, EPaymentMethod method, string storename, ulong payer, ulong payee, decimal amount, DateTime date) 
        {
            Id = Guid.NewGuid().ToString();
            Type = type; 
            PaymentMethod = method; 
            StoreName = storename; 
            PayerId = payer; 
            PayeeId = payee;
            Amount = amount; 
            Date = date; 
        }

        /// <summary>
        /// Creates a new transaction with default values and an auto-generated ID.
        /// </summary>
        public Transaction()
        {
            Id = Guid.NewGuid().ToString();
            StoreName = string.Empty;
        }

        /// <summary>
        /// Gets the transaction operator character based on the transaction type and amount.
        /// </summary>
        /// <returns>The transaction operator character.</returns>
        public char GetTransactionOperator()
        {
            if (Amount == 0)
                return '\0';

            switch (Type)
            {
                case ETransaction.DEPOSIT:
                    return '+';
                case ETransaction.WITHDRAW:
                    return '-';
                case ETransaction.REFUND:
                    return '+';
                case ETransaction.SALE:
                    return '-';
                case ETransaction.PURCHASE:
                    return '-';
                case ETransaction.PAYMENT:
                    return Amount > 0 ? '+' : '-';
                default:
                    return '\0';
            }
        }

        /// <summary>
        /// Gets the transaction operator character based on the transaction type and amount.
        /// </summary>
        /// <param name="type">The type of the transaction.</param>
        /// <param name="amount">The amount involved in the transaction.</param>
        /// <returns>The transaction operator character.</returns>
        public static char GetTransactionOperator(ETransaction type, decimal amount)
        {
            if (amount == 0)
                return '\0';

            switch (type)
            {
                case ETransaction.DEPOSIT:
                    return '+';
                case ETransaction.WITHDRAW:
                    return '-';
                case ETransaction.REFUND:
                    return '+';
                case ETransaction.SALE:
                    return '-';
                case ETransaction.PURCHASE:
                    return '-';
                case ETransaction.PAYMENT:
                    return amount > 0 ? '+' : '-';
                default:
                    return '\0';
            }
        }

        /// <summary>
        /// Gets the transaction name based on the transaction type.
        /// </summary>
        /// <param name="plugin">The plugin instance used for localization.</param>
        /// <param name="player">The player involved in the transaction.</param>
        /// <returns>The transaction name as <see cref="string"/></returns>
        public string GetTransactionName(IPlugin plugin, UnturnedPlayer player)
        {
            string name = plugin.Localize("ui_unknown");
            switch (Type)
            {
                case ETransaction.DEPOSIT:
                case ETransaction.WITHDRAW:
                {
                    name = player.CharacterName;
                    break;
                }
                case ETransaction.REFUND:
                case ETransaction.SALE:
                case ETransaction.PURCHASE:
                {
                    name = StoreName;
                    break;
                }
                case ETransaction.PAYMENT:
                {
                    if (player.CSteamID.m_SteamID == PayeeId)
                    {
                        UnturnedPlayer otherPlayer = UnturnedPlayer.FromCSteamID((CSteamID)PayerId);
                        name = otherPlayer != null ? otherPlayer.CharacterName : PayerId.ToString();
                    }
                    else
                    {
                        UnturnedPlayer otherPlayer = UnturnedPlayer.FromCSteamID((CSteamID)PayeeId);
                        name = otherPlayer != null ? otherPlayer.CharacterName : PayeeId.ToString();
                    }

                    break;
                }
            }
            return name;
        }
    }
}
