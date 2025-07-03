using Bogus;
using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterExpenseJsonBuilder
{
    public static RequestExpenseJson Build()
    {        
        return new Faker<RequestExpenseJson>()
            .RuleFor(r => r.Title, f => f.Commerce.ProductName())
            .RuleFor(r => r.Date, f => f.Date.Past())
            .RuleFor(r => r.Amount, f => f.Finance.Amount(decimals: 2))
            .RuleFor(r => r.Description, f => f.Lorem.Sentence())
            .RuleFor(r => r.PaymentType, f => f.PickRandom<PaymentType>());
    }
}
