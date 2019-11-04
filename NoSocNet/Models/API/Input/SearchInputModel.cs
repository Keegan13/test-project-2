using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoSocNet.Models.API.Input
{
    public class SearchInputModel
    {
        public string Keywords { get; set; }

        public int Chunk { get; set; }
    }
}
