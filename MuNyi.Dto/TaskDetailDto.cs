using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dto
{
    public class TaskDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserDto CreatedBy { get; set; }
        public IEnumerable<WorkDto> WorkItems { get; set; }
        public double LoggedHours { get; set; }
        public Guid ProjectId { get; set; }
    }
}
