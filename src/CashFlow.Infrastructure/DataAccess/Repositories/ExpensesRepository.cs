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

    public async Task Delete(long id)
    {
        var expense = await _dbContext.Expenses.FindAsync(id);

        _dbContext.Expenses.Remove(expense!);
    }

    public async Task<List<Expense>> GetAll(User user)
    {
        return await _dbContext.Expenses.AsNoTracking().Where(expense => expense.UserId == user.Id).ToListAsync();
    }

    async Task<Expense?> IExpenseReadOnlyRepository.GetById(User user, long id)
    {
        return await _dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(expense => expense.Id == id && expense.UserId == user.Id);
    }

    async Task<Expense?> IExpensesUpdateOnlyRepository.GetById(User user, long id)
    {
        return await _dbContext.Expenses.FirstOrDefaultAsync(expense => expense.Id == id && expense.UserId == user.Id);
    }

    public void Update(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
    }

    public async Task<List<Expense>> GetByMonth(User user, DateOnly month)
    {
        //var startDate = new DateTime(month.Year, month.Month, 1).Date;
        //var endDate = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month), hour: 23, minute: 59, second:59).Date;

        //return _dbContext.Expenses.AsNoTracking().Where(expense => expense.Date >= startDate && expense.Date <= endDate).OrderBy(expense => expense.Date).ThenBy(expense => expense.Title).ToListAsync();

        return await _dbContext.Expenses.AsNoTracking().Where(e => e.UserId == user.Id && e.Date.Year == month.Year && e.Date.Month == month.Month).OrderBy(e => e.Date).ThenBy(e => e.Title).ToListAsync();
    }
}
