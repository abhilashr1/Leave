using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Leave.Controllers
{
    public class RequestController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void Submit(DateTime From, DateTime To, string Reason)
        {
            int result = DateTime.Compare(From, To);
            if (result > 0)
            {
                // To date is greater than From
                return;
            }
            else
            {
                string Name = User.Identity.Name;
                using (var db = new LeaveRequestContext())
                {
                    db.LeaveRequest.Add(new LeaveRequest {
                        Name = Name,
                        From = From,
                        To = To,
                        Approved = "Not Approved Yet",
                        Approver = "Not Approved Yet"
                    });
                    var count = db.SaveChanges();
                }

            }
                
        }
    }
}
