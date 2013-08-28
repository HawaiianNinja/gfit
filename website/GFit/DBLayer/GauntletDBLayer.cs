#region

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using gFit.Models.Base;
using gFit.Models;

#endregion

namespace gFit.DBLayer
{
    public class GauntletDBLayer : DBLayer
    {
        private static GauntletDBLayer instance;


        private GauntletDBLayer()
        {
        }

        public static GauntletDBLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GauntletDBLayer();
                }
                return instance;
            }
            private set { }
        }


        public List<Gauntlet> GetTodaysGauntlets()
        {
            var dt = GetDataTableFromStoredProcedure("usp_getGauntletByDate", new List<SqlParameter>());

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return GetGauntletsFromDataTable(dt);
        }



        public Gauntlet GetGauntlet(int gauntlet_id)
        {
            var sqlParameters = new List<SqlParameter>();

            AddSqlParameter(sqlParameters, "@gauntlet_id", gauntlet_id);

            var dt = GetDataTableFromStoredProcedure("usp_getGauntletById", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            var g = GetGauntletsFromDataTable(dt)[0];

            return g;
        }


        private List<Gauntlet> GetGauntletsFromDataTable(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return new List<Gauntlet>();
            }


            var list = new List<Gauntlet>();

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var g = new Gauntlet
                            {
                                Id = (int) GetColValue(dt.Rows[i], "id"),
                                Excercise = (string) GetColValue(dt.Rows[i], "excercise"),
                                Reps = (int) GetColValue(dt.Rows[i], "reps"),
                                Difficulty = (int) GetColValue(dt.Rows[i], "difficulty"),
                                DateCreated = Convert.ToDateTime(GetColValue(dt.Rows[i], "date_created")),
                                DateAssigned = Convert.ToDateTime(GetColValue(dt.Rows[i], "date_assigned"))
                            };


                list.Add(g);
            }

            return list;
        }
    }
}