using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses;

public interface IExpenseWriteOnlyRepository
{
    Task Add(Expense expense);
    /// <summary>
    /// This function returns true if the expense was deleted successfully, otherwise false.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Delete(long id);
}
