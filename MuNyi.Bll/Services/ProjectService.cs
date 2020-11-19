using Microsoft.EntityFrameworkCore;
using MuNyi.Bll.ServiceInterfaces;
using MuNyi.Dal;
using MuNyi.Dal.Entities;
using MuNyi.Dal.Entities.Authentication;
using MuNyi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if((await context.Projects.FirstOrDefaultAsync(x => x.Name == newProject.ProjectName)) != null)
            {
                throw new ArgumentException("Már létezik projekt ezzel a névvel!");
            }

            context.Projects.Add(new Project
            {
                Name = newProject.ProjectName,
                CreatedBy = user,
                CreatedTime = DateTime.Now,
                Description = newProject.ProjectDescription
            });
            await context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteProject(Guid Id)
        {
            var proj = context.Projects.Include(x => x.Tasks.Select(y => y.WorkItems)).FirstOrDefault(x => x.Id == Id);

            if(proj == null)
            {
                throw new ArgumentException("Nem található projekt ezzel az azonosítóval.");
            }

            using(var transation = context.Database.BeginTransaction())
            {
                foreach(var task in proj.Tasks)
                {
                    context.WorkItems.RemoveRange(task.WorkItems);                    
                }
                context.Tasks.RemoveRange(proj.Tasks);
                context.Remove(proj);

                await context.SaveChangesAsync();
                transation.Commit();
            }
            
            context.Projects.Remove(proj);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            return await context.Projects.Include(x => x.CreatedBy).Select(x => new ProjectDto
            {
                Id = x.Id,
                CreatedBy = x.CreatedBy.Name,
                Name = x.Name,
                CreatedTime = x.CreatedTime,
                Description = x.Description
            }).ToListAsync();
        }

        public async Task<ProjectDetailDto> GetProjectByIdAsync(Guid id)
        {
            var project = await context.Projects.Include(x => x.Tasks.Select(y => y.WorkItems)).Include(x => x.CreatedBy).FirstOrDefaultAsync(x => x.Id == id);
            if(project == null)
            {
                throw new ArgumentException("Nem található projekt.");
            }

            double loggedHours = 0;
            foreach(var task in project.Tasks)
            {
                loggedHours += task.WorkItems.Sum(x => x.Time);
            }

            return new ProjectDetailDto
            {
                CreatedBy = new UserDto { Id = project.CreatedBy.Id, Name = project.CreatedBy.Name },
                Name = project.Name,
                Id = project.Id,
                CreatedTime = project.CreatedTime,
                Description = project.Description,
                Tasks = project.Tasks
                            .Select(y => new TaskDto
                            {
                                Description = y.Description,
                                Id = y.Id,
                                CreatedBy = y.CreatedBy.Name,
                                Name = y.Name,
                                CreatedDate = y.CreatedDate
                            }
                            ),
                LoggedHours = loggedHours
            };
        }

        public async Task<IEnumerable<ProjectDto>> SearchProjectsAsync(SearchProjectDto searchData)
        {
            var projectsQuery = context.Projects.Include(x => x.CreatedBy).AsQueryable();

            if (!String.IsNullOrEmpty(searchData.ProjectName))
            {
                projectsQuery = projectsQuery.Where(x => x.Name.Contains(searchData.ProjectName));
            }
            if (!String.IsNullOrEmpty(searchData.ProjectDescription))
            {
                projectsQuery = projectsQuery.Where(x => x.Description.Contains(searchData.ProjectDescription));
            }
            if (searchData.FromDate != null)
            {
                projectsQuery = projectsQuery.Where(x => x.CreatedTime > searchData.FromDate.Value);
            }
            if (searchData.ToDate != null)
            {
                projectsQuery = projectsQuery.Where(x => x.CreatedTime < searchData.ToDate.Value);
            }
            if (!String.IsNullOrEmpty(searchData.CreatedById))
            {
                projectsQuery = projectsQuery.Where(x => x.CreatedBy.Id == searchData.CreatedById);
            }

            switch (searchData.OrderBy)
            {
                case "ProjectName":
                    {
                        projectsQuery = projectsQuery.OrderBy(x => x.Name);
                        break;
                    }
                case "CreatedTime":
                    {
                        projectsQuery = projectsQuery.OrderBy(x => x.CreatedTime);
                        break;
                    }
                case "CreatedBy":
                    {
                        projectsQuery = projectsQuery.OrderBy(x => x.CreatedBy.Name);
                        break;
                    }
                case "ProjectNameDesc":
                    {
                        projectsQuery = projectsQuery.OrderByDescending(x => x.Name);
                        break;
                    }
                case "CreatedTimeDesc":
                    {
                        projectsQuery = projectsQuery.OrderByDescending(x => x.CreatedTime);
                        break;
                    }
                case "CreatedByDesc":
                    {
                        projectsQuery = projectsQuery.OrderByDescending(x => x.CreatedBy.Name);
                        break;
                    }
                default:
                    {
                        projectsQuery = projectsQuery.OrderByDescending(x => x.CreatedTime);
                        break;
                    }
            }
            //Page number starts from 1, first page shouldnt skip any
            projectsQuery.Skip((searchData.PageNumber - 1) * searchData.PageSize).Take(searchData.PageSize);

            return await projectsQuery.Select(x => new ProjectDto 
                                                { 
                                                    CreatedBy = x.CreatedBy.Name,
                                                    CreatedTime =x.CreatedTime,
                                                    Description = x.Description,
                                                    Id = x.Id,
                                                    Name = x.Name
                                                }
                                             ).ToListAsync();
        }
    }
}
