using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using ClosedXML.Excel;

namespace CashFlow.Application.UseCases.Expenses.Reports.Excel;

internal class GenerateExpensesReportExcelUseCase : IGenerateExpensesReportExcelUseCase
{
    private const string CURRENCY_SYMBOL = "R$";

    private readonly IExpenseReadOnlyRepository _repository;

    public GenerateExpensesReportExcelUseCase(IExpenseReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<byte[]> Execute(DateOnly month)
    {
        var expenses = await _repository.GetByMonth(month);

        if (expenses.Count == 0)
        {
            return [];
        }

        using var workbook = new XLWorkbook();

        workbook.Author = "CashFlow";
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Times New Roman";

        var workSheet = workbook.Worksheets.Add(month.ToString("Y"));

        InsertHeader(workSheet);

        int row = 2;
        foreach (var expense in expenses)
        {
            workSheet.Cell($"A{row}").Value = expense.Title;
            workSheet.Cell($"B{row}").Value = expense.Date;
            workSheet.Cell($"C{row}").Value = ConvertPaymentType(expense.PaymentType);
            workSheet.Cell($"D{row}").Style.NumberFormat.Format = $"-{CURRENCY_SYMBOL} #,##0.00";
            workSheet.Cell($"D{row}").Value = expense.Amount;
            workSheet.Cell($"E{row}").Value = expense.Description;

            row++;
        }

        workSheet.Columns().AdjustToContents();

        var file = new MemoryStream();

        workbook.SaveAs(file);

        return file.ToArray();
    }

    private string ConvertPaymentType(PaymentType paymentType)
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

    private void InsertHeader(IXLWorksheet workSheet)
    {
        workSheet.Cell("A1").Value = ResourceReportGenerationMessages.TITLE;
        workSheet.Cell("B1").Value = ResourceReportGenerationMessages.DATE;
        workSheet.Cell("C1").Value = ResourceReportGenerationMessages.PAYMENT_TYPE;
        workSheet.Cell("D1").Value = ResourceReportGenerationMessages.AMOUNT;
        workSheet.Cell("E1").Value = ResourceReportGenerationMessages.DESCRIPTION;

        workSheet.Cells("A1:E1").Style.Font.Bold = true;
        workSheet.Cells("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#F5C2B6");

        workSheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        workSheet.Cell("B1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        workSheet.Cell("C1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        workSheet.Cell("D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        workSheet.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }
}
