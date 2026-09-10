using BloodBankManagementSystem.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BloodBankManagementSystem.DAL
{
    class userDAL
    {
        // Create a Static String to Connect Database
        static string myconnstrng = ConfigurationManager.ConnectionStrings["connstrng"].ConnectionString;

        #region SELECT data from database
        public DataTable Select()
        {
            // Create an Object to connect database
            SqlConnection conn = new SqlConnection(myconnstrng);

            // Create a DataTable to Hold the Data from Database
            DataTable dt = new DataTable();

            try
            {
                // Write SQL Query to Get Data from Database
                string sql = "SELECT * FROM tbl_users";

                // Create SQL Command to Execute Query
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Create Sql Data Adapter to hold the data from database temporarily
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                // Open Database Connection
                conn.Open();

                // Transfer Data from SqlDataAdapter to DataTable
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                // Display Error Message if there's any exceptional errors
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close Database Connection
                conn.Close();
            }

            return dt;
        }
        #endregion

        #region Insert Data into Database for User Module
        public bool Insert(userBLL u)
        {
            // Create a boolean variable and set its default value to false
            bool isSuccess = false;

            // Create an Object of SqlConnection to connect Database
            SqlConnection conn = new SqlConnection(myconnstrng);

            try
            {
                // Create a String Variable to Store the INSERT Query using first_name and last_name
                string sql = "INSERT INTO tbl_users(first_name, last_name, email, username, password, contact, address, added_date, image_name) VALUES (@first_name, @last_name, @email, @username, @password, @contact, @address, @added_date, @image_name)";

                // Create a SQL Command to pass the value in our query
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Create the Parameters to pass values from UI to SQL Query
                cmd.Parameters.AddWithValue("@first_name", u.first_name);
                cmd.Parameters.AddWithValue("@last_name", u.last_name);
                cmd.Parameters.AddWithValue("@email", u.email);
                cmd.Parameters.AddWithValue("@username", u.username);
                cmd.Parameters.AddWithValue("@password", u.password);
                cmd.Parameters.AddWithValue("@contact", u.contact);
                cmd.Parameters.AddWithValue("@address", u.address);
                cmd.Parameters.AddWithValue("@added_date", u.added_date);
                cmd.Parameters.AddWithValue("@image_name", u.image_name);

                // Open Database Connection
                conn.Open();

                // Create an Integer Variable to hold the value after the query is executed
                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    // Query Executed Successfully
                    isSuccess = true;
                }
                else
                {
                    // Failed to Execute Query
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                // Display Error Message if there's any exceptional error
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close Database Connection
                conn.Close();
            }

            return isSuccess;
        }
        #endregion

        #region UPDATE data in database (User Module)
        public bool Update(userBLL u)
        {
            // Create a Boolean variable and set its default value to false
            bool isSuccess = false;

            // Create an Object for Database Connection
            SqlConnection conn = new SqlConnection(myconnstrng);

            try
            {
                // Create a string variable to hold the sql query
                string sql = "UPDATE tbl_users SET first_name=@first_name, last_name=@last_name, email=@email, username=@username, password=@password, contact=@contact, address=@address, added_date=@added_date, image_name=@image_name WHERE user_id=@user_id";

                // Create Sql Command to execute query
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Pass the values to SQL Query Parameters
                cmd.Parameters.AddWithValue("@first_name", u.first_name);
                cmd.Parameters.AddWithValue("@last_name", u.last_name);
                cmd.Parameters.AddWithValue("@email", u.email);
                cmd.Parameters.AddWithValue("@username", u.username);
                cmd.Parameters.AddWithValue("@password", u.password);
                cmd.Parameters.AddWithValue("@contact", u.contact);
                cmd.Parameters.AddWithValue("@address", u.address);
                cmd.Parameters.AddWithValue("@added_date", u.added_date);
                cmd.Parameters.AddWithValue("@image_name", u.image_name);
                cmd.Parameters.AddWithValue("@user_id", u.user_id);

                // Open Database Connection
                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isSuccess;
        }
        #endregion

        #region Delete Data from Database (User Module)
        public bool Delete(userBLL u)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(myconnstrng);

            try
            {
                string sql = "DELETE FROM tbl_users WHERE user_id=@user_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@user_id", u.user_id);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isSuccess;
        }
        #endregion

        #region SEARCH
        public DataTable Search(string keywords)
        {
            SqlConnection conn = new SqlConnection(myconnstrng);
            DataTable dt = new DataTable();

            try
            {
                // Corrected SQL Query searching first_name, last_name, username, and address
                string sql = "SELECT * FROM tbl_users WHERE user_id LIKE '%" + keywords + "%' OR first_name LIKE '%" + keywords + "%' OR last_name LIKE '%" + keywords + "%' OR username LIKE '%" + keywords + "%' OR address LIKE '%" + keywords + "%'";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                conn.Open();
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }
        #endregion

        #region GetIDFromUsername
        public userBLL GetIDFromUsername(string username)
        {
            userBLL u = new userBLL();

            SqlConnection conn = new SqlConnection(myconnstrng);

            DataTable dt = new DataTable();

            try
            {
                string sql = "SELECT user_id FROM tbl_users WHERE username=@username";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();

                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    u.user_id = int.Parse(dt.Rows[0]["user_id"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return u;
        }
        #endregion
    }
}