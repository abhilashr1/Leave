using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leave.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Leave.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Claims;

namespace Leave.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //private readonly ILogger _logger;

        private readonly Microsoft.AspNetCore.Http.ISession session;
        private static readonly HttpClient Client = new HttpClient();
        private readonly ITokenCacheFactory _tokenCacheFactory;
        private readonly AuthOptions _authOptions;

        
       /*public HomeController(IHttpContextAccessor httpContextAccessor) 
        {
            this.session = httpContextAccessor.HttpContext.Session;
            //_logger = logger;
        }*/

        public HomeController (IHttpContextAccessor httpContextAccessor, ITokenCacheFactory tokenCacheFactory, IOptions<AuthOptions> authOptions)
        {
            this.session = httpContextAccessor.HttpContext.Session;
            _tokenCacheFactory = tokenCacheFactory;
            _authOptions = authOptions.Value;
        }
        

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var claims = userIdentity.Claims;
            var userName = User.Identity.Name.Split('@')[0];
            var firstName= claims.Where(c => c.Type == ClaimTypes.GivenName).Single().Value;
            var secondName = claims.Where(c => c.Type == ClaimTypes.Surname).Single().Value;

            ViewData["firstName"] = firstName;
            ViewData["secondName"] = secondName;

            var context = new AdminModelContext();
            var IsAdmin = context.AdminModel.Where(s => s.Name == userName).Count();
            if(IsAdmin == 1)
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
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SimpleError()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProfilePhoto()
        {
            //var userName = User.Identity.Name.Split('@')[0];
            //string location = $"D:\\photos\\{userName}.jpg";
                        
            HttpResponseMessage res = await QueryGraphAsync("/me/photo/$value");
            await res.Content.LoadIntoBufferAsync();

            return File(await res.Content.ReadAsStreamAsync(), "image/jpeg");
        }

        private async Task<HttpResponseMessage> QueryGraphAsync(string relativeUrl)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/beta" + relativeUrl);

            string accessToken = await GetAccessTokenAsync();
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            string authority = _authOptions.Authority;

            var cache = _tokenCacheFactory.CreateForUser(User);

            var authContext = new AuthenticationContext(authority, cache);

            //App's credentials may be needed if access tokens need to be refreshed with a refresh token
            string clientId = _authOptions.ClientId;
            string clientSecret = _authOptions.ClientSecret;
            var credential = new ClientCredential(clientId, clientSecret);
            var userId = User.GetObjectId();

            var result = await authContext.AcquireTokenSilentAsync(
                "https://graph.microsoft.com",
                credential,
                new UserIdentifier(userId, UserIdentifierType.UniqueId));

            return result.AccessToken;
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
    }
}
