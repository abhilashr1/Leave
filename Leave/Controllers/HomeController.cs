using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;
using Microsoft.AspNetCore.Http;

namespace Leave.Controllers
{
    public class HomeController : Controller
    {
        private readonly Microsoft.AspNetCore.Http.ISession session;
        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            this.session = httpContextAccessor.HttpContext.Session;
        }

        public IActionResult Index()
        {
            var userName = User.Identity.Name.Split('\\')[1];
            var context = new AdminModelContext();
            var IsAdmin = context.AdminModel.Where(s => s.Name == userName);
            if(IsAdmin != null)
            {
                HttpContext.Session.SetString("admin", "true");
            }
            var LeaveContext = new LeaveRequestContext();

            var pendingcount = LeaveContext.LeaveRequest
                                .Where(o => o.Name == userName)
                                .Where(o => o.Approved == "NA")
                                .Count();
            ViewData["pendingcount"] = pendingcount;

            var approvedcount = LeaveContext.LeaveRequest
                                .Where(o => o.Name == userName)
                                .Where(o => o.Approved == "Approved")
                                .Count();
            ViewData["approvedcount"] = approvedcount;

            var rejectedcount = LeaveContext.LeaveRequest
                                .Where(o => o.Name == userName)
                                .Where(o => o.Approved == "Rejected")
                                .Count();
            ViewData["rejectedcount"] = rejectedcount;

            return View();
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
