using cinema_ticketing.Classes;
using MySql.Data.MySqlClient;
using Simple_Hotel_System.Models;
using System.Data;
using System.Xml.Linq;

namespace Simple_Hotel_System.Logic
{
    public class SaveLogic
    {
        public static (bool bOk, string sMsg) SaveGuest(GuestInfo guest)
        {
            DataAccess db = new();
            string sSQL = "";

            string encryptedName = Crypto.Encrypt(guest.Name);
            string hashedPassword = Hash.HashPassword(guest.Password);

            try
            {
                sSQL = "INSERT INTO guesttable (Name, Password, Gender, PhoneNum, Email) VALUES (@name, @password, @gender, @phonenum, @email)";

                using (MySqlCommand cmd = new())
                {
                    cmd.Parameters.AddWithValue("@name", encryptedName);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@gender", guest.Gender);
                    cmd.Parameters.AddWithValue("@phonenum", guest.PhoneNum);
                    cmd.Parameters.AddWithValue("@email", guest.Email);
                    cmd.CommandText = sSQL;

                    if (db.ExecuteNonQuery(cmd))
                    {
                        return (true, "Successfully saved.");
                    }
                    else
                    {
                        return (false, "Failed to save");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, "Failed to save" + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
        }


        //Login purpose
        public static (bool bOk, string sMsg, GuestInfo guest) FindGuest(string Email, string Password)
        {
            DataAccess db = new();
            string sSQL = "";

            try
            {
                sSQL = "SELECT Id, Name, Password, Gender, PhoneNum, Email  FROM guesttable WHERE Email = @email";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@email", Email);


                    var result = db.ExecuteDataTable(cmd);
                    if (result.Item2.Rows.Count > 0)
                    {
                        DataRow row = result.Item2.Rows[0];

                        string storedHash = row["Password"].ToString();

                        if (!Hash.VerifyPassword(Password, storedHash))
                            return (false, "Invalid password", null);

                        string decryptedName;
                        try
                        {
                            decryptedName = Crypto.Decrypt(row["Name"].ToString());
                        }
                        catch
                        {
                            decryptedName = row["Name"].ToString();
                        }


                        GuestInfo guest = new GuestInfo
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = decryptedName,
                            Gender = (Gender)Convert.ToInt32(row["Gender"]),
                            PhoneNum = row["PhoneNum"].ToString(),
                            Email = row["Email"].ToString()
                        };
                        return (true, "Login Successfully.", guest);
                    }
                    else
                    {
                        return (false, "User not found", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, "Error: " + ex.Message, null);
            }
            finally
            {
                db.Close();
                db = null;
            }
        }

        //Update
        public static (bool bOk, string sMsg) UpdateGuest(GuestInfo guest)
        {
            DataAccess db = new();
            string sSQL = "";

            try
            {
                sSQL = "UPDATE guesttable SET Name = @name, Gender = @gender, PhoneNum = @phonenum, Email = @email WHERE Id = @id";

                using (MySqlCommand cmd = new())
                {
                    cmd.Parameters.AddWithValue("@id", guest.Id);
                    cmd.Parameters.AddWithValue("@name", guest.Name);
                    cmd.Parameters.AddWithValue("@gender", guest.Gender);
                    cmd.Parameters.AddWithValue("@phonenum", guest.PhoneNum);
                    cmd.Parameters.AddWithValue("@email", guest.Email);
                    cmd.CommandText = sSQL;

                    if (db.ExecuteNonQuery(cmd))
                        return (true, "Updated successfully.");
                    else
                        return (false, "Guest not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Update failed: " + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
        }

        public static (bool bOk, string sMsg, DataTable dt) GetGuest()
        {
            DataAccess db = new();
            string sSQL = "";

            try
            {
                sSQL = "SELECT Id, Name, Gender, PhoneNum, Email FROM guesttable";


                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;

                    var result = db.ExecuteDataTable(cmd);

                    if (result.Item2.Rows.Count > 0)
                    {
                        return (true, "Login Successfully.", result.Item2);
                    }
                    else
                    {
                        return (false, "User not found", result.Item2);
                    }
                }

            }
            catch (Exception ex)
            {
                return (false, "Failed to Login" + ex.Message, null);
            }
            finally
            {
                db.Close();
                db = null;
            }

        }


        public static List<GuestInfo> GetGuestInfo()
        {
            DataAccess db = new();
            string sSQL = "";
            List<GuestInfo> guest = new List<GuestInfo>();
            try
            {
                sSQL = "SELECT Id, Name, Gender, PhoneNum, Email FROM guesttable ";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    var result = db.ExecuteDataTable(cmd);

                    if (result.Item2.Rows.Count > 0)
                    {
                        foreach (DataRow row in result.Item2.Rows)
                        {
                            guest.Add(new GuestInfo
                            {
                                Id = Convert.ToInt32(row["Id"]?? 0),
                                Name = row["Name"]?.ToString() ?? "",
                                Gender = (Gender)Convert.ToInt32(row["Gender"]),
                                PhoneNum = row["PhoneNum"]?.ToString() ?? "",
                                Email = row["Email"]?.ToString() ?? ""
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

        public static GuestInfo GetGuestById(int id)
        {
            DataAccess db = new();
            string sSQL = "";
            GuestInfo guest = null;

            try
            {
                sSQL = "SELECT Id, Name, Password, Gender, PhoneNum, Email FROM guesttable WHERE Id = @id";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@id", id);

                    var result = db.ExecuteDataTable(cmd);
                    if (result.Item2.Rows.Count > 0)
                    {
                        DataRow row = result.Item2.Rows[0];

                        guest = new GuestInfo
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Gender = (Gender)Convert.ToInt32(row["Gender"]),
                            PhoneNum = row["PhoneNum"].ToString(),
                            Email = row["Email"].ToString(),
                            Password = row["Password"].ToString()
                        };
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

        public static (bool bOk, string sMsg) DeleteGuest(int id)
        {
            DataAccess db = new();
            string sSQL = "";
            try
            {
                sSQL = "DELETE FROM guesttable WHERE Id = @id";
                using (MySqlCommand cmd = new())
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = sSQL;
                    if (db.ExecuteNonQuery(cmd))
                    {
                        return (true, "Successfully deleted.");
                    }
                    else
                    {
                        return (false, "Failed to delete");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, "Failed to delete" + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
        }
    }
}
