using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QLDSV
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qLDSVDataSet.V_DS_PHANMANH' table. You can move, or remove it, as needed.
            this.v_DS_PHANMANHTableAdapter.Fill(this.qLDSVDataSet.V_DS_PHANMANH);
            cmbTenKhoa.SelectedIndex = 1;
            cmbTenKhoa.SelectedIndex = 0;

            txtTenDangNhap.Text = "NHS";
            txtMatKhau.Text = "123";
        }

        private void cmbTenKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbTenKhoa.SelectedValue != null)
                Program.servername = cmbTenKhoa.SelectedValue.ToString();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {

            if (txtTenDangNhap.Text.Trim() == "" || txtMatKhau.Text.Trim() == "")
            {
                MessageBox.Show("Login name và mật mã không được trống", "", MessageBoxButtons.OK);
                return;
            }
            Program.mlogin = txtTenDangNhap.Text;
            Program.password = txtMatKhau.Text;
            if (Program.KetNoi() == 0) return;
            Program.mChinhanh = cmbTenKhoa.SelectedIndex;
            Program.bds_dspm = bdsDSPM;
            Program.mloginDN = Program.mlogin;
            Program.passwordDN = Program.password;
            string strLenh = "EXEC sp_DANGNHAP '" + Program.mlogin + "'";

            Program.myReader = Program.ExecSqlDataReader(strLenh);
            if (Program.myReader == null) return;
            Program.myReader.Read();


            Program.username = Program.myReader.GetString(0);     // Lay user name
            if (Convert.IsDBNull(Program.username))
            {
                MessageBox.Show("Login bạn nhập không có quyền truy cập dữ liệu\n Bạn xem lại username, password", "", MessageBoxButtons.OK);
                return;
            }
            Program.mHoten = Program.myReader.GetString(1);
            Program.mGroup = Program.myReader.GetString(2);
            Program.myReader.Close();
            Program.conn.Close();
            this.Hide();
            Program.frmMain.HienThiMenu();

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void fillBy1ToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.v_DS_PHANMANHTableAdapter.FillBy1(this.qLDSVDataSet.V_DS_PHANMANH);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

    }
}
