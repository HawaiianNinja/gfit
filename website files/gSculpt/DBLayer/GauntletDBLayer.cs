using gSculpt.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gSculpt.DBLayer
{
    public class GauntletDBLayer : DBLayer
    {

        private static GauntletDBLayer instance;

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




        private GauntletDBLayer()
            : base()
        {
        }




        public List<Gauntlet> GetTodaysGauntletsFromDB()
        {



            DataTable dt = GetDataTableFromStoredProcedure("usp_getGauntletByDate", new List<SqlParameter>());

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            return GetGauntletsFromDataTable(dt);

        }





        public Gauntlet GetGauntletFromDB(int gauntlet_id){

            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            AddSqlParameter(sqlParameters, "@gauntlet_id", gauntlet_id);

            DataTable dt = GetDataTableFromStoredProcedure("usp_getGauntletById", sqlParameters);

            if (dt.Rows.Count == 0)
            {
                return null;
            }

            Gauntlet g = GetGauntletsFromDataTable(dt)[0];

            return g;

        }



        public List<Gauntlet> GetGauntletsFromDataTable(DataTable dt)
        {

            if (dt.Rows.Count == 0)
            {
                return new List<Gauntlet>();
            }


            List<Gauntlet> list = new List<Gauntlet>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                Gauntlet g = new Gauntlet
                {
                    Id = (int)GetColValue(dt.Rows[i], "id"),
                    Excercise = (string)GetColValue(dt.Rows[i], "excercise"),
                    Reps = (int)GetColValue(dt.Rows[i], "reps"),
                    Difficulty = (int)GetColValue(dt.Rows[i], "difficulty"),
                    DateCreated = Convert.ToDateTime(GetColValue(dt.Rows[i], "date_created")),
                    DateAssigned = Convert.ToDateTime(GetColValue(dt.Rows[i], "date_assigned"))

                };


                list.Add(g);

            }

            return list;

        }






    }
}