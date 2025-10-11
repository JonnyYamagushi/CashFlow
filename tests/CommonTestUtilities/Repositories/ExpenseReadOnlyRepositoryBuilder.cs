using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using Moq;

namespace CommonTestUtilities.Repositories;

public class ExpenseReadOnlyRepositoryBuilder
{
    private readonly Mock<IExpenseReadOnlyRepository> _repository;

    public ExpenseReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IExpenseReadOnlyRepository>();
    }

    public ExpenseReadOnlyRepositoryBuilder GetAll(User user, List<Expense> expenses)
    {
        _repository.Setup(repository => repository.GetAll(user)).ReturnsAsync(expenses);

        return this;
    }

    public ExpenseReadOnlyRepositoryBuilder GetById(User user, Expense? expense)
    {
        if (expense is not null)
        {
            _repository.Setup(repository => repository.GetById(user, expense.Id)).ReturnsAsync(expense);
            return this;
        }

        return this;
    }

    public ExpenseReadOnlyRepositoryBuilder GetByMonth(User user, List<Expense> expenses)
    {
        _repository.Setup(repository => repository.GetByMonth(user, It.IsAny<DateOnly>())).ReturnsAsync(expenses);

        return this;
    }

    public IExpenseReadOnlyRepository Build() => _repository.Object;
}
