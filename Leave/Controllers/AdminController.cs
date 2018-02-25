using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;
using System.Net.Http;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Leave.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var context = new LeaveRequestContext();
            var userName = User.Identity.Name.Split('@')[0];
            var Admincontext = new AdminModelContext();
            var admin = Admincontext.AdminModel.Where(s => s.Name == userName).Count();

            if (admin == 0)
            {
                return View("SimpleError");
            }
            var baseContext = context.LeaveRequest;

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

        public IActionResult Approve()
        {
            var userName = User.Identity.Name.Split('@')[0];
            var context = new AdminModelContext();
            var admin = context.AdminModel.Where(s => s.Name == userName).Count();

            if(admin == 0 )
            {
                return View("SimpleError");
            }
            else
            {
                var LeaveContext = new LeaveRequestContext();
                var pending = LeaveContext.LeaveRequest
                                .Where(s => s.Approved == "NA");
                return View(pending);
            }
        }

        [HttpPost]
        public HttpResponseMessage Approver(string Id, int Approve)
        {
            var context = new LeaveRequestContext();
            int RealId = Convert.ToInt32(Id);
            var selected = context.LeaveRequest.Where(s => s.Id == RealId);

            if (selected == null)
            {
                // To date is greater than From
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest); ;
            }
            else
            {
                if(Approve == 1)
                {
                    foreach(var element in selected)
                    {
                        // This loop should happen only once
                        element.Approved = "Approved";
                        element.Approver = User.Identity.Name.Split('@')[0];
                    }
                    context.SaveChanges();

                }
                else if(Approve==2)
                {
                    foreach (var element in selected)
                    {
                        // This loop should happen only once
                        element.Approved = "Rejected";
                        element.Approver = User.Identity.Name.Split('@')[0];
                    }
                    context.SaveChanges();

                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK); ;
            }

        }
    }
}