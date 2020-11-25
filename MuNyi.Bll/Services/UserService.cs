using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MuNyi.Bll.ServiceInterfaces;
using MuNyi.Dal;
using MuNyi.Dal.Entities.Authentication;
using MuNyi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuNyi.Bll.Services
{
    public class UserService : IUserService
    {
        private readonly MuNyiContext context;
        private readonly UserManager<User> userManager;

        public UserService(MuNyiContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await context.Users.ToListAsync();
            var userDtos = new List<UserDto>();
            foreach(var user in users)
            {
                userDtos.Add(new UserDto { Id = user.Id, Name = user.Name, Role = (await userManager.IsInRoleAsync(user, UserRoles.Administrator) ? "Admin" : "User") });
            }
            return userDtos;
        }
    }
}
