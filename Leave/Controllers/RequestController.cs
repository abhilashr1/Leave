using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;
using System.Net.Http;

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
        public HttpResponseMessage Submit(string From, string To, string Reason)
        {
            DateTime FromDt = Convert.ToDateTime(From);
            DateTime ToDt = Convert.ToDateTime(To);
            string ReasonText = Reason.Trim();
            int result = DateTime.Compare(FromDt, ToDt);
            if (result > 0)
            {
                // To date is greater than From
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest); ;
            }
            else
            {
                string Name = User.Identity.Name.Split('\\')[1]; ;
                using (var db = new LeaveRequestContext())
                {
                    db.LeaveRequest.Add(new LeaveRequest {
                        Name = Name,
                        From = FromDt,
                        To = ToDt,
                        Reason = ReasonText,
                        Approved = "NA",
                        Approver = "NA",
                        RequestTime = DateTime.Now,

                    });
                    var count = db.SaveChanges();
                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK); ;
            }

        }
    }
}
