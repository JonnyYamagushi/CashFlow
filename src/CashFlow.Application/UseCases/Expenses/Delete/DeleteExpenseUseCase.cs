using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Delete;

public class DeleteExpenseUseCase : IDeleteExpenseUseCase
{
    private readonly IExpenseReadOnlyRepository _expenseReadOnlyRepository;
    private readonly IExpenseWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggerUser;

    public DeleteExpenseUseCase(IExpenseWriteOnlyRepository repository, IExpenseReadOnlyRepository expenseReadOnlyRepository, IUnitOfWork unitOfWork, ILoggedUser loggerUser)
    {
        _repository = repository;
        _expenseReadOnlyRepository = expenseReadOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggerUser = loggerUser;
    }

    public async Task Execute(long id)
    {
        var loggedUser = await _loggerUser.Get();

        var expense = await _expenseReadOnlyRepository.GetById(loggedUser, id);

        if (expense is null)
        {
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
        }

        await _repository.Delete(id);

        await _unitOfWork.Commit();
    }
}
