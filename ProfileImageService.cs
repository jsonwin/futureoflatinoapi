using FutureOfLatinos.Data;
using FutureOfLatinos.Models.Requests;
using FutureOfLatinos.Models.ViewModels;
using FutureOfLatinos.Services.Security;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;

namespace FutureOfLatinos.Services
{
    public class ProfileImageService : BaseService, IProfileImageService
    {
        private IPrincipal _principal;
        public ProfileImageService(IPrincipal principal)
        {
            _principal = principal;
        }

        public int Insert(ProfileImageAddRequest model)
        {
            //Models.IUserAuthData currentUser = _principal.Identity.GetCurrentUser();
            int id = 0;
            this.DataProvider.ExecuteNonQuery(
                "PersonImage_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    SqlParameter parm = new SqlParameter();
                    parm.ParameterName = "@Id";
                    parm.SqlDbType = System.Data.SqlDbType.Int;
                    parm.Direction = System.Data.ParameterDirection.Output;
                    paramCol.Add(parm);
                    paramCol.AddWithValue("@UserId", model.UserId);
                    paramCol.AddWithValue("@FileTypeId", model.FileTypeId);
                    paramCol.AddWithValue("@UserFileName", model.UserFileName);
                    paramCol.AddWithValue("@SystemFileName", model.SystemFileName);
                    paramCol.AddWithValue("@Location", model.Location);
                    paramCol.AddWithValue("@CreatedBy", model.CreatedBy);
                },
                returnParameters: delegate (SqlParameterCollection paramCol)
                {
                    id = (int)paramCol["@Id"].Value;
                }
                );
            return id;
        }

        public ProfileImageViewModel GetById(int id)
        {
            ProfileImageViewModel model = null;
            this.DataProvider.ExecuteCmd(
                "PersonImage_SelectById",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@Id", id);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    model = new ProfileImageViewModel();
                    model.PersonId = reader.GetSafeInt32(index++);
                    model.Title = reader.GetSafeString(index++);
                    model.FirstName = reader.GetSafeString(index++);
                    model.MiddleInitial = reader.GetSafeString(index++);
                    model.LastName = reader.GetSafeString(index++);
                    model.Bio = reader.GetSafeString(index++);
                    model.ModifiedBy = reader.GetSafeString(index++);
                    model.UserId = reader.GetSafeInt32(index++);
                    model.FileStorageId = reader.GetSafeInt32(index++);
                    model.Location = reader.GetSafeString(index++);
                }
                );
            return model;
        }
    }
}

