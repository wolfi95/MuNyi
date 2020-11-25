using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MuNyi.Bll.ServiceInterfaces;
using MuNyi.Dal.Entities.Authentication;
using MuNyi.Dto;
using MuNyi.Web.Authentication;

namespace MuNyi.Web.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IConfiguration configuration;
        private readonly IUserService userController;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IUserService userController)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.userController = userController;
        }

        [HttpGet]       
        [Authorize(Roles = UserRoles.Administrator)]
        public async Task<List<UserDto>> GetAllUsers()
        {
            return await userController.GetAllUsers();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDataResponse>> Login([FromBody] UserLoginDto loginData)
        {
            if (loginData.Email == null || loginData.Password == null)
            {
                return new UnauthorizedObjectResult("Email and password cannot be empty");
            }
            var user = await userManager.FindByEmailAsync(loginData.Email);
            if (user == null)
            {
                return new UnauthorizedObjectResult("Cannot find user");
            }

            var result = await signInManager.PasswordSignInAsync(loginData.Email, loginData.Password, false, false);
            if (result.Succeeded)
            {
                var token = AuthenticationHelper.GenerateJwtToken(user, configuration);
                var res = new UserDataResponse
                {
                    UserId = user.Id,
                    Token = token,
                    Name = user.Name
                };
                return new OkObjectResult(res);
            }
            else
            {
                return new UnauthorizedObjectResult("Wrong password");
            }
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto registerData)
        {
            if (String.IsNullOrEmpty(registerData.Email) ||
                String.IsNullOrEmpty(registerData.Password) ||
                String.IsNullOrEmpty(registerData.EmailRe) ||
                String.IsNullOrEmpty(registerData.PasswordRe))
            {
                return new UnauthorizedObjectResult("Email and password cannot be empty.");
            }

            if (String.IsNullOrEmpty(registerData.Name))
            {
                return new UnauthorizedObjectResult("Name cannot be empty.");
            }

            if (registerData.Email != registerData.EmailRe)
            {
                return new BadRequestObjectResult("Email and confirmation doesnt match.");
            }
            if (registerData.Password != registerData.PasswordRe)
            {
                return new BadRequestObjectResult("Password and confirmation doesnt match.");
            }

            if ((await userManager.FindByEmailAsync(registerData.Email)) != null)
            {
                return new BadRequestObjectResult("Email already in use.");
            }

            var newUser = new User
            {
                Email = registerData.Email,
                TwoFactorEnabled = false,
                UserName = registerData.Email,
                Name = registerData.Name,
            };

            var result = await userManager.CreateAsync(newUser, registerData.Password);

            if (!result.Succeeded)
            {
                var msg = "";
                foreach (var err in result.Errors)
                {
                    msg += err.Description + Environment.NewLine;
                }
                return new BadRequestObjectResult(msg);
            }

            return new OkResult();

        }
    }
}
