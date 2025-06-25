using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Expenses.Register;

public class RegisterExpenseUseCase
{
    public ResponseRegisterExpenseJson Execute(RequestRegisterExpenseJson request)
    {
        Validate(request);

        return new ResponseRegisterExpenseJson
        {
            Title = request.Title
        };
    }

    private void Validate(RequestRegisterExpenseJson request)
    {
        if (string.IsNullOrEmpty(request.Title.Trim()))
        {
            throw new ArgumentException("Title cannot be empty.");
        }
        if (request.Amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.");
        }
        if (DateTime.Compare(request.Date, DateTime.UtcNow) > 0)
        {
            throw new ArgumentException("Expenses cannot be for the future.");
        }
        if (Enum.IsDefined(typeof(PaymentType), request.PaymentType) == false)
        {
            throw new ArgumentException("Payment type is not valid.");
        }
    }
}
