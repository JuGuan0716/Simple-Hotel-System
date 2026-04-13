using cinema_ticketing.Classes;
using MySql.Data.MySqlClient;
using Simple_Hotel_System.Models;
using System.Data;

namespace Simple_Hotel_System.Logic
{
    public class DataTableSave
    {
        public static List<GuestInfo> GetGuest()
        {
            DataAccess db = new();
            List <GuestInfo> guest = new();
            string sSQL = "";

            try
            {
                sSQL = "SELECT * FROM guesttable ORDER BY Id ";
                using(MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    var result = db.ExecuteDataTable(cmd);
                    if(result.Item2.Rows.Count > 0)
                    {
                        foreach(DataRow row in result.Item2.Rows)
                        {
                            string decryptedName;
                            try
                            {
                                decryptedName = Crypto.Decrypt(row["Name"].ToString());
                            }
                            catch
                            {
                                decryptedName = row["Name"].ToString();
                            }
                            guest.Add(new GuestInfo
                            {
                                Id = Convert.ToInt32(row["Id"]),
                                Name = decryptedName,
                                Gender = (Gender)Convert.ToInt32(row["Gender"]),
                                PhoneNum = row["PhoneNum"].ToString(),
                                Email = row["Email"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
            return guest;
        }
    }
}
