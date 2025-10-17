using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Auto_parking
{
    public class clsCommon
    {
        string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Haui_SmartParking;Integrated Security=True";

       
        DLDatabase db = new DLDatabase();

        /// <summary>
        /// Đếm số lượt xe vào trong ngày
        /// </summary>
        /// <returns></returns>
        internal int InputCount()
        {
            DataTable data = new DataTable();
            DateTime thoigian = DateTime.Now;
            string query = "select * from SystemHistoryData Where InputTime > '" + thoigian.ToString("yyyy-MM-dd 00:00:00") + "' AND InputTime < '" + thoigian.ToString("yyyy-MM-dd 23:59:59") + "'";
            //sqlconnection
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);
                connection.Close();
            }
            return data.Rows.Count;

        }

        /// <summary>
        /// Đếm số lượt xe ra trong ngày
        /// </summary>
        /// <returns></returns>
        internal int OutputCount()
        {
            DataTable data = new DataTable();

            DateTime thoigian = DateTime.Now;
            string query = "select * from SystemHistoryData Where OutputTime > '" + thoigian.ToString("yyyy-MM-dd 00:00:00") + "' AND OutputTime < '" + thoigian.ToString("yyyy-MM-dd 23:59:59") + "'";

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }
            return data.Rows.Count;
        }

        /// <summary>
        /// Lưu thông tin gửi xe
        /// </summary>
        /// <param name="m_RF"></param>
        /// <param name="bienso"></param>
        public void GuiXe(string m_RF, string bienso)
        {
            DateTime thoigian = DateTime.Now;
            string Employee;
            if (thoigian.Hour > 18 || thoigian.Hour < 6)
                Employee = "Ca tối";
            else
                Employee = "Ca ngày";

            string Stored = "Insert_SystemHistoryData";
            db.InsertHistory(Stored, m_RF, bienso, Employee);
        }

        /// <summary>
        /// Lưu thông tin lấy xe
        /// </summary>
        /// <param name="m_RF"></param>
        /// <param name="bienso"></param>
        public void LayXe(string m_RF, Guid ID, int TotalTime)
        {
            string Stored = "Update_SystemHistoryData";
            db.UpdateHistory(Stored, m_RF, ID, TotalTime * 1000);
        }

        /// <summary>
        /// Kiểm tra mã thẻ RFID có trong hệ thống không
        /// </summary>
        /// <param name="m_RF"></param>
        /// <returns></returns>
        internal bool Check_RF(string m_RF)
        {
            DataTable data = new DataTable();

            //sqlconnection
            string query = "select * from RFID_Managerment WHERE RFCode = '" + m_RF + "'";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }
            return data.Rows.Count > 0;
        }


        /// <summary>
        /// Kiểm tra xem biển số xe này có đang trong bãi không
        /// </summary>
        /// <param name="bienso"></param>
        /// <returns></returns>
        internal bool Check_BienSo(string bienso)
        {
            DataTable data = new DataTable();

            //sqlconnection
            string query = "select * from CurentSystemData WHERE CarNumber = '" + bienso + "'";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }
            return data.Rows.Count > 0;
        }


        /// <summary>
        /// Lấy về thông tin xe đang có trong bãi
        /// </summary>
        /// <param name="m_RF"></param>
        /// <param name="bienso"></param>
        /// <returns></returns>
        public DataTable GetInfor(string m_RF, string bienso)
        {
            DataTable data = new DataTable();

            //sqlconnection
            string query = "select * from CurentSystemData WHERE CarNumber = '" + bienso + "' AND RFCode = '" + m_RF + "'";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }
            return data;
        }

        /// <summary>
        /// Lấy ra doanh thu ngày
        /// </summary>
        /// <returns></returns>
        internal string GetTotalMoney()
        {
            try
            {
                DateTime thoigian = DateTime.Now;
                DataTable data = new DataTable();
                string query = "Select SUM(Money) AS Money From TotalMoney Where UpdateTime > '" + thoigian.ToString("yyyy-MM-dd 00:00:00") + "' AND UpdateTime < '" + thoigian.ToString("yyyy-MM-dd 23:59:59") + "'";
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(data);
                    connection.Close();
                }

                return data.Rows[0]["Money"].ToString();
            }
            catch (Exception)
            {

                return "0";
            }

        }

        /// <summary>
        /// Lưu lại doanh thu
        /// </summary>
        /// <param name="Money"></param>

        internal void SaveMoney(int Money)
        {
            DateTime thoigian = DateTime.Now;
            //Lịch sử thi tiền
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                string query = "Insert TotalMoney SELECT NewID(), @1, @2";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@1", thoigian.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@2", Money);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// lịch sử hệ thống
        /// </summary>
        /// <returns></returns>
        public DataTable GetHistory()
        {
            DateTime thoigian = DateTime.Now;
            DataTable data = new DataTable();
            string query = "EXEC Proc_GetSystemHistoryData";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);
                connection.Close();
            }
            return data;
        }


        //---------------------------------------------------------
        /// <summary>
        /// ckec biển số xe 
        /// </summary>
        /// <param name="bienso"></param>
        /// <returns></returns>
        public bool Check_SystemPlate(string bienso)
        {
            DataTable data = new DataTable();

            //sqlconnection
            string query = "select * from SystemPlate WHERE CarPlate = '" + bienso + "'";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }
            return data.Rows.Count > 0;
        }
        /// <summary>
        /// hệ thống biển số xe 
        /// </summary>
        /// <returns></returns>
        public DataTable GetAll_SystemPlate()
        {
            DataTable data = new DataTable();

            //sqlconnection
            string query = "select CarPlate, UserName, PhoneNumber, Address from SystemPlate";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(data);

                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// check bien so xe ra
        /// </summary>
        /// <param name="bienso"></param>
        /// <returns></returns>
        public DataTable Check_BienSoRa(string bienso)
        {
            DataTable dt = new DataTable();
            //sqlconnection
            string query = "select * from CurentSystemData WHERE CarNumber = '" + bienso + "'";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dt);

                connection.Close();
            }
            return dt;
        }
        /// <summary>
        /// thêm biển số xe vào
        /// </summary>
        /// <param name="CarPlate"></param>
        /// <param name="User"></param>
        /// <param name="Address"></param>
        /// <param name="Phone"></param>
        public void Add_SystemPlate(string CarPlate, string User, string Address, string Phone)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                string query = "Insert SystemPlate SELECT NewID(), @CarPlate, @UserName, @Address, @PhoneNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CarPlate", CarPlate);
                command.Parameters.AddWithValue("@UserName", User);
                command.Parameters.AddWithValue("@Address", Address);
                command.Parameters.AddWithValue("@PhoneNumber", Phone);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        /// <summary>
        /// xóa lịch sử
        /// </summary>
        public void DeleteHistory()
        {
            string query1 = "delete SystemHistoryData";
            string query2 = "delete CurentSystemData";

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query1, connection);
                command.ExecuteNonQuery();

                SqlCommand command2 = new SqlCommand(query2, connection);
                command2.ExecuteNonQuery();

                connection.Close();
            }
        }
/// <summary>
///  xóa biển số xe 
/// </summary>
/// <param name="CarPlate"></param>
        public void DeleteCarPlate(string CarPlate)
        {
            string query = "delete SystemPlate Where CarPlate = N'" + CarPlate + "'";
            

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();                
                connection.Close();
            }
        }
    }
}
