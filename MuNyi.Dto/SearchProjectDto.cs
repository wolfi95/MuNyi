using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dto
{
    public class SearchProjectDto
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime? FromDate { get; set; }        
        public DateTime? ToDate { get; set; }
        public string CreatedById { get; set; }
        public string OrderBy { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
