using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;

namespace CashFlow.Application.UseCases.Expenses.GetAll;

public class GetAllExpensesUseCase : IGetAllExpensesUseCase
{
    private readonly IExpenseReadOnlyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggerUser;

    public GetAllExpensesUseCase(IExpenseReadOnlyRepository repository, IMapper mapper, ILoggedUser loggerUser)
    {
        _repository = repository;
        _mapper = mapper;
        _loggerUser = loggerUser;
    }

    public async Task<ResponseExpensesJson> Execute()
    {
        var loggedUser = await _loggerUser.Get();

        var result = await _repository.GetAll(loggedUser);

        return new ResponseExpensesJson
        {
            Expenses = _mapper.Map<List<ResponseShortExpenseJson>>(result)
        };
    }
}
