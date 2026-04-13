using cinema_ticketing.Classes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace Simple_Hotel_System.Logic
{
    public class Delete
    {
        

        public static (bool bOk, string sMsg) DeleteAny(int bookingId)
        {
            DataAccess db = new();
            string sSQL = "";

            try
            {
                sSQL = "DELETE FROM bookingtable WHERE Id = @Id";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@Id", bookingId);

                    if (db.ExecuteNonQuery(cmd))
                    {
                        return (true, "Record deleted successfully.");
                    }
                    else 
                    {
                        return (false, "Failed to delete record: ");
                    }


                }
            }
            catch (Exception ex)
            {
                return (false, "Error: " + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
        }
    }
}
