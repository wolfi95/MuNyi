using MuNyi.Dal.Dtos;
using MuNyi.Dal.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MuNyi.Bll.ServiceInterfaces
{
    public interface IProjectService
    {
        public Task CreateNewProjectAsync(NewProjectDto newProject, User user);
    }
}
