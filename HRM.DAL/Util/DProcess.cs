using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Util
{
    public class DProcess
    {
        long ReturnCode = 0;
        Boolean Inserted = false;
        readonly static oneportalEntities _context = new oneportalEntities();




        public static IQueryable<ws_lookup> GetLookupType()
        {
            return _context.ws_lookup.Where(e => e.category == "complain");
        }

        public static IQueryable<ws_lookup> GetLookup(string category)
        {
            return _context.ws_lookup.Where(e => e.category == category);
        }

        public static IQueryable<ws_complain> GetComplain()
        {
            return _context.ws_complain.AsQueryable();
        }



        public Boolean InsertNewComplain(String Type,
                                           String Priority,
                                           String Comment,
                                           String Deleted,
                                           String ResolvedBy,
                                           String Resolved,
                                           String Filename,
                                           String CreateDate,
                                           String UpdateDate,
                                           String ResolutionDate,
                                           out String ErrMsg)

        {
            Inserted = false;
            ReturnCode = 0;
            ErrMsg = "";

            try
            {
                SqlCommand command = DataAccess.CreateCommand();
                //command = DataAccess.CreateCommand();

                //Set the Stored Procedure name
                command.CommandText = "InsertnigQualImplementingPartner";

                // create a new parameter
                SqlParameter param = command.CreateParameter();
                param.ParameterName = "@Type";
                param.Value = Type;
                param.SqlDbType = SqlDbType.VarChar;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@Priority";
                param.Value = Priority;
                param.SqlDbType = SqlDbType.VarChar;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@Comment";
                param.Value = Comment;
                param.SqlDbType = SqlDbType.VarChar;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "Deleted";
                param.Value = Deleted;
                param.SqlDbType = SqlDbType.VarChar;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ResolvedBy";
                param.Value = ResolvedBy;
                param.SqlDbType = SqlDbType.VarChar;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@Filename";
                param.Value = Filename;
                param.SqlDbType = SqlDbType.VarChar;
                command.Parameters.Add(param);

                //    
                param = command.CreateParameter();
                param.ParameterName = "@Resolved";
                param.Value = Resolved;
                param.SqlDbType = SqlDbType.VarChar;
                command.Parameters.Add(param);
                //     


                ReturnCode = DataAccess.ExecuteNonQuery(command);
                command.Connection.Close();

            }
            catch (Exception extn)
            {
                ErrMsg = extn.Message;

            }

            if ((ErrMsg.Trim().Length == 0) && (ReturnCode != 0))
            {
                Inserted = true;

            }
            else
            {
                Inserted = false;
            }

            return Inserted;
        }
    }
}
