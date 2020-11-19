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
    [Route("{projectId}/task")]
    [ApiController]
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly UserManager<User> userManager;

        public TaskController(ITaskService taskService, UserManager<User> userManager)
        {
            this.taskService = taskService;
            this.userManager = userManager;            
        }

        [HttpPost]
        public async Task NewTask([FromRoute] Guid projectId, NewTaskDto newTaskData)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }

            if (String.IsNullOrEmpty(newTaskData.Name))
            {
                throw new ArgumentNullException("Feladat neve nem lehet üres!");
            }

            await taskService.CreateNewTaskAsync(projectId, newTaskData, (await userManager.GetUserAsync(User)));
        }

        [HttpGet]
        public async Task<IEnumerable<TaskDto>> GetAllTasks([FromRoute] Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }

            return await taskService.GetAllTasksAsync(projectId);
        }

        [HttpGet]
        [Route("search")]
        public async Task<PagedData<TaskDto>> SearchTasks([FromRoute] Guid projectId, SearchTaskDto searchData)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }

            if (String.IsNullOrEmpty(searchData.OrderBy))
            {
                searchData.OrderBy = "";
            }
            return new PagedData<TaskDto>
            {
                Data = (await taskService.SearchTasksAsync(projectId, searchData)),
                PageNumber = searchData.PageNumber,
                PageSize = searchData.PageSize
            };
        }

        [HttpDelete]
        [Route("{id}/delete")]
        [Authorize(Roles = UserRoles.Administrator)]
        public async Task DeleteProject([FromRoute]Guid projectId, [FromRoute] Guid id)
        {

            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("Id nem lehet üres");
            }

            await taskService.DeleteTask(projectId, id);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<TaskDetailDto> GetTaskDetails([FromRoute] Guid projectId, [FromRoute] Guid id)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("A feladat azonosítója nem lehet üres.");
            }
            return await taskService.GetTaskByIdAsync(projectId, id);
        }

        [HttpGet]
        [Route("{id}/work")]
        public async Task<double> GetTaskWorkTime([FromRoute] Guid projectId, [FromRoute] Guid id)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("A deladat azonosítója nem lehet üres.");
            }
            return await taskService.GetTaskWorkAsync(projectId, id);
        }
    }
}
