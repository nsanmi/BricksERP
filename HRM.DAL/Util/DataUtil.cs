using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Util
{
    public static class DataUtil
    {
        public static DataSet GetDataSet(SqlCommand cmd)
        {
            // var conn = (SqlConnection)entity.Database.Connection;
            var connstring = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //var conn = (SqlConnection)entity.Database.Connection;
            using (var conn = new SqlConnection(connstring))
            {
                conn.Open();
                cmd.Connection = conn;
                using (cmd)
                {
                    using (SqlDataAdapter a = new SqlDataAdapter(cmd))
                    {

                        DataSet t = new DataSet();
                        a.Fill(t);
                        return t;

                    }
                }
            }


        }
    }
}
