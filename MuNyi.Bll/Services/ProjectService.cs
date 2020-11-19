using MuNyi.Bll.ServiceInterfaces;
using MuNyi.Dal;
using MuNyi.Dal.Dtos;
using MuNyi.Dal.Entities;
using MuNyi.Dal.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MuNyi.Bll.Services
{
    public class ProjectService : IProjectService
    {
        private readonly MuNyiContext context;

        public ProjectService(MuNyiContext context)
        {
            this.context = context;
        }
        public async System.Threading.Tasks.Task CreateNewProjectAsync(NewProjectDto newProject, User user)
        {
            context.Projects.Add(new Project
            {
                Name = newProject.ProjectName,
                CreatedBy = user,
                CreatedTime = DateTime.Now,
                Description = newProject.ProjectDescription
            });
            await context.SaveChangesAsync();
        }
    }
}
