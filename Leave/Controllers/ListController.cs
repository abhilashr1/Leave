using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;
using System.Dynamic;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace Leave.Controllers
{
    [Authorize]
    public class ListController : Controller
    {
        public IActionResult Index()
        {
            var context = new LeaveRequestContext();
            var userName = User.Identity.Name.Split('@')[0];
            var baseContext = context.LeaveRequest
                       .Where(s => s.Name == userName);

            var Approved = baseContext
                            .Where(x => x.Approved == "Approved");

            var Rejected = baseContext
                            .Where(x => x.Approved == "Rejected");

            var Pending = baseContext
                            .Where(x => x.Approved == "NA");

            dynamic model = new ExpandoObject();
            model.Approved = Approved;
            model.Rejected = Rejected;
            model.Pending = Pending;
            return View(model);
        }

        [HttpPost]
        public HttpResponseMessage Delete(string sid)
        {
            try
            {
                int Id = Convert.ToInt32(sid);
                string Name = User.Identity.Name.Split('@')[0];
                using (var db = new LeaveRequestContext())
                {
                    var selected = db.LeaveRequest
                            .Where(x => x.Id == Id && x.Approved == "NA").First();

                    db.LeaveRequest.Remove(selected);
                    var count = db.SaveChanges();
                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK); 
            }
            catch(Exception e)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest); 
            }
        }
    }
}