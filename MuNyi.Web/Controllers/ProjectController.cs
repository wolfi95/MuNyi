using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MuNyi.Bll.ServiceInterfaces;
using MuNyi.Dal.Entities.Authentication;
using MuNyi.Dto;
using MuNyi.Web.Helpers;
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
            if (String.IsNullOrEmpty(newProjectDto.ProjectName))
            {
                throw new ArgumentNullException("Projekt neve nem lehet üres!");
            }
            await projectService.CreateNewProjectAsync(newProjectDto, (await userManager.GetUserAsync(User)));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<ProjectDto>> GetAllProjects()
        {
            return await projectService.GetAllProjectsAsync();
        }

        [HttpGet]
        [Route("search")]
        [AllowAnonymous]
        public async Task<PagedData<ProjectDto>> SearchProjects(SearchProjectDto searchData)
        {
            if (String.IsNullOrEmpty(searchData.OrderBy))
            {
                searchData.OrderBy = "";
            }
            return new PagedData<ProjectDto>
            {
                Data = (await projectService.SearchProjectsAsync(searchData)),
                PageNumber = searchData.PageNumber,
                PageSize = searchData.PageSize
            };
        }

        [HttpDelete]
        [Route("{id}/delete")]
        public async Task DeleteProject([FromRoute] Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException("Id nem lehet üres");
            }
            await projectService.DeleteProject(id);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ProjectDetailDto> GetProjectDetails([FromRoute] Guid id)
        {
            return
        }
    }
}
