using MuNyi.Dal.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dal.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }       
        public IEnumerable<Task> Tasks  { get; set; }
    }
}
