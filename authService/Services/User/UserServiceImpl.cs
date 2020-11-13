using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using authService.Data;
using authService.Helpers;
using authService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace authService.Services.User
{
   
    public class UserServiceImpl : IUserService
    {
        private readonly AuthServiceDbContext _context;
        private readonly IConfiguration _configuration;

        public UserServiceImpl(AuthServiceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        public async Task<Token> Register(Data.User user)
        {
            // GENERATED RANDOM USER ID
            var random = new Random();
            Guid g = Guid.NewGuid();
            //user.Id = random.Next();
            user.Id = Guid.NewGuid();
                 
            var token = GetToken(user);
                
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
            return token;
        }

        public async Task<Token> Authenticate(Authenticate authenticate)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == authenticate.Username && x.Password == authenticate.Password);
            if (user == null) return null;
            
            var token = GetToken(user); 

            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<Token> RefreshToken(RefreshToken refresh)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == refresh.refreshToken);
            if (user == null || !(user.RefreshTokenEndDate > DateTime.Now)) return null;
            
            var token = GetToken(user); 
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<bool> RevokeRefreshToken(RevokeToken revokeToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == revokeToken.refreshToken);
            if (user == null || !(user.RefreshTokenEndDate > DateTime.Now)) return false;
            user.RefreshToken = null;
            user.RefreshTokenEndDate = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync(); 
            return true;

        }

        public IEnumerable<Data.User> GetAll()
        { 
            return _context.Users;
        }

        private Models.Token GetToken(Data.User user)
        {
            // Generating token
            var tokenHandler = new TokenHelper(_configuration);
            var token = tokenHandler.CreateAccessToken(user);
            
            user.RefreshToken = token.RefreshToken;
            user.RefreshTokenEndDate = token.Expiration.AddMinutes(3);
            
            token.Id = user.Id;
            token.Name = user.Name;
            token.Surname = user.Surname;
            token.Username = user.Username;
            
            return token;
        }
    }
}