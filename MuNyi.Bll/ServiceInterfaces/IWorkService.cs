using MuNyi.Dal.Entities.Authentication;
using MuNyi.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MuNyi.Bll.ServiceInterfaces
{
    public interface IWorkService
    {
        public Task<IEnumerable<WorkDto>> GetTaskWorkItemsAsync(Guid projectId,Guid id);
        public Task<IEnumerable<WorkDto>> SearchTaskWorkItemsAsync(Guid projectId,Guid id, SearchWorkItemDto searchData);
        public Task CreateNewWorkItem(Guid projectId, Guid id, NewWorkItemDto newWorkItemData, User user);
        public Task DeleteWorkItem(Guid projectId, Guid id,Guid workItemId, User user);
    }
}
