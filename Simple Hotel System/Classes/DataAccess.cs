using MySql.Data.MySqlClient;
using System.Data;

namespace cinema_ticketing.Classes
{
    public enum DataBaseServer
    {
        DEFAULT = 0,
    }

    public class DataAccess
    {
        public MySqlConnection Con;

        public MySqlTransaction Trans;

        public bool IsTrans = false;
        public string ErrMsg = string.Empty;

        //construction
        public DataAccess(bool IsTransaction = false, DataBaseServer dataBaseServer = DataBaseServer.DEFAULT)
        {
            string connectString = Utility.GetConfiguration().GetSection("Data").GetSection("DefaultConnection").GetSection("ConnectionString").Value;

            //if (string.IsNullOrEmpty(connectString))
            //{
            //    connectString = Utility.GetConfiguration().GetSection("Data").GetSection("DefaultConnection").GetSection("ConnectionString").Value;
            //}

            Con = new MySqlConnection(connectString);
            Con.Open();

            if (IsTransaction)
            {
                IsTrans = true;
                Trans = Con.BeginTransaction();
            }
        }

        public void BeginTrans()
        {
            IsTrans = true;
            Trans = Con.BeginTransaction();
        }

        public bool ExecuteNonQuery(MySqlCommand Cmd)
        {
            Cmd.Connection = Con;
            if (IsTrans)
                Cmd.Transaction = Trans;
            _ = Cmd.ExecuteNonQuery();

            return true;
        }

        public bool ExecuteNonQuery(MySqlCommand Cmd, ref int iNumRecAffected)
        {
            Cmd.Connection = Con;
            if (IsTrans)
                Cmd.Transaction = Trans;
            iNumRecAffected = Cmd.ExecuteNonQuery();

            return true;
        }

        public bool ExecuteReader(ref MySqlDataReader dr, ref MySqlCommand cmd)
        {
            // Dim Cmd As Object
            // Try
            // cmd = getCommand()
            // Cmd.commandtext = sSQL
            // Cmd.CommandType = CommandType.Text
            if (IsTrans == true)
                cmd.Transaction = Trans;
            cmd.Connection = Con;
            dr = cmd.ExecuteReader(CommandBehavior.Default);
            return true;
        }

        public bool ExecuteDataTable(MySqlCommand cmd, ref System.Data.DataTable dt, ref int recCnt)
        {
            if (dt == null)
                dt = new System.Data.DataTable();
            else
                dt.Clear();

            cmd.Connection = Con;
            if (IsTrans == true)
                cmd.Transaction = Trans;
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
            {
                if (IsTrans)
                    adapter.SelectCommand.Transaction = Trans;

                adapter.Fill(dt);
                recCnt = dt.Rows.Count;
                return true;
            }
        }

        public (bool, DataTable, int) ExecuteDataTable(MySqlCommand cmd)
        {
            DataTable dt = new DataTable();

            cmd.Connection = Con;
            if (IsTrans == true)
                cmd.Transaction = Trans;
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
            {
                if (IsTrans)
                    adapter.SelectCommand.Transaction = Trans;

                adapter.Fill(dt);
                return (true, dt, dt.Rows.Count);
            }
        }

        public object ExecuteScalar(MySqlCommand cmd)
        {
            cmd.Connection = Con;

            if (IsTrans)
                cmd.Transaction = Trans;
            return cmd.ExecuteScalar();
        }

        public void CommitTrans()
        {
            Trans.Commit();
            //IsTrans = false;
        }

        public void RollBackTrans()
        {
            Trans.Rollback();
            //IsTrans = false;
        }

        public void Close()
        {
            if (Con != null)
                // Con.Dispose()
                Con.Close();
            Con = null;
        }

        /// <summary>
        /// Static Functions !!!
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>

        //public static (bool, List<string>, List<string>) DatabaseCompare(string connectionString1, string connectionString2)
        //{
        //    List<string> arrMissTable = new List<string>();
        //    List<string> arrMissField = new List<string>();
        //    DataAccess db1 = new DataAccess(false, connectionString1);
        //    DataAccess db2 = new DataAccess(false, connectionString2);

