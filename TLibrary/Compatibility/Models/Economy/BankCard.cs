using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility.Database;
using Tavstal.TLibrary.Compatibility.Interfaces.Economy;

namespace Tavstal.TLibrary.Compatibility.Economy
{
    /// <summary>
    /// BankCard object used to make economy related plugins more compatible
    /// </summary>
    [Serializable]
    public class BankCard : IEconomyCard
    {
        /// <summary>
        /// Id of the card
        /// </summary>
        [SqlMember(columnType: "varchar(32)", isPrimaryKey: true)]
        public string Id { get; set; }
        /// <summary>
        /// Pincode of the card
        /// </summary>
        [SqlMember(columnType: "varchar(8)")]
        public string PinCode { get; set; }
        /// <summary>
        /// Pincode of the card
        /// </summary>
        [SqlMember(columnType: "varchar(3)")]
        public string SecurityCode { get; set; }
        /// <summary>
        /// SteamId of the card holderId
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong HolderId { get; set; }
        /// <summary>
        /// Balance use of the card
        /// </summary>
        [SqlMember(columnType: "decimal(18,2)")]
        public decimal BalanceUse { get; set; }
        /// <summary>
        /// Balance limit of the card
        /// </summary>
        [SqlMember(columnType: "decimal(18,2)")]
        public decimal BalanceLimit { get; set; }
        /// <summary>
        /// This variable is used to enable / disable the card
        /// </summary>
        [SqlMember(columnType: "tinyint(1)")]
        public bool IsActive { get; set; }
        /// <summary>
        /// This variable is used to check if the card is in an ATM or not
        /// <br/>Note: If it's true then the card shouldn't be able to be used outside of that specific ATM
        /// </summary>
        [SqlMember(columnType: "tinyint(1)")]
        public bool IsInATM { get; set; }
        /// <summary>
        /// The date when the card expires and can not be used anymore
        /// </summary>
        public DateTime ExpireDate { get; set; }

        public BankCard(string cardId, string securityCode, string pinCode, ulong holderId, decimal balance, decimal maxBalance, DateTime expireDate)
        {
            Id = cardId;
            SecurityCode = securityCode;
            PinCode = pinCode;
            HolderId = holderId;
            BalanceUse = balance;
            BalanceLimit = maxBalance;
            ExpireDate = expireDate;
            IsInATM = false;
            IsActive = false;
        }

        public BankCard(string cardId, string securityCode, string pinCode, ulong holderId, decimal balance, decimal maxBalance, DateTime expireDate, bool isActive, bool isInATM)
        {
            Id = cardId;
            SecurityCode = securityCode;
            PinCode = pinCode;
            HolderId = holderId;
            BalanceUse = balance;
            BalanceLimit = maxBalance;
            ExpireDate = expireDate;
            IsInATM = isInATM;
            IsActive = isActive;
        }

        public BankCard() { }
    }
}
