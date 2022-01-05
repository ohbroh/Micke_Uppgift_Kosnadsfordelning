/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *  LogisticsSE
 *  
 *  CREATED:
 *      2013-01-11
 *      Johan Skarström <johan.skarstrom@unit4.com>
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

// .NET
using System;
using System.Collections.Generic;
using System.Text;
// Agresso
using Agresso.Interface.CommonExtension;
using Agresso.ServerExtension;

namespace ACT.SE.Common
{
    /// <summary>Snygg text</summary>
    internal partial class CurrentContext : Agresso.Interface.CommonExtension.CurrentContext
    {
        /// <summary>Ger access till LogisticsSE genom CurrentContext</summary>
        internal class LogisticsSE
        {
            #region internal enum PurchaseOrderFlags : uint
            /// <summary>Enumeration over the Agresso purchase order flags.</summary>
            [Flags]
            internal enum PurchaseOrderFlags : uint
            {
                /// <summary>FULL_MATCH, Order and invoice matches, invoice less than order, or invoice less than EI_MIN_APPR_AMT</summary>
                FullMatch = 1,

                /// <summary>MISSING_GOODS_RECEIVED, Goods are missing. Summary:no goods have been received</summary>
                MissingGoodsReceived = 2,

                /// <summary>CREDIT_NOTE, It is a credit invoice</summary>
                CreditNote = 4,

                /// <summary>LESS_THAN_MIN_APPR_AMT, The invoice amount is less than the EI_MIN_APPR_AMT</summary>
                LessThanMinapprovalAmount = 8,

                /// <summary>EXCEEDS_MAX_OVERRUN_AMT, The difference between the order and the invoice is less than invoice control (overrun percentage), but more than the value of EI_MAX_OVERRUN_AMOUNT</summary>
                ExceedsMaxOverrunAmount = 16,

                /// <summary>EXCEEDS_MAX_POST_AMT, Full match, but the invoice is greater than the value of EI_MAX_POST_AMOUNT</summary>
                ExceedsMaxPostAmount = 32,

                /// <summary>EXTRA_INVOICELINES, Extra detail line on the invoice (negative LineNo on the xml-invoice)</summary>
                ExtraInvoiceLines = 64,

                /// <summary>DISCREPANCY, Summary invoice with flag 8, 16 or 32</summary>
                Discrepancy = 128,

                /// <summary>LESS_THAN_MIN_OVERRUN_AMT, The invoice exceeds the invoice control, but the discrepancy is less than the value of EI_MIN_OVERRUN_AMT and should be ready for posting.</summary>
                LessThanMinOverrunAmount = 256,

                /// <summary>MISMATCH, There is a mismatch between the order and the invoice but it does not exceeds the invoice control</summary>
                Mismatch = 512,

                /// <summary>EXCEEDS_OVERRUNPCT_QUANTITY_DELIVERED, The difference between the ordered quantity and the invoice quantity is greater than invoice control (overrun percentage)</summary>
                ExceedsOverrunPercentQuantityDelivered = 1024,

                /// <summary>EXCEEDS_OVERRUNPCT_AMOUNT_DELIVERED, The difference between the delivered amount and the invoice amount is greater than invoice control (overrun percentage)</summary>
                ExceedsOverrunPercentAmountDelivered = 2048,

                /// <summary>EXCEEDS_OVERRUNPCT_AMOUNT_ORDERED, The difference between the order amount and the invoice amount is greater than invoice control (overrun percentage)</summary>
                ExceedsOverrunPercentAmountOrdered = 4096,

                /// <summary>EXTRA_ORDERLINES, The invoice should be forced on workflow independent of other circumstances. E.g. apodetail flag E</summary>
                ExtraOrderLines = 8192,

                /// <summary>EXCEEDS_POST_CREDIT_AMOUNT, Force credit notes on workflow if invoice exceeds the parameter value</summary>
                ExceedsPostCreditAmount = 16384
            }
            #endregion
        }
    }
}
