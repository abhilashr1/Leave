using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Leave.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;

        private readonly Microsoft.AspNetCore.Http.ISession session;
        public HomeController(IHttpContextAccessor httpContextAccessor, ILogger<HomeController> logger)
        {
            this.session = httpContextAccessor.HttpContext.Session;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {

            var userName = User.Identity.Name.Split('\\')[1];
            _logger.LogInformation("Username", userName);

            var context = new AdminModelContext();
            var IsAdmin = context.AdminModel.Where(s => s.Name == userName);
            if(IsAdmin.Count() == 1)
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
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return View();
            }
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
