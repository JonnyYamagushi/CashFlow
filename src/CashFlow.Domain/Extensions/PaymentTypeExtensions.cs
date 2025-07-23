using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;

namespace CashFlow.Domain.Extensions;

public static class PaymentTypeExtensions
{
    public static string PaymentTypeToString(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => ResourceReportGenerationMessages.Cash,
            PaymentType.CreditCard => ResourceReportGenerationMessages.CreditCard,
            PaymentType.DebitCard => ResourceReportGenerationMessages.DebitCard,
            PaymentType.BankTransfer => ResourceReportGenerationMessages.BankTransfer,
            PaymentType.Pix => ResourceReportGenerationMessages.PIX,
            _ => string.Empty
        };
    }
}
