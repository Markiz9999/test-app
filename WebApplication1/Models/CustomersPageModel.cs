using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CustomersPageModel
    {
        public int ItemsCount { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public List<SimpleCustomerViewModel> Customers { get; set; } 
    }
}