using FutureOfLatinos.Data;
using FutureOfLatinos.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutureOfLatinos.Services
{
    public class ProfileServices : BaseService, IProfileServices
    {
        public ProfileViewModel GetByUserId(int id)
        {
            ProfileViewModel model = null;
            this.DataProvider.ExecuteCmd(
                "Profile_SelectById",
                inputParamMapper: delegate (SqlParameterCollection paramCol)
            {
                paramCol.AddWithValue("@Id", id);
            },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int index = 0; 
                model = new ProfileViewModel();
                model.Name = reader.GetSafeString(index++);
                model.PhoneNumber = reader.GetSafeString(index++);
                model.Address = reader.GetSafeString(index++);
            }
                );
            return model;
        }
    }
}
