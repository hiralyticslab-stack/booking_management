using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace booking_mgmt.Models
{
    public class Dbhandle
    {
        SqlConnection con;
        SqlCommand cmd;
        public void Connection()
        {
            String constr = ConfigurationManager.ConnectionStrings["constr"].ToString();
            con = new SqlConnection(constr);
        }

        public bool add_cat(Category ctmodel)
        {
            Connection();
            cmd = new SqlCommand("add_cat", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Cat_Type", ctmodel.Cat_Type);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
                return true;
            else
                return false;
        }
        public bool add_movie(Movie mov_model)
        {
            Connection();
            cmd = new SqlCommand("add_movie", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Movie_name", mov_model.Movie_name);
            cmd.Parameters.AddWithValue("@Release_Date", mov_model.Release_Date);
            cmd.Parameters.AddWithValue("@Cat_ID", mov_model.Cat_ID);
            cmd.Parameters.AddWithValue("@Rate", mov_model.Rate);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if(i >= 1)
                return true;
            else return false;
        }
        public DataTable ddlquery(String catstr)
        {
            Connection();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();            
            SqlDataAdapter da = new SqlDataAdapter(catstr,con);
            con.Open();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public int u_login(User umodel)
        {
            Connection();
            SqlCommand cmd = new SqlCommand("user_login", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User_Name", umodel.User_Name);
            cmd.Parameters.AddWithValue("@User_password", umodel.User_Password);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Read();
                int id = Convert.ToInt32(dr["User_ID"]);                
                con.Close();
                return id;
            }
            else
            {
                con.Close();
                return 0;
            }
        }
        public List<Movie> GetMovies()
        {
            Connection();
            List<Movie> movList= new List<Movie>();
            SqlCommand cmd = new SqlCommand("movie_display", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Movie mov = new Movie();
                mov.Cat_ID = (int)Convert.ToInt64(dr["Cat_ID"]);
                mov.Movie_ID =(int)dr["Movie_ID"];
                mov.Movie_name = Convert.ToString(dr["Movie_name"]);
                mov.Release_Date =(DateTime) dr["Release_Date"];
                mov.Rate = (int)dr["Rate"];
                movList.Add(mov);
            }
            con.Close();
            return movList;
        }
        public List<Category> GetCat()
        {
            Connection();
            List<Category> catList = new List<Category>();
            SqlCommand cmd = new SqlCommand("category_display", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Category cat = new Category();
                cat.Cat_ID = (int)dr["Cat_ID"];
                cat.Cat_Type = Convert.ToString(dr["Cat_Type"]);
                catList.Add(cat);
            }
            con.Close();
            return catList;
        }

        public List<Booking> GetBookings()
        {
            Connection();
            List<Booking> bookingList = new List<Booking>();

            // SQL Join to get readable Names and Rate instead of raw IDs
            string query = @"SELECT b.Booking_ID, b.User_ID, b.Cat_ID, b.Movie_ID, 
                            b.No_of_tickets, b.Amount, 
                            c.Cat_Type, m.Movie_name, m.Rate 
                     FROM Tbl_Booking b
                     INNER JOIN Tbl_Movie_Category c ON b.Cat_ID = c.Cat_ID
                     INNER JOIN Tbl_Movie m ON b.Movie_ID = m.Movie_ID";

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Booking bk = new Booking
                {
                    Booking_ID = Convert.ToInt32(dr["Booking_ID"]),
                    User_ID = Convert.ToInt32(dr["User_ID"]),
                    Cat_ID = Convert.ToInt32(dr["Cat_ID"]),
                    Movie_ID = Convert.ToInt32(dr["Movie_ID"]),
                    No_of_tickets = Convert.ToInt32(dr["No_of_tickets"]),
                    Amount = Convert.ToInt32(dr["Amount"]),
                    // Populating display values
                    Cat_Type = Convert.ToString(dr["Cat_Type"]),
                    Movie_name = Convert.ToString(dr["Movie_name"]),
                    Rate = Convert.ToInt32(dr["Rate"])
                };
                bookingList.Add(bk);
            }
            con.Close();
            return bookingList;
        }


        public bool editUser(int uid, User umodel)
        {
            Connection();

            DataSet ds = new DataSet();
            //DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("edit_user", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User_ID", uid);
            cmd.Parameters.AddWithValue("@User_Name", umodel.User_Name);
            cmd.Parameters.AddWithValue("@Email_ID", umodel.Email_ID);
            cmd.Parameters.AddWithValue("@User_password", umodel.User_Password);
            cmd.Parameters.AddWithValue("@City", umodel.City);
            cmd.Parameters.AddWithValue("@PhoneNo", umodel.PhoneNo);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
                return true;
            else 
                return false;
            //SqlDataAdapter da = new SqlDataAdapter(cmd);            
            //da.Fill(dt);
            
            //return dt;
        }
        public bool del_movie(int id)
        {
            Connection();
            SqlCommand cmd= new SqlCommand("DELETE FROM Tbl_Movie WHERE Movie_ID =@id",con);
            cmd.Parameters.AddWithValue ("id", id);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
                return true;
            else
                return false;
        }

        public int calRate(int movid) {
            Connection();
            SqlCommand cmd = new SqlCommand("SELECT Rate FROM Tbl_Movie WHERE Movie_ID=@movid", con);
            cmd.Parameters.AddWithValue("@movid", movid);
            con.Open();
            object result = cmd.ExecuteScalar(); // Correct method for SELECT returning single value
            con.Close();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }
        public bool addBooking(Booking bkmodel, int uid, int amt)
        {
            Connection();
            cmd = new SqlCommand("add_booking", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User_ID", uid);
            cmd.Parameters.AddWithValue("@Cat_ID", bkmodel.Cat_ID);
            cmd.Parameters.AddWithValue("@Movie_ID", bkmodel.Movie_ID);
            cmd.Parameters.AddWithValue("@No_of_tickets",bkmodel.No_of_tickets);
            cmd.Parameters.AddWithValue("@Amount",amt);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
                return true;
            else
                return false;
        }
        // 1. Fetch a single booking record by ID
        public Booking GetBookingByID(int id)
        {
            Connection();
            Booking bk = null;
            string query = "SELECT Booking_ID, User_ID, Cat_ID, Movie_ID, No_of_tickets, Amount FROM Tbl_Booking WHERE Booking_ID = @Booking_ID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Booking_ID", id);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                bk = new Booking
                {
                    Booking_ID = Convert.ToInt32(dr["Booking_ID"]),
                    User_ID = Convert.ToInt32(dr["User_ID"]),
                    Cat_ID = Convert.ToInt32(dr["Cat_ID"]),
                    Movie_ID = Convert.ToInt32(dr["Movie_ID"]),
                    No_of_tickets = Convert.ToInt32(dr["No_of_tickets"]),
                    Amount = Convert.ToInt32(dr["Amount"])
                };
            }
            con.Close();
            return bk;
        }

        // 2. Update existing booking in Database
        public bool UpdateBooking(Booking bk)
        {
            Connection();
            string query = @"UPDATE Tbl_Booking 
                    SET Cat_ID = @Cat_ID, Movie_ID = @Movie_ID, No_of_tickets = @No_of_tickets, Amount = @Amount 
                    WHERE Booking_ID = @Booking_ID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Cat_ID", bk.Cat_ID);
            cmd.Parameters.AddWithValue("@Movie_ID", bk.Movie_ID);
            cmd.Parameters.AddWithValue("@No_of_tickets", bk.No_of_tickets);
            cmd.Parameters.AddWithValue("@Amount", bk.Amount);
            cmd.Parameters.AddWithValue("@Booking_ID", bk.Booking_ID);

            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i >= 1;
        }
        public List<Booking> GetBookingsByCategory(int? catId)
        {
            Connection();
            List<Booking> bookingList = new List<Booking>();

            string query = @"SELECT b.Booking_ID, b.User_ID, b.Cat_ID, b.Movie_ID, 
                            b.No_of_tickets, b.Amount, 
                            c.Cat_Type, m.Movie_name, m.Rate 
                     FROM Tbl_Booking b
                     INNER JOIN Tbl_Movie_Category c ON b.Cat_ID = c.Cat_ID
                     INNER JOIN Tbl_Movie m ON b.Movie_ID = m.Movie_ID";

            // Append WHERE clause if a specific category is selected
            if (catId.HasValue && catId.Value > 0)
            {
                query += " WHERE b.Cat_ID = " + catId.Value;
            }

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Booking bk = new Booking
                {
                    Booking_ID = Convert.ToInt32(dr["Booking_ID"]),
                    User_ID = Convert.ToInt32(dr["User_ID"]),
                    Cat_ID = Convert.ToInt32(dr["Cat_ID"]),
                    Movie_ID = Convert.ToInt32(dr["Movie_ID"]),
                    No_of_tickets = Convert.ToInt32(dr["No_of_tickets"]),
                    Amount = Convert.ToInt32(dr["Amount"]),
                    Cat_Type = Convert.ToString(dr["Cat_Type"]),
                    Movie_name = Convert.ToString(dr["Movie_name"]),
                    Rate = Convert.ToInt32(dr["Rate"])
                };
                bookingList.Add(bk);
            }
            con.Close();
            return bookingList;
        }
    }
}