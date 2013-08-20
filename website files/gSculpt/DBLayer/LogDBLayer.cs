using gFit.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gFit.DBLayer
{
    public class LogDBLayer : DBLayer
    {


        private static LogDBLayer instance;

         public static LogDBLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogDBLayer();
                }
                return instance;
            }
            private set { }
        }




        private LogDBLayer()
            : base()
        {
        }




        public bool AddToLog(string s)
        {

            List<SqlParameter> list = new List<SqlParameter>();

            AddSqlParameter(list, "@log_text", s);
            
            return ExecuteNonQuery("usp_addToLog", list) == 1;

        }




    }
}