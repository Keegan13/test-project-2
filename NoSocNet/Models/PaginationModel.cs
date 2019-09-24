using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models
{
    public class PaginationModel
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int Count { get; set; } = 0;
    }
}
