#region

using System.Collections.Generic;
using System.Data.SqlClient;

#endregion

namespace gFit.DBLayer
{
    public class LogDBLayer : DBLayer
    {
        private static LogDBLayer instance;


        private LogDBLayer()
        {
        }

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


        public bool AddToLog(string s)
        {
            var list = new List<SqlParameter>();

            AddSqlParameter(list, "@log_text", s);

            return ExecuteNonQuery("usp_addToLog", list) == 1;
        }
    }
}