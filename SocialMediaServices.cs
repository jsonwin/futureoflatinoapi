using FutureOfLatinos.Data;
using FutureOfLatinos.Models.Domain;
using FutureOfLatinos.Models.Requests;
using FutureOfLatinos.Models.ViewModels;
using FutureOfLatinos.Services.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;



namespace FutureOfLatinos.Services
{
    public class SocialMediaServices : BaseService, ISocialMediaServices
    {
        public int Insert(SocialMediaAddRequest model)
        {
            int id = 0;
            this.DataProvider.ExecuteNonQuery(
         "SocialMedia_Insert",
         inputParamMapper: delegate (SqlParameterCollection paramCol)
         {
             SqlParameter parm = new SqlParameter();
             parm.ParameterName = "@Id";
             parm.SqlDbType = System.Data.SqlDbType.Int;
             parm.Direction = System.Data.ParameterDirection.Output;
             paramCol.Add(parm);
             paramCol.AddWithValue("@UserId", model.UserId);
             paramCol.AddWithValue("@Url", model.Url);
             paramCol.AddWithValue("@SocialMediaTypeId", model.SocialMediaTypeId);
             paramCol.AddWithValue("@ModifiedBy", model.ModifiedBy);
         },
         returnParameters: delegate (SqlParameterCollection paramCol)
         {
             id = (int)paramCol["@Id"].Value;
         }
                );
            return id;
        }

        public List<SocialMediaViewModel> GetAll()
        {
            List<SocialMediaViewModel> result = new List<SocialMediaViewModel>();
            this.DataProvider.ExecuteCmd(
            "SocialMedia_SelectAll",
            inputParamMapper: null,
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                SocialMediaViewModel model = new SocialMediaViewModel();
                int index = 0;
                model.UserId = reader.GetSafeInt32(index++);
                model.PersonId = reader.GetSafeInt32(index++);
                model.SocialMediaId = reader.GetSafeInt32(index++);
                model.Url = reader.GetSafeString(index++);

                result.Add(model);
            }
                );
            return result;
        }

        public List<SocialMediaViewModel> GetById(int id)
        {
            List<SocialMediaViewModel> result = new List<SocialMediaViewModel>();
            this.DataProvider.ExecuteCmd(
              "SocialMedia_SelectById",
              inputParamMapper: delegate (SqlParameterCollection paramCol)
              {
                  paramCol.AddWithValue("@Id", id);
              },
               singleRecordMapper: delegate (IDataReader reader, short set)
               {
                   SocialMediaViewModel model = new SocialMediaViewModel();
                   int index = 0;
                   model.UserId = reader.GetSafeInt32(index++);
                   model.PersonId = reader.GetSafeInt32(index++);
                   model.SocialMediaId = reader.GetSafeInt32(index++);
                   model.Url = reader.GetSafeString(index++);

                   result.Add(model);
               }
                );
            return result;
        }

        public void Update(SocialMediaUpdateRequest model)
        {
            this.DataProvider.ExecuteNonQuery(
             "SocialMedia_Update",
             inputParamMapper: delegate (SqlParameterCollection paramCol)
             {
                 paramCol.AddWithValue("@Id", model.Id);
                 paramCol.AddWithValue("@Url", model.Url);
                 paramCol.AddWithValue("@SocialMediaTypeId", model.SocialMediaTypeId);
                 paramCol.AddWithValue("@ModifiedBy", model.ModifiedBy);
             },
             returnParameters: null
                );
        }

        public void Delete(int id)
        {
            this.DataProvider.ExecuteNonQuery(
        "SocialMedia_Delete",
        inputParamMapper: delegate (SqlParameterCollection paramCol)
        {
            paramCol.AddWithValue("@Id", id);
        }
                );
        }

        private SocialMedia Mapper(IDataReader reader)
        {
            SocialMedia model = new SocialMedia();
            int index = 0;
            model.Id = reader.GetSafeInt32(index++);
            model.Url = reader.GetSafeString(index++);
            model.SocialMediaTypeId = reader.GetSafeInt32(index++);
            model.CreatedDate = reader.GetDateTime(index++);
            model.ModifiedDate = reader.GetDateTime(index++);
            model.ModifiedBy = reader.GetSafeString(index++);
            return model;
     
        }
    }
}
  
      