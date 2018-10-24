using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Pagination.Paginationv2
{
    public class ResponseJSON
    {
        public object items { get; set; }
        public int currentPage { get; set; }
        public int limitRow { get; set; }
        public int resultRow { get; set; }
    }
}
