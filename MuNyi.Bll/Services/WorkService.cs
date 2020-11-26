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
    public class WorkService : IWorkService
    {
        private readonly MuNyiContext context;

        public WorkService(MuNyiContext context)
        {
            this.context = context;
        }

        public async Task CreateNewWorkItem(Guid projectId, Guid id, NewWorkItemDto newWorkItemData, User user)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Id == id);
            if (task == null)
            {
                throw new ArgumentException("Nem található feladat a megadott projekten ilyen azonosítóval");
            }            

            context.WorkItems.Add(new Dal.Entities.Work
            {
                Comment = newWorkItemData.Comment,
                CreatedBy = user,
                CreatedDate = DateTime.Now,
                TaskId = id,
                Time = newWorkItemData.Time
            });

            await context.SaveChangesAsync();
        }

        public async Task DeleteWorkItem(Guid projectId, Guid id, Guid workItemId, User user)
        {
            var workItem = await context.WorkItems.Include(x => x.CreatedBy).FirstOrDefaultAsync(x => x.TaskId == id && x.Task.ProjectId == projectId && x.Id == workItemId);
            if(workItem == null)
            {
                throw new ArgumentException("Nem található munkaidő bejegyzés");
            }
            if (user != workItem.CreatedBy)
            {
                throw new ArgumentException("Nincs jogosultsága más bejegyszésének törlésére");
            }

            context.WorkItems.Remove(workItem);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkDto>> GetTaskWorkItemsAsync(Guid projectId, Guid id)
        {
            var task = await context.Tasks.Include(x => x.WorkItems).ThenInclude(y => y.CreatedBy).FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Id == id);
            if(task == null)
            {
                throw new ArgumentException("A projekten nem található feladat ezzel az azonosítóval.");
            }

            return task.WorkItems.Select(x => new WorkDto { CreatedBy = x.CreatedBy.Name, Comment = x.Comment, CreatedDate = x.CreatedDate, Id = x.Id, Time = x.Time}).ToList();
        }

        public async Task<IEnumerable<WorkDto>> SearchTaskWorkItemsAsync(Guid projectId, Guid id, SearchWorkItemDto searchData)
        {
            var task = await context.Tasks.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Id == id);
            if (task == null)
            {
                throw new ArgumentException("Nem található feladat a megadott projekten ilyen azonosítóval");
            }

            var workItemsQuery = context.WorkItems.Include(x => x.CreatedBy).Where(x => x.TaskId == id).AsQueryable();

            if (!String.IsNullOrEmpty(searchData.Comment))
            {
                workItemsQuery = workItemsQuery.Where(x => x.Comment.Contains(searchData.Comment));
            }
            
            if (searchData.CreatedDateFrom != null)
            {
                workItemsQuery = workItemsQuery.Where(x => x.CreatedDate > searchData.CreatedDateFrom.Value);
            }
            if (searchData.CreatedDateFrom != null)
            {
                workItemsQuery = workItemsQuery.Where(x => x.CreatedDate < searchData.CreatedDateFrom.Value);
            }
            if (!String.IsNullOrEmpty(searchData.CreatedById))
            {
                workItemsQuery = workItemsQuery.Where(x => x.CreatedBy.Id == searchData.CreatedById);
            }

            switch (searchData.OrderBy)
            {                
                case "CreatedTime":
                    {
                        workItemsQuery = workItemsQuery.OrderBy(x => x.CreatedDate);
                        break;
                    }
                case "CreatedBy":
                    {
                        workItemsQuery = workItemsQuery.OrderBy(x => x.CreatedBy.Name);
                        break;
                    }               
                case "CreatedTimeDesc":
                    {
                        workItemsQuery = workItemsQuery.OrderByDescending(x => x.CreatedDate);
                        break;
                    }
                case "CreatedByDesc":
                    {
                        workItemsQuery = workItemsQuery.OrderByDescending(x => x.CreatedBy.Name);
                        break;
                    }
                default:
                    {
                        workItemsQuery = workItemsQuery.OrderByDescending(x => x.CreatedDate);
                        break;
                    }
            }
            
            //Page number starts from 1, first page shouldnt skip any
            workItemsQuery.Skip((searchData.PageNumber - 1) * searchData.PageSize).Take(searchData.PageSize);

            return await workItemsQuery.Select(x => new WorkDto
            {
                CreatedBy = x.CreatedBy.Name,
                CreatedDate = x.CreatedDate,
                Comment = x.Comment,
                Time = x.Time,
                Id = x.Id
            }).ToListAsync();
        }
    }
}
