using cinema_ticketing.Classes;
using MySql.Data.MySqlClient;
using Simple_Hotel_System.Models;
using System.Data;

namespace Simple_Hotel_System.Logic
{
    public class AdminSave
    {
        public static (bool bOk, string sMsg) SetAdmin(AdminInfo admin)
        {
            DataAccess db = new();
            string sSQL = "";
            string hashedPassword = Hash.HashPassword(admin.Password);

            try
            {
                sSQL = "INSERT INTO admintable (Name, Password, PhoneNum, Email) VALUES (@name, @password, @phonenum, @email)";
                
                using(MySqlCommand cmd = new())
                {
                    cmd.Parameters.AddWithValue("@name", admin.Name);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@phonenum", admin.PhoneNum);
                    cmd.Parameters.AddWithValue("@email", admin.Email);
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
            catch (Exception ex)
            {
                return (false, "Failed to Save" + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }

        }
        public static (bool bOk, string sMsg, AdminInfo admin) GetAdmin(string Name, string Password)
        {
            DataAccess db = new();
            string checkSQL = "";
            
            try
            {
                checkSQL = "SELECT Name, Password, PhoneNum, Email FROM admintable WHERE Name = @name";


                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = checkSQL;
                    cmd.Parameters.AddWithValue("@name", Name);

                    var result = db.ExecuteDataTable(cmd);
                    if (result.Item2.Rows.Count > 0)
                    {
                        DataRow row = result.Item2.Rows[0];

                        string storedHash = row["Password"].ToString();

                        if (!Hash.VerifyPassword(Password, storedHash))
                            return (false, "Invalid password", null);

                        AdminInfo admin = new AdminInfo
                        {
                            Name = row["Name"].ToString(),
                            PhoneNum = row["PhoneNum"].ToString(),
                            Email = row["Email"].ToString()
                        };
                        return (true, "Login Successfully.", admin);
                    }
                    else
                    {
                        return (false, "User not found", null);
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
    }
}
