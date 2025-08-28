using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Security.Tokens;
public interface IAccessTokenGerator
{
    string Generate(User user);
}
