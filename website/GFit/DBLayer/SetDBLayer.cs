#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using gFit.Models.Base;

#endregion

namespace gFit.DBLayer
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


        public Set GetSetByGuid(Guid setGuid)
        {
            var sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@set_guid", setGuid.ToString());

            var dt = GetDataTableFromStoredProcedure("dbo.usp_getSetById", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            var gs = GetSetsFromDataTable(dt)[0];

            return gs;
        }


        public bool StoreCompletedSet(Set s)
        {
            var sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@set_guid", s.Guid);
            AddSqlParameter(sqlParameters, "@num_reps", s.NumReps);

            var result = ExecuteNonQuery("dbo.usp_storeCompletedSet", sqlParameters);

            if (result > 1)
            {
                throw new DataException("Altered more than one row when row identity should be unique");
            }

            return result == 1;
        }


        public bool DeleteSet(Set s)
        {
            var sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@set_guid", s.Guid.ToString());

            var result = ExecuteNonQuery("dbo.usp_deleteSet", sqlParameters);

            if (result > 1)
            {
                throw new DataException("Altered more than one row when row identity should be unique");
            }

            return result == 1;
        }





        public List<Set> GetSetsByAccountAndGauntlet(int accountId, int gauntletId)
        {
            var sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@account_id", accountId);
            AddSqlParameter(sqlParameters, "@gauntlet_id", gauntletId);

            var dt = GetDataTableFromStoredProcedure("dbo.usp_getSetsByAccountAndGauntlet", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return new List<Set>();
            }

            return GetSetsFromDataTable(dt);
        }


        public Set GetNewSet(int accountId, int gauntletId)
        {
            var sqlParameters = new List<SqlParameter>();
            AddSqlParameter(sqlParameters, "@account_id", accountId);
            AddSqlParameter(sqlParameters, "@gauntlet_id", gauntletId);

            var dt = GetDataTableFromStoredProcedure("dbo.usp_newSet", sqlParameters);

            return GetSetsFromDataTable(dt)[0];
        }


        public List<Set> GetSetsFromDataTable(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return new List<Set>();
            }

            var list = new List<Set>();

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var gs = new Set
                             {
                                 Guid = (Guid) GetColValue(dt.Rows[i], "set_guid"),
                                 AccountId = (int) GetColValue(dt.Rows[i], "account_id"),
                                 GauntletId = (int) GetColValue(dt.Rows[i], "gauntlet_id"),
                                 NumReps = (int) GetColValue(dt.Rows[i], "num_reps"),
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