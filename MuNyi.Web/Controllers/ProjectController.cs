using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MuNyi.Bll.ServiceInterfaces;
using MuNyi.Dal.Dtos;
using MuNyi.Dal.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MuNyi.Web.Controllers
{
    [Route("project")]
    [ApiController]
    [Authorize(UserRoles.Administrator)]
    public class ProjectController : Controller
    {
        private readonly IProjectService projectService;
        private readonly UserManager<User> userManager;

        public ProjectController(IProjectService projectService, UserManager<User> userManager)
        {
            this.projectService = projectService;
            this.userManager = userManager;
        }
        [HttpPost]
        public async Task NewProject(NewProjectDto newProjectDto)
        {
            await projectService.CreateNewProjectAsync(newProjectDto, (await userManager.GetUserAsync(User)));
        }
    }
}
