using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess.Repositories;

internal class ExpensesRepository : IExpenseReadOnlyRepository, IExpenseWriteOnlyRepository, IExpensesUpdateOnlyRepository
{
    private readonly CashFlowDbContext _dbContext;

    public ExpensesRepository(CashFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Expense expense)
    {
        await _dbContext.Expenses.AddAsync(expense);
    }

    public async Task<bool> Delete(long id)
    {
        var expense = await _dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id);

        if (expense is null)
        {
            return false;
        }
        _dbContext.Expenses.Remove(expense);
        return true;
    }

    public async Task<List<Expense>> GetAll()
    {
        return await _dbContext.Expenses.AsNoTracking().ToListAsync();
    }

    async Task<Expense?> IExpenseReadOnlyRepository.GetByIdAsync(long id)
    {
        return await _dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(expense => expense.Id == id);
    }

    async Task<Expense?> IExpensesUpdateOnlyRepository.GetByIdAsync(long id)
    {
        return await _dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    public async Task<List<Expense>> GetByMonth(DateOnly month)
    {
        //var startDate = new DateTime(month.Year, month.Month, 1).Date;
        //var endDate = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month), hour: 23, minute: 59, second:59).Date;

        //return _dbContext.Expenses.AsNoTracking().Where(expense => expense.Date >= startDate && expense.Date <= endDate).OrderBy(expense => expense.Date).ThenBy(expense => expense.Title).ToListAsync();

        return await _dbContext.Expenses.AsNoTracking().Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month).OrderBy(e => e.Date).ThenBy(e => e.Title).ToListAsync();
    }
}
