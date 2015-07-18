using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hyper.NACHA;

namespace NACHASampleProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var companyID = "9999999999";
            string companyName = "Heitz, Inc.";
            string batchDescription = "Paychecks";

            NACHAFile file = new NACHAFile();

            file.ImmediateDestination = "111111118"; // Routing number of destination bank.
            file.ImmediateDestinationName = "Heitz Test Bank"; // Name of the receiving bank.
            file.ImmediateOrigin = companyID; // Originator's ACH Company ID
            file.ImmediateOriginName = companyName; // Name of the originating company.

            var batch = new NACHACompanyBatch(1, new DateTime(2014, 8, 21))
            {
                CompanyName = companyName,
                CompanyID = companyID,
                BatchDescription = batchDescription,
                OriginatingRoutingNumber = "111111118"
            };

            var transactions = new List<Transaction>() {
                new Transaction()
                {
                UniqueIndividualID=123456789,
                IndividualName="John Doe",
                IndividualRoutingNumber="111111118",
                IndividualAccountNumber="987654321",
                BankAccountType=1, // 1=Checking, 2=Savings
                Amount=123.04M
                }
            };

            long tranCount = 0;
            foreach (var transaction in transactions)
            {
                transaction.Amount = -transaction.Amount;

                var transactionCode = NACHATransactionCodeType.CreditChecking;
                if (transaction.BankAccountType == 1)
                {
                    if (transaction.Amount >= 0)
                    {
                        transactionCode = NACHATransactionCodeType.CreditChecking;
                    }
                    else
                    {
                        transactionCode = NACHATransactionCodeType.DebitChecking;
                    }
                }
                else if (transaction.BankAccountType == 2)
                {
                    if (transaction.Amount >= 0)
                    {
                        transactionCode = NACHATransactionCodeType.CreditSavings;
                    }
                    else
                    {
                        transactionCode = NACHATransactionCodeType.DebitSavings;
                    }
                }

                batch.AddEntry(new NACHAEntryDetailRecord(transactionCode)
                {
                    Amount = transaction.Amount,
                    DFIAccountNumber = transaction.IndividualAccountNumber,
                    IndividualIdentificationNumber = transaction.UniqueIndividualID.ToString(),
                    ReceiverName = transaction.IndividualName,
                    ReceiverRoutingNumber = transaction.IndividualRoutingNumber,
                    TraceNumber = long.Parse("111111118".PadLeft(9, '0').Substring(0, 8) + tranCount.ToString("0000000")) // Per NACHA docs, must be first 8 digits of originating bank's routing number, plus a unique number
                });

                tranCount++;
            }

            file.AddCompanyBatch(batch);

            using (FileStream fs = new FileStream(@"NACHATest.txt", FileMode.Create))
            {
                string fileContents = file.ToString();
                byte[] nachaBytes = Encoding.UTF8.GetBytes(fileContents);
                fs.Write(nachaBytes, 0, nachaBytes.Length);
                fs.Flush();
            }
        }
    }

    public class Transaction
    {
        public int UniqueIndividualID { get; set; }
        public string IndividualName { get; set; }
        public string IndividualAccountNumber { get; set; }
        public string IndividualRoutingNumber { get; set; }
        public int BankAccountType { get; set; }
        public decimal Amount { get; set; }
    }
}
