using CashFlow.Domain.Repositories.Expenses;
using Moq;

namespace CommonTestUtilities.Repositories;

public class ExpensesWriteOnlyRepositoryBuilder
{
    public static IExpenseWriteOnlyRepository Build()
    {
        var mock = new Mock<IExpenseWriteOnlyRepository>();

        return mock.Object;
    }
}
