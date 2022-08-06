using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ElevenNote.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public TokenService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<TokenResponse> GetTokenAsync(TokenRequest model)
        {
            var userEntity = await GetValidUserAsync(model);
            if(userEntity is null)
            return null;

            return GenerateToken(userEntity);

        }
        private async Task<UserEntity> GetValidUserAsync(TokenRequest model)
        {
            var userEntity = await _context.Users.FirstOrDefaultAsync(userEntity => userEntity.Username.ToLower() == model.Username.ToLower());
            if(userEntity is null)
            return null;

            var passwordHasher = new PasswordHasher<UserEntity>();
            var verifyPasswordResult = passwordHasher.VerifyHashedPassword(userEntity, userEntity.Password, model.Password);
            if(verifyPasswordResult == PasswordVerificationResult.Failed)
            return null;

            return userEntity;
        }
        private TokenResponse GenerateToken(UserEntity entity)
        {
            var claims = GetClaims(entity);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenResponse = new TokenResponse
            {
                Token = tokenHandler.WriteToken(token),
                IssuedAt = token.ValidFrom,
                Expires = token.ValidTo
            };

            return tokenResponse;
        }
        private Claim[] GetClaims(UserEntity user)
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            var name = !string.IsNullOrWhiteSpace(fullName) ? fullName : user.Username;
            var claims = new Claim[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Username", user.Username),
                new Claim("Email", user.Email),
                new Claim("Name", name)
            };
            return claims;
        }
    }
}