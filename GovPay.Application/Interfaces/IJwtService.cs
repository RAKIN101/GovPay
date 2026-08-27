using GovPay.Domain.Entities;

namespace GovPay.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}