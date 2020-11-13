using System.Collections.Generic;
using System.Threading.Tasks;
using authService.Models;

namespace authService.Services.User
{
    public interface IUserService
    {
        Task<Token> Register(Data.User user);
        
        Task<Token> Authenticate(Authenticate authenticate);
        
        Task<Token> RefreshToken(RefreshToken refreshToken);
        
        Task<bool> RevokeRefreshToken(RevokeToken revokeToken);
        
        IEnumerable<Data.User> GetAll();
    }
}