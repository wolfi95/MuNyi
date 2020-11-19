using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Web.Helpers
{
    public class PagedData<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
