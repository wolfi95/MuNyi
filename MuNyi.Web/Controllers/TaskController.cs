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
using System.Security.Claims;
using System.Threading.Tasks;

namespace MuNyi.Web.Controllers
{
    [Route("{projectId}/task")]
    [ApiController]
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;
        private readonly IWorkService workService;
        private readonly UserManager<User> userManager;

        public TaskController(ITaskService taskService, UserManager<User> userManager, IWorkService workService)
        {
            this.workService = workService;
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
            var user = await userManager.FindByEmailAsync(User.Claims.First(x => x.Type == ClaimTypes.Email).Value);            

            await taskService.CreateNewTaskAsync(projectId, newTaskData, user);
        }

        [HttpPut]
        [Route("{taskId}")]
        public async Task UpdateTask([FromRoute] Guid taskId, [FromRoute] Guid projectId, NewTaskDto newTaskData)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }

            if (String.IsNullOrEmpty(newTaskData.Name))
            {
                throw new ArgumentNullException("Feladat neve nem lehet üres!");
            }
            var user = await userManager.FindByEmailAsync(User.Claims.First(x => x.Type == ClaimTypes.Email).Value);

            await taskService.UpdateTaskAsync(projectId, taskId, newTaskData, user);
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
        public async Task DeleteTask([FromRoute]Guid projectId, [FromRoute] Guid id)
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
                throw new ArgumentNullException("A feladat azonosítója nem lehet üres.");
            }
            return await taskService.GetTaskWorkAsync(projectId, id);
        }

        [HttpGet]
        [Route("{id}/workItems")]
        public async Task<IEnumerable<WorkDto>> GetTaskWorkItems([FromRoute] Guid projectId, [FromRoute] Guid id)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("A feladat azonosítója nem lehet üres.");
            }

            return await workService.GetTaskWorkItemsAsync(projectId, id);
        }

        [HttpGet]
        [Route("{id}/workItems/search")]
        public async Task<PagedData<WorkDto>> SearchTaskWorkItems([FromRoute] Guid projectId, [FromRoute] Guid id, SearchWorkItemDto searchData)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("A feladat azonosítója nem lehet üres.");
            }

            return new PagedData<WorkDto>{
                Data = (await workService.SearchTaskWorkItemsAsync(projectId, id, searchData)),
                PageNumber = searchData.PageNumber,
                PageSize = searchData.PageSize,
            };
        }

        [HttpPost]
        [Route("{id}/workItems/new")]
        public async Task CreateNewWorkItem([FromRoute] Guid projectId, [FromRoute] Guid id, NewWorkItemDto newWorkItemData)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("A feladat azonosítója nem lehet üres.");
            }

            var user = await userManager.FindByEmailAsync(User.Claims.First(x => x.Type == ClaimTypes.Email).Value);            

            await workService.CreateNewWorkItem(projectId, id, newWorkItemData, user);
        }

        [HttpDelete]
        [Route("{id}/workItems/{workId}")]
        public async Task DeleteWorkItem([FromRoute] Guid projectId, [FromRoute] Guid id, [FromRoute]Guid workId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException("Projekt azonosító nem lehet üres");
            }
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("A feladat azonosítója nem lehet üres.");
            }
            if (workId == Guid.Empty)
            {
                throw new ArgumentNullException("A munkaidő azonosítója nem lehet üres.");
            }

            var user = await userManager.FindByEmailAsync(User.Claims.First(x => x.Type == ClaimTypes.Email).Value);

            await workService.DeleteWorkItem(projectId, id, workId, user);
        }
    }
}
