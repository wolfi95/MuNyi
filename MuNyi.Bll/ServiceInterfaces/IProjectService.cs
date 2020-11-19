using MuNyi.Dal.Entities.Authentication;
using MuNyi.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MuNyi.Bll.ServiceInterfaces
{
    public interface IProjectService
    {
        public Task CreateNewProjectAsync(NewProjectDto newProject, User user);
        public Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        public Task<IEnumerable<ProjectDto>> SearchProjectsAsync(SearchProjectDto searchData);
    }
}
