using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Register;

public class RegisterExpenseUseCase : IRegisterExpenseUseCase
{
    private readonly IExpenseWriteOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggerUser;

    public RegisterExpenseUseCase(IExpenseWriteOnlyRepository repository, IUnitOfWork unitOfWork, IMapper mapper, ILoggedUser loggerUser)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggerUser = loggerUser;
    }

    public async Task<ResponseRegisterExpenseJson> Execute(RequestExpenseJson request)
    {
        Validate(request);

        var loggedUser = await _loggerUser.Get();

        var expenseEntity = _mapper.Map<Expense>(request);
        expenseEntity.UserId = loggedUser.Id;

        await _repository.Add(expenseEntity);

        await _unitOfWork.Commit();

        return _mapper.Map<ResponseRegisterExpenseJson>(expenseEntity);
    }

    private void Validate(RequestExpenseJson request)
    {
        var result = new ExpenseValidator().Validate(request);

        if (result.IsValid == false)
        {
            throw new ErrorOnValidationException(result.Errors.Select(x => x.ErrorMessage).ToList());
        }
    }
}
