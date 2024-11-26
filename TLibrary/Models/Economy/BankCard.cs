using System;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TLibrary.Models.Economy
{
    /// <summary>
    /// BankCard object used to make economy related plugins more compatible
    /// </summary>
    [Serializable]
    public class BankCard : IBankCard
    {
        /// <summary>
        /// Id of the card
        /// </summary>
        [SqlMember(columnType: "varchar(34)", isPrimaryKey: true)]
        public string Iban { get; set; }
        /// <summary>
        /// Number of the card.
        /// Example: 1234123412341234
        /// </summary>
        [SqlMember(columnType: "varchar(16)", isPrimaryKey: true)]
        public string Number { get; set; }
        /// <summary>
        /// Card Verification Code of the card
        /// </summary>
        [SqlMember(columnType: "varchar(3)")]
        public string Cvc { get; set; }
        /// <summary>
        /// Pin code of the card
        /// </summary>
        [SqlMember(columnType: "varchar(8)")]
        public string PinCode { get; set; }
        /// <summary>
        /// SteamId of the card holderId
        /// </summary>
        [SqlMember(isUnsigned: true)]
        public ulong HolderId { get; set; }
        /// <summary>
        /// Balance use of the card
        /// </summary>
        [SqlMember(columnType: "decimal(18,2)")]
        public decimal BalanceUsed { get; set; }
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
        public bool IsInAtm { get; set; }
        /// <summary>
        /// The date when the card expires and can not be used anymore
        /// </summary>
        [SqlMember]
        public DateTime ExpireDate { get; set; }

        public BankCard() { }

        public BankCard(string iban, string number, string cvc, string pinCode, ulong holderId, decimal balanceUsed, decimal balanceLimit, bool isActive, bool isInAtm, DateTime expireDate)
        {
            Iban = iban;
            Number = number;
            Cvc = cvc;
            PinCode = pinCode;
            HolderId = holderId;
            BalanceUsed = balanceUsed;
            BalanceLimit = balanceLimit;
            IsActive = isActive;
            IsInAtm = isInAtm;
            ExpireDate = expireDate;
        }

        public string GetFormatedNumber()
        {
            return Number.Insert(12, " ").Insert(8, " ").Insert(4, " ");
        }

        public string GetShortIban()
        {
            return Iban.Substring(0, 17);
        }
    }
}
