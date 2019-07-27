using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PaginationViewModel
    {
        public string SortBy { get; set; }

        public bool Desc { get; set; }

        public string SearchString { get; set; }
    }
}