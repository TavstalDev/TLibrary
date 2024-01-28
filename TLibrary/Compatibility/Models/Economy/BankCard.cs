using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility.Database;

namespace Tavstal.TLibrary.Compatibility.Economy
{
    /// <summary>
    /// BankCard object used to make economy related plugins more compatible
    /// </summary>
    [Serializable]
    public class BankCard
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
        /// SteamId of the card owner
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong OwnerId { get; set; }
        /// <summary>
        /// Balance of the card
        /// </summary>
        [SqlMember(columnType: "decimal(18,2)")]
        public decimal Balance { get; set; }
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

        public BankCard(string code, string pincode, ulong owner, decimal balance, decimal maxbalance, DateTime expireDate)
        {
            Id = code;
            PinCode = pincode;
            OwnerId = owner;
            Balance = balance;
            BalanceLimit = maxbalance;
            ExpireDate = expireDate;
            IsInATM = false;
            IsActive = false;
        }

        public BankCard(string code, string pincode, ulong owner, decimal balance, decimal maxbalance, DateTime expireDate, bool isactive, bool isinatm)
        {
            Id = code;
            PinCode = pincode;
            OwnerId = owner;
            Balance = balance;
            BalanceLimit = maxbalance;
            ExpireDate = expireDate;
            IsInATM = isinatm;
            IsActive = isactive;
        }

        public BankCard() { }
    }
}
