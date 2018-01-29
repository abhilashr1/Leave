using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;

namespace Leave.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            var context = new LeaveRequestContext();
            var models = context.LeaveRequest.
                Where(s => s.Approved != "Rejected" );

            return View(models);
        }

    }
}