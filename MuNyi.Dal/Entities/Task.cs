using MuNyi.Dal.Entities.Authentication;
using System;
using System.Collections.Generic;

namespace MuNyi.Dal.Entities
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public User CreatedBy { get; set; }
        public Project Project { get; set; }
        public Guid ProjectId { get; set; }
        public IEnumerable<Work> WorkItems { get; set; }
    }
}