        //    (bool valid1, List<string> arrTables1) = getDatabaseTables(ref db1);
        //    (bool valid2, List<string> arrTables2) = getDatabaseTables(ref db2);
        //    if (valid1 == false || valid2 == false)
        //    {
        //        return (false, null,null);
        //    }
        //    else
        //    {
        //        //valid process
        //        for (int i=0; i<arrTables1.Count; i++)
        //        {
        //            bool found = false;
        //            for (int j=0; j<arrTables2.Count; j++)
        //            {
        //                if (arrTables1[i] == arrTables2[j])
        //                {
        //                    found = true;
        //                    //check fiedls
        //                    (bool tableValid1, List<string> arrField1) = getDatabaseTableFields(ref db1, arrTables1[i]);
        //                    (bool tableValid2, List<string> arrField2) = getDatabaseTableFields(ref db2, arrTables1[i]);
        //                    if (tableValid1 == false || tableValid2 == false)
        //                    {
        //                        //not possible to reach here
        //                        return (false, null,null);
        //                    }
        //                    else
        //                    {
        //                        bool foundField = false;
        //                        for (int k=0; k<arrField1.Count; k++)
        //                        {
        //                            foundField = false;
        //                            for (int l=0; l<arrField2.Count; l++)
        //                            {
        //                                if (arrField1[k] == arrField2[l])
        //                                {
        //                                    foundField = true;
        //                                    break;
        //                                }
        //                            }
        //                            if (foundField == false)
        //                            {
        //                                //arrMissField.Add("Field in Database 1, Table " + arrTables1[i] + ", Field " + arrField1[k] + ", not found in Database 2");
        //                                arrMissField.Add(arrTables1[i] + "." + arrField1[k]);
        //                            }
        //                        }
        //                    }
        //                    break;
        //                }
        //            }
        //            if (found == false)
        //            {
        //                //table in DB1 not found in DB2
        //                //arrMissTable.Add("Table in Database 1, not found in Database 2 = " + arrTables1[i]);
        //                arrMissTable.Add(arrTables1[i]);
        //            }
        //        }
        //        return (true,arrMissTable,arrMissField);
        //    }
        //    //return (false,null);
        //}

        private static (bool, List<string>) GetDatabaseTables(ref DataAccess db)
        {
            //string sql = "SELECT TABLE_NAME FROM information_schema.tables WHERE table_schema = '" + db.Con.Database + "'";
            string sql = "SELECT TABLE_NAME FROM information_schema.tables WHERE table_schema = @databaseName";
            try
            {
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("databaseName", db.Con.Database);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    int cnt = 0;
                    if (db.ExecuteDataTable(cmd, ref dt, ref cnt))
                    {
                        if (cnt > 0)
                        {
                            List<string> arr = new List<string>();
                            for (int i = 0; i < cnt - 1; i++)
                            {
                                arr.Add(dt.Rows[i]["TABLE_NAME"].ToString());
                            }
                            return (true, arr);
                        }
                    }
                }
                return (false, null);
            }
            catch (System.Exception e)
            {
                Logging.Write(Constant.APP_NAME, e.ToString(), System.Reflection.MethodBase.GetCurrentMethod().ToString(), sql);
                return (false, null);
            }
            finally
            {
                //if (db != null)
                //{
                //    db.Close();
                //}
                //db = null;
            }
        }

        private static (bool, List<string>) GetDatabaseTableFields(ref DataAccess db, string tableName)
        {
            //string sql = "SELECT TABLE_NAME FROM information_schema.tables WHERE table_schema = @databaseName";
            string sql = "SELECT COLUMN_NAME  FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_SCHEMA = @databaseName AND TABLE_NAME = @tableName";
            try
            {
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("databaseName", db.Con.Database);
                    cmd.Parameters.AddWithValue("tableName", tableName);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    int cnt = 0;
                    if (db.ExecuteDataTable(cmd, ref dt, ref cnt))
                    {
                        if (cnt > 0)
                        {
                            List<string> arr = new List<string>();
                            for (int i = 0; i < cnt; i++)
                            {
                                arr.Add(dt.Rows[i]["COLUMN_NAME"].ToString());
                            }
                            return (true, arr);
                        }
                    }
                }
                return (false, null);
            }
            catch (System.Exception e)
            {
                Logging.Write(Constant.APP_NAME, e.ToString(), System.Reflection.MethodBase.GetCurrentMethod().ToString(), sql);
                return (false, null);
            }
            finally
            {
                //if (db != null)
                //{
                //    db.Close();
                //}
                //db = null;
            }
        }

        private static (bool, List<string>) GetDatabaseTableFields(DataTable dt)
        {
            if (dt.Columns.Count > 0)
            {
                List<string> arr = new List<string>();
                foreach (DataColumn column in dt.Columns)
                {
                    arr.Add(column.ColumnName);
                }
                return (true, arr);
            }
            return (false, null);
        }
    }
}