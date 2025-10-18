using System;
using System.Windows.Forms;

namespace Auto_parking
{
    public partial class frmCarSystem : Form
    {
        private clsCommon cls = new clsCommon();
        private System.Data.DataTable dt = new System.Data.DataTable();
        public frmCarSystem()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (!cls.Check_SystemPlate(txtCarPlate.Text))
                {
                    cls.Add_SystemPlate(txtCarPlate.Text, txtUserName.Text, txtAddress.Text, txtPhone.Text);
                    LoadData();

                    MessageBox.Show("Thêm biển số xe thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("Biển số xe đã tồn tại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void frmHistory_Load(object sender, EventArgs e)
        {
            LoadData();

        }

        private void LoadData()
        {
            try
            {
                dt = cls.GetAll_SystemPlate();
                dtgHistory.DataSource = dt;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dtgHistory.Columns[i].Width = 129;

                }
                txtCarPlate.Clear();
                txtUserName.Clear();
                txtAddress.Clear();
                txtPhone.Clear();
            }
            catch (Exception ee)
            {

                MessageBox.Show(ee.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtgHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtCarPlate.Text = dtgHistory.CurrentRow.Cells["CarPlate"].Value.ToString();
            txtUserName.Text = dtgHistory.CurrentRow.Cells["UserName"].Value.ToString();
            txtPhone.Text = dtgHistory.CurrentRow.Cells["PhoneNumber"].Value.ToString();
            txtAddress.Text = dtgHistory.CurrentRow.Cells["Address"].Value.ToString();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCarPlate.Text))
            {
                cls.DeleteCarPlate(txtCarPlate.Text);
                MessageBox.Show("Xóa biển số xe thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập thông tin biển số", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
