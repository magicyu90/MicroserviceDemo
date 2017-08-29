using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.API.Database.Entities;

namespace User.API.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserByIdAsync(int id);
        Task DeleteUserAysncAsync(int id);
    }
}
