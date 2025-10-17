using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Auto_parking
{
    public class DLDatabase
    {
        static string connString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Haui_SmartParking;Integrated Security=True";

        private static string ConnectionString = connString;
        private SqlConnection conn = new SqlConnection(ConnectionString);
        private SqlCommand cmd = new SqlCommand();
        private SqlDataReader dr;
        private SqlDataAdapter da;


        /// <summary>
        /// Thêm lịch sử 
        /// </summary>
        /// <param name="Stored"></param>
        /// <param name="item"></param>
        public void InsertHistory(string Stored, string m_RF, string BienSo, string Employee)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Stored;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@RFCode", m_RF);
                cmd.Parameters.AddWithValue("@CarNumber", BienSo);
                cmd.Parameters.AddWithValue("@Employee", Employee);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ee)
            {

            }
        }

        /// <summary>
        /// Lấy lịch sử chấm công của 1 nhân viên nào đó
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        public DataTable GetKeppingHistoryByEmployeeCode(string sqlCommand)
        {
            DataTable dt = new DataTable();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                da = new SqlDataAdapter(sqlCommand, conn);
                da.Fill(dt);
                conn.Close();
            }
            catch (Exception ee)
            {

            }

            return dt;
        }

        /// <summary>
        /// Cập nhật lịch sử chấm công
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="item"></param>
        public void UpdateHistory(string Stored, string RFID, Guid ID, int TotalMoney)
        {

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Stored;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ID", ID);
                cmd.Parameters.AddWithValue("@RFCode", RFID);
                cmd.Parameters.AddWithValue("@TotalMoney", TotalMoney);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ee)
            {

            }
        }
    }
}
