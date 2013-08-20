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

            DataTable dt = GetDataTableFromStoredProcedure("dbo.usp_getSetById", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            Set gs = GetSetsFromDataTable(dt)[0];

            return gs;

        }


        public bool StoreCompletedSet(Set s)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@set_id", s.Id);
            AddSqlParameter(sqlParameters, "@account_id", s.AccountId);
            AddSqlParameter(sqlParameters, "@gauntlet_id", s.GauntletId);
            AddSqlParameter(sqlParameters, "@num_reps", s.NumReps);

            int result = ExecuteNonQuery("dbo.usp_storeCompletedSet", sqlParameters);

            if(result > 1)
            {
                throw new DataException("Altered more than one row when row identity should be unique");
            }

            return result == 1;

        }



        public List<Set> GetSetsByAccountAndGauntlet(int Id, int gauntletId)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@account_id", Id);
            AddSqlParameter(sqlParameters, "@gauntlet_id", gauntletId);

            DataTable dt = GetDataTableFromStoredProcedure("dbo.usp_getGauntletSetsByAccount", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return new List<Set>();
            }

            return GetSetsFromDataTable(dt);

        }


        
        public Set GetNewSet(int Id, int gauntletId)
        {

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@account_id", Id);
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