using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dto
{
    public class WorkDto
    {
        public Guid Id { get; set; }
        public double Time { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }        
    }
}
