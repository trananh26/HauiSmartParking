using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Auto_parking
{
    public partial class frmHistory : Form
    {
        private clsCommon cls = new clsCommon();
        private System.Data.DataTable dt = new System.Data.DataTable();
        public frmHistory()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (Excel_SaveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportExcel(dtgHistory, Excel_SaveDialog.FileName);
                    MessageBox.Show("Xuất báo cáo thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void frmHistory_Load(object sender, EventArgs e)
        {

            dt = cls.GetHistory();
            dtgHistory.DataSource = dt;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i != 0 && i != 1 && i != 5 && i != 6)
                {
                    dtgHistory.Columns[i].Width = 150;
                }
                else
                {
                    dtgHistory.Columns[0].Width = 80;
                    dtgHistory.Columns[5].Width = 120;
                }

            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                cls.DeleteHistory();

                dt = cls.GetHistory();
                dtgHistory.DataSource = dt;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i != 0 && i != 1 && i != 5 && i != 6)
                    {
                        dtgHistory.Columns[i].Width = 150;
                    }
                    else
                    {
                        dtgHistory.Columns[0].Width = 80;
                        dtgHistory.Columns[5].Width = 120;
                    }

                }

                MessageBox.Show("Xóa lịch sử thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {

                ;
            }
            
        }

        private void ExportExcel(DataGridView dgvlichsu, string fileName)
        {

            //Tạo các đối tượng Excel

            Microsoft.Office.Interop.Excel.Application oExcel = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel.Workbooks oBooks;

            Microsoft.Office.Interop.Excel.Sheets oSheets;

            Microsoft.Office.Interop.Excel.Workbook oBook;

            Microsoft.Office.Interop.Excel.Worksheet oSheet;

            //Tạo mới một Excel WorkBook 

            oExcel.Visible = true;

            oExcel.DisplayAlerts = false;

            oExcel.Application.SheetsInNewWorkbook = 1;

            oBooks = oExcel.Workbooks;

            oBook = (Microsoft.Office.Interop.Excel.Workbook)(oExcel.Workbooks.Add(Type.Missing));

            oSheets = oBook.Worksheets;

            oSheet = (Microsoft.Office.Interop.Excel.Worksheet)oSheets.get_Item(1);

            oSheet.Name = "Trang 1";

            // Tạo phần Tiêu đề
            Microsoft.Office.Interop.Excel.Range head = oSheet.get_Range("A1", "G1");

            head.MergeCells = true;
            DateTime thoigian = DateTime.Now;
            head.Value2 = "Lịch Sử Gửi Xe " + thoigian.ToString("dd-MM-yyyy");

            head.Font.Bold = true;

            head.Font.Name = "Times New Roman";

            head.Font.Size = "16";

            head.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            // Tạo tiêu đề cột 

            for (int i = 0; i < dgvlichsu.ColumnCount; i++)
            {
                oSheet.Cells[3, i + 1] = dgvlichsu.Columns[i].HeaderText;
               
            }

            // nội dung trong bảng
            for (int i = 0; i < dgvlichsu.RowCount - 1; i++)
            {
                for (int j = 0; j < dgvlichsu.ColumnCount; j++)
                {
                    oSheet.Cells[i + 4, j + 1] = dgvlichsu.Rows[i].Cells[j].Value.ToString();
                }
            }

            oSheet.Columns.AutoFit();
            Microsoft.Office.Interop.Excel.Range rowHead = oSheet.get_Range("A3", "G3");

            rowHead.Font.Bold = true;

            // Kẻ viền

            rowHead.Borders.LineStyle = Microsoft.Office.Interop.Excel.Constants.xlSolid;

            // Thiết lập màu nền

            rowHead.Interior.ColorIndex = 4;

            rowHead.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            //for (int i = 0; i < dgvlichsu.RowCount - 1; i++)
            //{
            //    for (int j = 0; j < dgvlichsu.ColumnCount; j++)
            //    {
            //        oSheet.Cells[i + 4, j + 1] = dgvlichsu.Rows[i].Cells[j].Value.ToString();
            //    }
            //}
            Microsoft.Office.Interop.Excel.Range giatritrongbang = oSheet.get_Range("A3", "G50");// đặt tên đối tượng cần edit
            giatritrongbang.Borders.LineStyle = Microsoft.Office.Interop.Excel.Constants.xlSolid; //kẻ viền 
            giatritrongbang.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter; //căn giữa
            oBook.SaveAs(fileName);
        }

        
    }
}
