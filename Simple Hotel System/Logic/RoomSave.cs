using cinema_ticketing.Classes;
using MySql.Data.MySqlClient;
using Simple_Hotel_System.Models;
using System.Data;

namespace Simple_Hotel_System.Logic
{
    public class RoomSave
    {
        public static (bool bOk, string sMsg) SetRoom(RoomInfo room)
        {
            DataAccess db = new();
            string sSQL = "";
            try
            {
                sSQL = "INSERT INTO roomtable (Name, Type, Status, Price, PicUrl) VALUES (@name, @type, @status, @price, @picurl)";
                
                using(MySqlCommand cmd = new())
                {
                    cmd.Parameters.AddWithValue("@name", room.Name);
                    cmd.Parameters.AddWithValue("@type", room.Type);
                    cmd.Parameters.AddWithValue("@status", room.Status);
                    cmd.Parameters.AddWithValue("@price", room.Price);
                    cmd.Parameters.AddWithValue("@picurl", room.PicUrl);
                    cmd.CommandText = sSQL;

                    if (db.ExecuteNonQuery(cmd))
                    {
                        return (true, "Successfully Saved.");
                    }
                    else
                    {
                        return (false, "Failed to Save");
                    }
                }
            }
            catch(Exception ex)
            {
                return (false, "Failed to Save" + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
        }

        public static List<RoomInfo> GetRoomInfo()
        {
            DataAccess db = new();
            string sSQL = "";
            List<RoomInfo> room = new List<RoomInfo>();

            try
            {
                sSQL = "SELECT Id, Name, Type, Status, Price, PicUrl FROM roomtable " +
                    "WHERE isActive = 1";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    var result = db.ExecuteDataTable(cmd);

                    if(result.Item2.Rows.Count > 0)
                    {
                        foreach(DataRow row in result.Item2.Rows)
                        {
                            room.Add(new RoomInfo
                            {
                                Id = Convert.ToInt32(row["Id"]),
                                Name = row["Name"].ToString(),
                                Type = row["Type"].ToString(),
                                Status = row["Status"].ToString(),
                                Price = Convert.ToDecimal(row["Price"]),
                                PicUrl = row.Field<string>("PicUrl"),
                            });
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
            return room;
        }

        public static RoomInfo GetRoomById(int roomId)
        {
            DataAccess db = new();
            string sSQL = "";
            RoomInfo room = null;

            try
            {
                sSQL = "SELECT Id, Name, Type, Price, PicUrl FROM roomtable WHERE Id = @id";
                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@id", roomId);

                    var result = db.ExecuteDataTable(cmd);

                    if(result.Item2.Rows.Count > 0)
                    {
                        DataRow row = result.Item2.Rows[0];

                        room = new RoomInfo
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Type = row["Type"].ToString(),
                            Price = Convert.ToDecimal(row["Price"]),
                            PicUrl = row["PicUrl"].ToString()
                        };
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }

            return room;
        }

        public static (bool bOk, string sMsg) UpdateRoom (RoomInfo room)
        {
            DataAccess db = new();
            string sSQL = "";
            try
            {
                sSQL = "UPDATE roomtable SET Name = @name, Type = @type, Status = @status, Price = @price, PicUrl = @picurl WHERE Id = @id";
                
                using(MySqlCommand cmd = new())
                {
                    cmd.Parameters.AddWithValue("@name", room.Name);
                    cmd.Parameters.AddWithValue("@type", room.Type);
                    cmd.Parameters.AddWithValue("@status", room.Status);
                    cmd.Parameters.AddWithValue("@price", room.Price);
                    cmd.Parameters.AddWithValue("@picurl", room.PicUrl);
                    cmd.Parameters.AddWithValue("@id", room.Id);
                    cmd.CommandText = sSQL;
                    if (db.ExecuteNonQuery(cmd))
                    {
                        return (true, "Successfully Updated.");
                    }
                    else
                    {
                        return (false, "Failed to Update");
                    }
                }
            }
            catch(Exception ex)
            {
                return (false, "Failed to Update: " + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
        }

        public static (bool bOk, string sMsg) DeleteRoom(int id)
        {
            DataAccess db = new();
            string sSQL = "";

            try
            {
                sSQL = "UPDATE roomtable SET isActive = 0 WHERE Id = @id";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@id", id);

                    if (db.ExecuteNonQuery(cmd))
                        return (true, "Room deleted successfully.");
                    else
                        return (false, "Failed to delete room.");
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
