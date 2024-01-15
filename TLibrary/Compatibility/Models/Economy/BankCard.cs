using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Id { get; set; }
        /// <summary>
        /// Pincode of the card
        /// </summary>
        public string PinCode { get; set; }
        /// <summary>
        /// SteamId of the card owner
        /// </summary>
        public ulong OwnerId { get; set; }
        /// <summary>
        /// Balance of the card
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// Balance limit of the card
        /// </summary>
        public decimal BalanceLimit { get; set; }
        /// <summary>
        /// This variable is used to enable / disable the card
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// This variable is used to check if the card is in an ATM or not
        /// <br/>Note: If it's true then the card shouldn't be able to be used outside of that specific ATM
        /// </summary>
        public bool IsInATM { get; set; }
        /// <summary>
        /// Transactions list of the card
        /// </summary>
        public List<Transaction> Transactions { get; set; }
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
            Transactions = new List<Transaction>();
        }

        public BankCard(string code, string pincode, ulong owner, decimal balance, decimal maxbalance, DateTime expireDate, bool isactive, bool isinatm, List<Transaction> transactions)
        {
            Id = code;
            PinCode = pincode;
            OwnerId = owner;
            Balance = balance;
            BalanceLimit = maxbalance;
            ExpireDate = expireDate;
            IsInATM = isinatm;
            IsActive = isactive;
            Transactions = transactions;
        }

        public BankCard() { }

        /// <summary>
        /// Function used to update the details of the card and make more maintanable
        /// </summary>
        /// <param name="details"></param>
        public void UpdateDetails(BankCardDetails details)
        {
            if (details.CardBalance >= 0)
                Balance = details.CardBalance;
            IsActive = details.IsActive;
            IsInATM = details.IsInATM;
            if (details.Transactions.Count >= 0 && details.Transactions != null)
                Transactions = details.Transactions;
        }
    }
}
