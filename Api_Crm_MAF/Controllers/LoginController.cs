using Api_Crm_MAF.Models;
using Api_Crm_MAF.Security;
using System.Net;
using System.Threading;
using System.Web.Http;

namespace Api_Crm_MAF.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("echoping")]
        public IHttpActionResult EchoPing()
        {
            return Ok(true);
        }

        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginRequest login)
        {
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            bool isUserValid = false;
            isUserValid = (login.Username == "WSM4F");
            isUserValid = (login.Password == "=6O%Hi@Dyu");
            if (isUserValid)
            {
                var rolename = "User";
                var token = TokenGenerator.GenerateTokenJwt(login.Username, rolename);
                return Ok(token);
            }
            // Unauthorized access 
            return Unauthorized();
        }
    }
}
