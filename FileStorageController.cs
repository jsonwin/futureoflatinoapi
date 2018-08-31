using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FutureOfLatinos.Models;
using FutureOfLatinos.Models.Domain;
using FutureOfLatinos.Models.Requests;
using FutureOfLatinos.Models.Responses;
using FutureOfLatinos.Models.ViewModels;
using FutureOfLatinos.Services.Interfaces;
using FutureOfLatinos.Services.Security;
using FutureOfLatinos.Web.helpers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FutureOfLatinos.Web.Controllers.Api
{
    [RoutePrefix("api/fileupload")]
    public class FileStorageController : ApiController
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger();
        private string bucketname = "sabio-training/C53";
        private IAmazonS3 awsS3Client = new AmazonS3Client(Amazon.RegionEndpoint.USWest2);
        private IFileStorageService _fileStorageService;
        private IPrincipal _principal;

        [Route, HttpPost]
        public HttpResponseMessage UploadFile()
        {
            var httpPostedFile = HttpContext.Current.Request.Files[0];
            string fileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
            string extension = Path.GetExtension(httpPostedFile.FileName);
            var newGuid = Guid.NewGuid().ToString("");
            var newFileName = fileName + "_" + newGuid + extension;
            Stream st = httpPostedFile.InputStream;

            try
            {
                if (httpPostedFile != null)
                {
                    TransferUtility utility = new TransferUtility(awsS3Client);
                    TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
                    request.BucketName = bucketname;
                    request.Key = newFileName;
                    request.InputStream = st;
                    log.Debug(newFileName + "uploading to AWS S3");
                    utility.Upload(request); //File Streamed to AWS

                    FileStorageAddRequest model = new FileStorageAddRequest();
                    IUserAuthData currentUser = _principal.Identity.GetCurrentUser();

                    if(extension == ".jpg" || extension == ".jpeg" || extension == ".png" ||
                       extension == ".gif" || extension == ".bmp" || extension == ".svg")
                    {
                        model.FileTypeId = 1;
                    }
                    else 
                    {
                        model.FileTypeId = 8;
                    }
                     //Logic needed in order to separate filetypeId in correct type because this is more specific to documents...
                    model.UserFileName = fileName;
                    model.SystemFileName = newFileName;
                    model.Location = "https://sabio-training.s3.us-west-2.amazonaws.com/C53/" + newFileName;
                    model.CreatedBy = currentUser.Name;
                    int id = _fileStorageService.Insert(model);
                    ItemResponse<int> resp = new ItemResponse<int>();
                    resp.Item = id;
                    log.Debug("URL:" + " " + model.Location);
                    return Request.CreateResponse(HttpStatusCode.OK, resp);

                }
                else
                {
                    log.Error("Error trying to upload and store metadata");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
            } 
            catch (Exception ex)
            {
                log.Error("Unable to upload files");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("getall"), HttpGet]
        public HttpResponseMessage GetAll()
        {
            try
            {
                ItemsResponse<FileViewModel> resp = new ItemsResponse<FileViewModel>();
                resp.Items = _fileStorageService.GetAll();
                log.Debug("All files successfully retrieved");
                return Request.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch (Exception ex)
            {
                log.Error("Unable to retrieve all files");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage GetById(int id)
        {
            try
            {
                ItemResponse<FileStorage> resp = new ItemResponse<FileStorage>();
                resp.Item = _fileStorageService.GetById(id);
                log.Debug("File successfully retrieved");
                return Request.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch (Exception ex)
            {
                log.Error("Unable to get file requested");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("{id}"), HttpDelete]
        public async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            try
            {
                FileStorage model = new FileStorage();
                model = _fileStorageService.GetById(id);
                log.Debug(model.SystemFileName + " file name selected");
                var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/upload"), model.SystemFileName);

                try
                {
                    log.Debug("File to be Deleted:" + " " + model.SystemFileName);
                    var deleteObjectRequest = new DeleteObjectRequest
                    {
                        BucketName = bucketname,
                        Key = model.SystemFileName
                    };
                    log.Debug("File Deleted from AWS S3");
                    await awsS3Client.DeleteObjectAsync(deleteObjectRequest);
                }
                catch (AmazonS3Exception e)
                {
                    log.Error("AWS S3 exception thrown, make sure file exist");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
                }

                if (File.Exists(fileSavePath))
                {
                    log.Debug(model.SystemFileName + " deleted from local server" );
                    File.Delete(fileSavePath);
                }
                log.Debug("Metadata being removed from database");
                _fileStorageService.Delete(id);
                SuccessResponse resp = new SuccessResponse();
                return Request.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch (Exception ex)
            {
                log.Error("File not found or unable to execute request to Delete from Database...");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public FileStorageController(IFileStorageService fileStorageService, IPrincipal principal)
        {

            _fileStorageService = fileStorageService;
            _principal = principal;
        }
    }
}