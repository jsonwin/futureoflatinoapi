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
    public class FileStorageService : BaseService, IFileStorageService
    {
        public int Insert(FileStorageAddRequest model)
        {
            int id = 0;
            this.DataProvider.ExecuteNonQuery(
                "FileStorage_Insert",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
                {
                    SqlParameter parm = new SqlParameter();
                    parm.ParameterName = "@Id";
                    parm.SqlDbType = SqlDbType.Int;
                    parm.Direction = ParameterDirection.Output;
                    paramCol.Add(parm);
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

        public List<FileViewModel> GetAll()
        {
            List<FileViewModel> result = new List<FileViewModel>();
            this.DataProvider.ExecuteCmd(
                "FileStorage_SelectAll",
                inputParamMapper: null,
                singleRecordMapper: delegate(IDataReader reader, short set)
                {
                    FileViewModel model = new FileViewModel();
                    int index = 0;
                    model.Id = reader.GetSafeInt32(index++);
                    model.IconUrl = reader.GetSafeString(index++);
                    model.UserFileName = reader.GetSafeString(index++);
                    model.SystemFileName = reader.GetSafeString(index++);
                    model.Location = reader.GetSafeString(index++);
                    model.CreatedBy = reader.GetSafeString(index++);
                    result.Add(model);
                }
            );
            return result;
        }

        public FileStorage GetById(int id)
        {
            FileStorage model = null;
            this.DataProvider.ExecuteCmd(
               "FileStorage_SelectById",
               inputParamMapper: delegate (SqlParameterCollection paramCol)
               {
                   paramCol.AddWithValue("@Id", id);
               },
               singleRecordMapper: delegate (IDataReader reader, short set)
               {
                   model = Mapper(reader);
               }
            );
            return model;
        }

        public void Delete(int id)
        {
            this.DataProvider.ExecuteNonQuery(
                "FileStorage_Delete",
                inputParamMapper: delegate(SqlParameterCollection paramCol)
                {
                    paramCol.AddWithValue("@Id", id);
                }
           );
        }

        private FileStorage Mapper(IDataReader reader)
        {
            FileStorage model = new FileStorage();
            int index = 0;
            model.Id = reader.GetSafeInt32(index++);
            model.FileTypeId = reader.GetSafeInt32(index++);
            model.UserFileName = reader.GetSafeString(index++);
            model.SystemFileName = reader.GetSafeString(index++);
            model.Location = reader.GetSafeString(index++);
            model.CreatedDate = reader.GetDateTime(index++);
            model.ModifiedDate = reader.GetDateTime(index++);
            model.CreatedBy = reader.GetSafeString(index++);
            return model;
        }
    }
}
