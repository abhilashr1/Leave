using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;
using System.Dynamic;

namespace Leave.Controllers
{
    public class ListController : Controller
    {
        public IActionResult Index()
        {
            var context = new LeaveRequestContext();

            var baseContext = context.LeaveRequest
                       .Where(s => s.Name == User.Identity.Name);

            var Approved = baseContext
                            .Where(x => x.Approved != "Approved");

            var Rejected = baseContext
                            .Where(x => x.Approved != "Rejected");


            var Pending = baseContext
                            .Where(x => x.Approved == "NA");

            dynamic model = new ExpandoObject();
            model.Approved = Approved;
            model.Rejected = Rejected;
            model.Pending = Pending;
            return View(model);
        }
    }
}