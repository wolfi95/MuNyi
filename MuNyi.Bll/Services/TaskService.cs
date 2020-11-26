using Microsoft.EntityFrameworkCore;
using MuNyi.Bll.ServiceInterfaces;
using MuNyi.Dal;
using MuNyi.Dal.Entities.Authentication;
using MuNyi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuNyi.Bll.Services
{
    public class TaskService : ITaskService
    {
        private readonly MuNyiContext context;

        public TaskService(MuNyiContext context)
        {
            this.context = context;
        }

        public async Task CreateNewTaskAsync(Guid projectId, NewTaskDto newTask, User user)
        {
            var proj = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            if (proj == null)
            {
                throw new ArgumentException("Nem található projekt ilyen azonosítóval");
            }

            context.Tasks.Add(new Dal.Entities.Task
            {
                CreatedBy = user,
                CreatedDate = DateTime.Now,
                Description = newTask.Description,
                Name = newTask.Name,
                ProjectId = projectId,
            });
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync(Guid projectId)
        {
            var proj = await context.Projects.Include(x => x.CreatedBy).FirstOrDefaultAsync(x => x.Id == projectId);
            if (proj == null)
            {
                throw new ArgumentException("Nem található projekt ilyen azonosítóval");
            }

            return await context.Tasks.Where(x => x.ProjectId == projectId).Select(x => new TaskDto
            {
                CreatedBy = x.CreatedBy.Name,
                Name = x.Name,
                CreatedDate = x.CreatedDate,
                Description = x.Description,
                Id = x.Id,
                ProjectId = x.ProjectId
            }).ToListAsync();
        }

        public async Task<IEnumerable<TaskDto>> SearchTasksAsync(Guid projectId, SearchTaskDto searchData)
        {
            var proj = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            if (proj == null)
            {
                throw new ArgumentException("Nem található projekt ilyen azonosítóval");
            }

            var tasksQuery = context.Tasks.Include(x => x.CreatedBy).Where(x => x.ProjectId == projectId).AsQueryable();

            if (!String.IsNullOrEmpty(searchData.TaskName))
            {
                tasksQuery = tasksQuery.Where(x => x.Name.Contains(searchData.TaskName));
            }
            if (!String.IsNullOrEmpty(searchData.TaskDescription))
            {
                tasksQuery = tasksQuery.Where(x => x.Description.Contains(searchData.TaskDescription));
            }
            if (searchData.FromDate != null)
            {
                tasksQuery = tasksQuery.Where(x => x.CreatedDate > searchData.FromDate.Value);
            }
            if (searchData.ToDate != null)
            {
                tasksQuery = tasksQuery.Where(x => x.CreatedDate < searchData.ToDate.Value);
            }
            if (!String.IsNullOrEmpty(searchData.CreatedById))
            {
                tasksQuery = tasksQuery.Where(x => x.CreatedBy.Id == searchData.CreatedById);
            }

            switch (searchData.OrderBy)
            {
                case "ProjectName":
                    {
                        tasksQuery = tasksQuery.OrderBy(x => x.Name);
                        break;
                    }
                case "CreatedTime":
                    {
                        tasksQuery = tasksQuery.OrderBy(x => x.CreatedDate);
                        break;
                    }
                case "CreatedBy":
                    {
                        tasksQuery = tasksQuery.OrderBy(x => x.CreatedBy.Name);
                        break;
                    }
                case "ProjectNameDesc":
                    {
                        tasksQuery = tasksQuery.OrderByDescending(x => x.Name);
                        break;
                    }
                case "CreatedTimeDesc":
                    {
                        tasksQuery = tasksQuery.OrderByDescending(x => x.CreatedDate);
                        break;
                    }
                case "CreatedByDesc":
                    {
                        tasksQuery = tasksQuery.OrderByDescending(x => x.CreatedBy.Name);
                        break;
                    }
                default:
                    {
                        tasksQuery = tasksQuery.OrderByDescending(x => x.CreatedDate);
                        break;
                    }
            }
            //Page number starts from 1, first page shouldnt skip any
            tasksQuery.Skip((searchData.PageNumber - 1) * searchData.PageSize).Take(searchData.PageSize);

            return await tasksQuery.Select(x => new TaskDto
            {
                CreatedBy = x.CreatedBy.Name,
                CreatedDate = x.CreatedDate,
                Description = x.Description,
                Id = x.Id,
                Name = x.Name,
                ProjectId = x.ProjectId
            }).ToListAsync();
        }

        public async System.Threading.Tasks.Task DeleteTask(Guid projectId, Guid Id)
        {
            var proj = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            if (proj == null)
            {
                throw new ArgumentException("Nem található projekt ilyen azonosítóval");
            }

            var task = await context.Tasks.FirstOrDefaultAsync(x => x.Id == Id && x.ProjectId == projectId);
            if(task == null)
            {
                throw new ArgumentException("Nem található feladat ilyen azonosítóval");
            }

            using (var transation = context.Database.BeginTransaction())
            {
                context.WorkItems.RemoveRange(task.WorkItems);
                context.Tasks.Remove(task);

                await context.SaveChangesAsync();
                transation.Commit();
            }
        }

        public async Task<TaskDetailDto> GetTaskByIdAsync(Guid projectId, Guid Id)
        {
            var proj = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            if (proj == null)
            {
                throw new ArgumentException("Nem található projekt ilyen azonosítóval");
            }

            var task = await context.Tasks.Include(x => x.CreatedBy).Include(x => x.WorkItems).ThenInclude(y => y.CreatedBy).FirstOrDefaultAsync(x => x.Id == Id && x.ProjectId == projectId);
            if (task == null)
            {
                throw new ArgumentException("Nem található feladat ilyen azonosítóval");
            }

            return new TaskDetailDto
            {
                CreatedBy = new UserDto { Id = task.CreatedBy.Id, Name = task.CreatedBy.Name },
                Name = task.Name,
                Id = task.Id,
                CreatedDate = task.CreatedDate,
                Description = task.Description,
                LoggedHours = task.WorkItems.Sum(x => x.Time),
                WorkItems = task.WorkItems.Select(x => new WorkDto { CreatedDate = x.CreatedDate, Comment = x.Comment, CreatedBy = x.CreatedBy.Name, Id = x.Id, Time = x.Time }),
                ProjectId = task.ProjectId
            };
        }

        public async Task<double> GetTaskWorkAsync(Guid projectId, Guid Id)
        {
            var proj = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectId);
            if (proj == null)
            {
                throw new ArgumentException("Nem található projekt ilyen azonosítóval");
            }

            var task = await context.Tasks.Include(x => x.WorkItems).FirstOrDefaultAsync(x => x.Id == Id && x.ProjectId == projectId);
            if (task == null)
            {
                throw new ArgumentException("Nem található feladat ilyen azonosítóval");
            }

            return task.WorkItems.Sum(x => x.Time);
        }
    }
}
