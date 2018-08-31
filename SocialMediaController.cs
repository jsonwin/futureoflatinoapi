using FutureOfLatinos.Models.Domain;
using FutureOfLatinos.Models.Requests;
using FutureOfLatinos.Models.Responses;
using FutureOfLatinos.Models.ViewModels;
using FutureOfLatinos.Services.Interfaces;
using FutureOfLatinos.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Principal;
using FutureOfLatinos.Services.Security;
using FutureOfLatinos.Models;

namespace FutureOfLatinos.Web.Controllers.Api
{
    [RoutePrefix("api/socialmedia")]
    public class SocialMediaController : ApiController
    {
        private ISocialMediaServices _socialMediaServices;

        private IPrincipal _principal;

        public SocialMediaController(ISocialMediaServices socialMediaServices, IPrincipal principal)
        {
            _socialMediaServices = socialMediaServices;
            _principal = principal;
        }

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                //SocialMediaServices svc = new SocialMediaServices();
                //svc.Delete(id);
                _socialMediaServices.Delete(id);
                SuccessResponse resp = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            try
            {
                ItemsResponse<SocialMediaViewModel> resp = new ItemsResponse<SocialMediaViewModel>();
                resp.Items = _socialMediaServices.GetById(id);
                return Request.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("getAll"), HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                ItemsResponse<SocialMediaViewModel> resp = new ItemsResponse<SocialMediaViewModel>();
                resp.Items = _socialMediaServices.GetAll();
                return Request.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Put(SocialMediaUpdateRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IUserAuthData currentUser = _principal.Identity.GetCurrentUser();
                    model.ModifiedBy = currentUser.Name;
                    _socialMediaServices.Update(model);
                    SuccessResponse resp = new SuccessResponse();
                    return Request.CreateResponse(HttpStatusCode.OK, resp);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route, HttpPost]
        public HttpResponseMessage Post(SocialMediaAddRequest model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IUserAuthData currentUser = _principal.Identity.GetCurrentUser();
                    model.ModifiedBy = currentUser.Name;
                    int id = _socialMediaServices.Insert(model);
                    ItemResponse<int> resp = new ItemResponse<int>();
                    resp.Item = id;
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, resp);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
 }
