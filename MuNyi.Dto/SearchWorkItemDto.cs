using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dto
{
    public class SearchWorkItemDto
    {
        public string Comment { get; set; }
        public string CreatedById { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        public string OrderBy { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
