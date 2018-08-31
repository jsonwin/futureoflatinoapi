using System.Net.Http;
using System.Web.Http;
using FutureOfLatinos.Models.Responses;
using FutureOfLatinos.Web.helpers;

namespace FutureOfLatinos.Web.Controllers.Api
{
    [RoutePrefix("api/webscraper")]
    public class WebScraperController : ApiController
    {
        [Route, AllowAnonymous]
        public HttpResponseMessage Get()
        {
            WebScraper scraper = new WebScraper();
            ItemResponse<string> resp = new ItemResponse<string>();
            resp.Item = scraper.GetContent("og: description");
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, resp);
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}