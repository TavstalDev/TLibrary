using System;
using Tavstal.TLibrary.Models.Database.Attributes;

namespace Tavstal.TLibrary.Models.Economy
{
    /// <inheritdoc/>
    public class BankCard : IBankCard
    {
        /// <inheritdoc/>
        [SqlMember(columnType: "varchar(34)", isPrimaryKey: true)]
        public string Iban { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "varchar(16)", isUniqueKey: true)]
        public string Number { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "varchar(3)")]
        public string Cvc { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "varchar(8)")]
        public string PinCode { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(isUnsigned: true)]
        public ulong HolderId { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "decimal(18,2)")]
        public decimal BalanceUsed { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "decimal(18,2)")]
        public decimal BalanceLimit { get; set; }
        
        /// <inheritdoc/>
        [SqlMember(columnType: "tinyint(1)")]
        public bool IsActive { get; set; }
        
        /// <summary>
        /// This variable is used to check if the card is in an ATM or not
        /// <br/>Note: If it's true then the card shouldn't be able to be used outside of that specific ATM
        /// </summary>
        [SqlMember(columnType: "tinyint(1)")]
        public bool IsInAtm { get; set; }
       
        /// <inheritdoc/>
        [SqlMember]
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// Creates a new bank card with default values.
        /// </summary>
        public BankCard()
        {
            Iban = string.Empty;
            Number = string.Empty;
            Cvc = string.Empty;
            PinCode = string.Empty;
            HolderId = 0;
            BalanceUsed = 0;
        }
        
        /// <summary>
        /// Creates a new bank card with the given details.
        /// </summary>
        /// <param name="iban">The IBAN of the bank card.</param>
        /// <param name="number">The card number.</param>
        /// <param name="cvc">The CVC code of the card.</param>
        /// <param name="pinCode">The PIN code of the card.</param>
        /// <param name="holderId">The ID of the card holder.</param>
        /// <param name="balanceUsed">The amount of balance already used.</param>
        /// <param name="balanceLimit">The maximum balance limit of the card.</param>
        /// <param name="isActive">Whether the card is active.</param>
        /// <param name="isInAtm">Whether the card is currently in an ATM.</param>
        /// <param name="expireDate">The expiration date of the card.</param>
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

        /// <summary>
        /// Returns the card number formatted with spaces every 4 digits.
        /// </summary>
        /// <returns>The formatted card number.</returns>
        public string GetFormatedNumber() =>
            Number.Insert(12, " ").Insert(8, " ").Insert(4, " ");

        /// <summary>
        /// Returns the first 17 characters of the IBAN.
        /// </summary>
        /// <returns>The short IBAN string.</returns>
        public string GetShortIban() =>
            Iban.Substring(0, 17);
    }
}
