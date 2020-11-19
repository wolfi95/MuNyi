using MuNyi.Dal.Entities.Authentication;
using MuNyi.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MuNyi.Bll.ServiceInterfaces
{
    public interface ITaskService
    {
        public Task CreateNewTaskAsync(Guid projectId, NewTaskDto newTask, User user);
        public Task<IEnumerable<TaskDto>> GetAllTasksAsync(Guid projectId);
        public Task<IEnumerable<TaskDto>> SearchTasksAsync(Guid projectId, SearchTaskDto searchData);
        public Task DeleteTask(Guid projectId, Guid Id);
        public Task<TaskDetailDto> GetTaskByIdAsync(Guid projectId, Guid id);
        public Task<double> GetTaskWorkAsync(Guid projectId, Guid id);
    }
}
