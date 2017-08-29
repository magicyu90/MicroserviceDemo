using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Database;
using User.API.Database.Entities;

namespace User.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EfContext _context;

        public UserRepository(EfContext context)
        {
            _context = context;
        }


        public async Task<UserEntity> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task DeleteUserAysncAsync(int id)
        {
            var userToDelete = await GetUserByIdAsync(id);

            _context.Users.Remove(userToDelete);

            await _context.SaveChangesAsync();
        }
    }
}
