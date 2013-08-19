using gSculpt.Models.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gSculpt.DBLayer
{
    public class SetDBLayer : DBLayer
    {


        private static SetDBLayer instance;

        public static SetDBLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SetDBLayer();
                }
                return instance;
            }
            private set { }
        }




        public Set GetSetById(int setId)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@set_id", setId);

            DataTable dt = GetDataTableFromStoredProcedure("usp_getSetById", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            Set gs = GetSetsFromDataTable(dt)[0];

            return gs;

        }


        
        public Set GetNewSet(int accountId, int gauntletId)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@account_id", accountId);
            AddSqlParameter(sqlParameters, "@gauntlet_id", gauntletId);


            return new Set();
        }


        public List<Set> GetSetsFromDataTable(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return new List<Set>();
            }

            List<Set> list = new List<Set>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                Set gs = new Set
                {
                    Id = (int)GetColValue(dt.Rows[i], "set_id"),
                    AccountId = (int)GetColValue(dt.Rows[i], "account_id"),
                    GauntletId = (int)GetColValue(dt.Rows[i], "gauntlet_id"),
                    NumReps = (int)GetColValue(dt.Rows[i], "num_reps"),
                    StartTime = Convert.ToDateTime(GetColValue(dt.Rows[i], "start_time")),
                    EndTime = Convert.ToDateTime(GetColValue(dt.Rows[i], "end_time")),
                    Completed = Convert.ToBoolean(GetColValue(dt.Rows[i], "completed"))
                };

                list.Add(gs);

            }
            return list;

        }



    }
}