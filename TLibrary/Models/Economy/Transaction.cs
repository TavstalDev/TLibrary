using System;
using Rocket.Unturned.Player;
using Steamworks;
using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TLibrary.Models.Economy
{
    /// <summary>
    /// Transaction
    /// </summary>
    [Serializable]
    public class Transaction : ITransaction
    {
        [SqlMember(columnType: "varchar(36)", isPrimaryKey: true)]
        public string Id { get; set; }
        /// <summary>
        /// Type of the transaction to check the transaction was sent or recieved by the payee
        /// <br/>For values check <see cref="ETransaction"/>
        /// </summary>
        [SqlMember(columnType: "tinyint(1)")]
        public ETransaction Type { get; set; }
        /// <summary>
        /// Type of the payment method, used to check the type of the money
        /// </summary>
        [SqlMember(columnType: "tinyint(1)")]
        public EPaymentMethod PaymentMethod { get; set; }
        /// <summary>
        /// Name of the store, can be used as displayName if the transaction was made between a player and a plugin, like TShop
        /// </summary>
        [SqlMember(isNullable: true, columnType: "varchar(64)")]
        public string StoreName { get; set; }
        /// <summary>
        /// SteamId of the payer, the player who sends the money
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong PayerId { get; set; }
        /// <summary>
        /// SteamId of the payer, the player who recieves the money
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong PayeeId { get; set; }
        /// <summary>
        /// Amout of the transaction
        /// </summary>
        [SqlMember(columnType: "decimal(18,2)")]
        public decimal Amount { get; set; }
        /// <summary>
        /// Date when the transaction was created
        /// </summary>
        [SqlMember]
        public DateTime Date { get; set; }

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

        public Transaction() { }


        /// <summary>
        /// Gets the transaction operator character based on the transaction type and amount.
        /// </summary>
        /// <returns>The transaction operator character.</returns>
        public char GetTransactionOperator()
        {
            if (Amount == 0)
                return new char();

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
                    return new char();
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
                return new char();

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
                    return new char();
            }
        }

        /// <summary>
        /// Gets the transaction name based on the transaction type.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="player"></param>
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
                            if (otherPlayer != null)
                                name = otherPlayer.CharacterName;
                            else
                                name = PayerId.ToString();
                        }
                        else
                        {
                            UnturnedPlayer otherPlayer = UnturnedPlayer.FromCSteamID((CSteamID)PayeeId);
                            if (otherPlayer != null)
                                name = otherPlayer.CharacterName;
                            else
                                name = PayeeId.ToString();
                        }
                        break;
                    }
            }
            return name;
        }
    }
}
