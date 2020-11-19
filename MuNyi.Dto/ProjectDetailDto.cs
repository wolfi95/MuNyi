using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dto
{
    public class ProjectDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UserDto CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public IEnumerable<TaskDto> Tasks { get; set; }
        public double LoggedHours { get; set; }        
    }
}
