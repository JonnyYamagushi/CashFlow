using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.GetById;

public class GetExpenseByIdUseCase : IGetExpenseByIdUseCase
{
    private readonly IExpenseReadOnlyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggerUser;

    public GetExpenseByIdUseCase(IExpenseReadOnlyRepository repository, IMapper mapper, ILoggedUser loggerUser)
    {
        _repository = repository;
        _mapper = mapper;
        _loggerUser = loggerUser;
    }

    public async Task<ResponseExpenseJson> Execute(long id)
    {
        var loggedUser = await _loggerUser.Get();

        var result = await _repository.GetById(loggedUser, id);

        if (result is null)
        {
            throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
        }

        return _mapper.Map<ResponseExpenseJson>(result);
    }
}
