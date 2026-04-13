using cinema_ticketing.Classes;
using Microsoft.AspNetCore.Http.Features;
using MySql.Data.MySqlClient;
using Simple_Hotel_System.Models;
using System;
using System.Data;

namespace Simple_Hotel_System.Logic
{
    public class BookingSave
    {
        public static(bool bOk, string sMsg, int newId) SetBooking(BookingInfo booking)
        {
            DataAccess db = new();
            string sSQL = "";
            try
            {
                sSQL = "INSERT INTO bookingtable (GuestId, RoomId, CheckIn, CheckOut, Nights, TotalPrice) " +
                    "VALUES (@guestid, @roomid, @checkin, @checkout, @nights, @totalprice)";

                using (MySqlCommand cmd = new())
                {
                    cmd.Parameters.AddWithValue("@guestid", booking.GuestId);
                    cmd.Parameters.AddWithValue("@roomid", booking.RoomId);
                    cmd.Parameters.AddWithValue("@checkin", booking.CheckIn);
                    cmd.Parameters.AddWithValue("@checkout", booking.CheckOut);
                    cmd.Parameters.AddWithValue("@nights", booking.Nights);
                    cmd.Parameters.AddWithValue("@totalprice", booking.TotalPrice);
                    cmd.CommandText = sSQL;

                    if (db.ExecuteNonQuery(cmd))
                    {
                        int newId = (int)cmd.LastInsertedId;
                        return (true, "Booking saved successfully.", newId);
                    }
                    else
                    {
                        return (false, "Failed to Save the booking", 0);
                    }
                }

            }
            catch (Exception ex)
            {
                return(false, "Failed to Save " + ex.Message, 0);
            }
            finally
            {
                db.Close();
                db = null;
            }
        }
        public static List<CheckoutItemDto> GetCheckoutItemdByGuestId(int guestId)
        {
            DataAccess db = new();
            string sSQL = "";
            List<CheckoutItemDto> items = new List<CheckoutItemDto>();

            try
            {
                sSQL = "SELECT b.Id, r.Name AS RoomName, r.Type AS RoomType, b.Nights, b.TotalPrice " +
                    "FROM bookingtable b " +
                    "LEFT JOIN roomtable r ON b.RoomId = r.Id " +
                    "WHERE b.GuestId = @guestId";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@guestId", guestId);

                    var result = db.ExecuteDataTable(cmd);

                    foreach(DataRow row in result.Item2.Rows)
                    {
                        items.Add(new CheckoutItemDto
                        {
                            BookingId = Convert.ToInt32(row["Id"]),
                            RoomName = row["RoomName"].ToString() ?? "",
                            RoomType = row["RoomType"].ToString() ?? "",
                            Nights = Convert.ToInt32(row["Nights"]),
                            TotalPrice = Convert.ToDecimal(row["TotalPrice"])
                        });
                    }                
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
            return items;
        }
        public static BookingInfo GetBookingInfoById(int id)
        {
            DataAccess db = new();
            string sSQL = "";
            BookingInfo booking = null;

            try
            {
                sSQL = "SELECT Id, GuestId, RoomId, CheckIn, CheckOut, Nights, TotalPrice " +
                    "FROM bookingtable WHERE Id = @id";
                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@id", id);

                    var result = db.ExecuteDataTable(cmd);
                    if (result.Item2.Rows.Count > 0)
                    {
                        DataRow row = result.Item2.Rows[0];

                        booking = new BookingInfo
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            GuestId = Convert.ToInt32(row["GuestId"]),
                            RoomId = Convert.ToInt32(row["RoomId"]),
                            CheckIn = Convert.ToDateTime(row["CheckIn"]),
                            CheckOut = Convert.ToDateTime(row["CheckOut"]),
                            Nights = Convert.ToInt32(row["Nights"]),
                            TotalPrice = Convert.ToDecimal(row["TotalPrice"])
                        };
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
            return booking;
        }

        public static List<BookingInfo> GetBookingInfo()
        {
            DataAccess db = new();
            string sSQL = "";
            List<BookingInfo> booking = new List<BookingInfo>();
            try
            {
                sSQL = "SELECT Id, GuestId, RoomId, CheckIn, CheckOut, Nights, TotalPrice FROM bookingtable";
                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    
                    var result = db.ExecuteDataTable(cmd);
                    if(result.Item2.Rows.Count > 0)
                    {
                        foreach (DataRow row in result.Item2.Rows)
                        {
                            booking.Add(new BookingInfo
                            {
                                Id = Convert.ToInt32(row["Id"]),
                                GuestId = Convert.ToInt32(row["GuestId"]),
                                RoomId = Convert.ToInt32(row["RoomId"]),
                                CheckIn = Convert.ToDateTime(row["CheckIn"]),
                                CheckOut = Convert.ToDateTime(row["CheckOut"]),
                                Nights = Convert.ToInt32(row["Nights"]),
                                TotalPrice = Convert.ToDecimal(row["TotalPrice"])
                            });                 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
            return booking;
        }

        public static ReceiptInfo GetReceiptByBookingId(int id)
        {
            DataAccess db = new();
            string sSQL = "";
            ReceiptInfo receipt = null;

            try
            {
                sSQL = "SELECT b.Id AS BookingId, b.RoomId AS BookingRoomId, b.GuestId AS BookingGuestId, b.CheckIn, b.CheckOut, b.Nights, b.TotalPrice, " +
                    "r.Id AS RoomId, r.Type, r.Price, g.Id AS GuestId, g.Name " +
                    "FROM bookingtable b " +
                    "LEFT JOIN roomtable r ON b.RoomId = r.Id " +
                    "LEFT JOIN guesttable g ON b.GuestId = g.Id " +
                    "WHERE b.Id = @id";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@id", id);

                    var result = db.ExecuteDataTable(cmd);
                    if(result.Item2.Rows.Count > 0)
                    {
                        DataRow row = result.Item2.Rows[0];
                        receipt = new ReceiptInfo
                        {
                            Booking = new BookingInfo
                            {
                                Id = Convert.ToInt32(row["BookingId"]),
                                GuestId = Convert.ToInt32(row["BookingGuestId"]),
                                RoomId = Convert.ToInt32(row["BookingRoomId"]),
                                CheckIn = row["CheckIn"] != DBNull.Value ? Convert.ToDateTime(row["CheckIn"]) : DateTime.MinValue,
                                CheckOut = row["CheckOut"] != DBNull.Value ? Convert.ToDateTime(row["CheckOut"]) : DateTime.MinValue,
                                Nights = Convert.ToInt32(row["Nights"]),
                                TotalPrice = Convert.ToDecimal(row["TotalPrice"])
                            },
                            Room = new RoomInfo
                            {
                                Id = Convert.ToInt32(row["RoomId"]),
                                Type = row["Type"].ToString() ?? "",
                                Price = Convert.ToDecimal(row["Price"])
                            },
                            Guest = new GuestInfo
                            {
                                Id = Convert.ToInt32(row["GuestId"]),
                                Name = row["Name"].ToString() ?? ""
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }
            return receipt;
        }

        public static List<BookingInfo> GetBookedDates (int roomId)
        {
            DataAccess db = new();
            string sSQL = "";
            List<BookingInfo> bookings = new List<BookingInfo>();

            try
            {
                sSQL = "SELECT CheckIn, CheckOut FROM bookingtable"
                    + " WHERE RoomId = @roomId";

                using (MySqlCommand cmd = new())
                {
                    cmd.CommandText = sSQL;
                    cmd.Parameters.AddWithValue("@roomId", roomId);

                    var result = db.ExecuteDataTable(cmd);
                    foreach(DataRow row in result.Item2.Rows)
                    {
                        bookings.Add(new BookingInfo
                        {
                            CheckIn = Convert.ToDateTime(row["CheckIn"]),
                            CheckOut = Convert.ToDateTime(row["CheckOut"])
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                db.Close();
                db = null;
            }

            return bookings;
        }
    }
}
