using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ElevenNote.Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> RegisterUserAsync(UserRegister model)
        {
            if (await GetUserEmailAsync(model.Email) != null || await GetUsernameAsync(model.Username) != null)
                return false;

            var entity = new UserEntity
            { 
                Email = model.Email,
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateCreated = DateTime.Now
            };

            var passwordHasher = new PasswordHasher<UserEntity>();
            entity.Password = passwordHasher.HashPassword(entity, model.Password);

            _context.Users.Add(entity);
            var numberOfChanges = await _context.SaveChangesAsync();

            return numberOfChanges == 1;
        }
        
        public async Task<UserDetail> GetUserByIdAsync(int userId)
        {
            var entity = await _context.Users.FindAsync(userId);
            if(entity is null)
            return null;
            
            var userDetail = new UserDetail
            {
                Id = entity.Id,
                Email = entity.Email,
                Username = entity.Username,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DateCreated = entity.DateCreated
            };
            return userDetail;
        }
        
    //Helper
        private async Task<UserEntity> GetUserEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower());
        }
        private async Task<UserEntity> GetUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
        }
        
    }
}