using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLDSV
{
    public partial class frmTaoTaiKhoan : Form
    {
        public frmTaoTaiKhoan()
        {
            InitializeComponent();
        }

        private void frmTaoTaiKhoan_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
            this.GiangVienTableAdapter.Fill(this.DS.GIANGVIEN);

            cmbKhoa.DataSource = Program.bds_dspm;  // sao chép bds_dspm đã load ở form đăng nhập  qua
            cmbKhoa.DisplayMember = "TENKHOA";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mChinhanh;
            if (Program.mGroup == "PGV")
                cmbKhoa.Enabled = true;  // bật tắt theo phân quyền
            else
                cmbKhoa.Enabled = false;
            initComboBoxGiangVien();
            if (Program.mGroup == "PGV")
            {
                cmbQuyen.Items.Add("PGV");
                cmbQuyen.Items.Add("Khoa");
                cmbQuyen.Items.Add("User");
                cmbQuyen.SelectedIndex = 0;
                //this.panel1.Enabled = false;
                cmbKhoa.Enabled = true;
            }
            else
            {
                if (Program.mGroup == "Khoa")
                {
                    cmbQuyen.Items.Add("Khoa");
                    cmbQuyen.Items.Add("User");
                    cmbQuyen.SelectedIndex = 0;
                    //this.panel1.Enabled = true;
                }
                else
                {
                    cmbQuyen.Items.Add("User");
                    cmbQuyen.SelectedIndex = 0;
                }
                cmbKhoa.Enabled = false;
            }

        }

        private void initComboBoxGiangVien()
        {
            cmbKhoa.Enabled = true;
            try
            {

                String sql = "exec sp_LayDsGiaoVienTheoTenKhoa N'" + cmbKhoa.Text.ToString() + "'";
                DataTable tb = Program.ExecSqlDataTable(sql);
                if (tb.Columns.Count > 0)
                {
                    cmbGiangVien.DataSource = tb;
                    cmbGiangVien.DisplayMember = "MAGV";
                    cmbGiangVien.ValueMember = "TEN";

                    cmbGiangVien.SelectedIndex = 0;

                }

            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void cmbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            initComboBoxGiangVien();

            this.GiangVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.GiangVienTableAdapter.Fill(this.DS.GIANGVIEN);
            cmbGiangVien.SelectedItem = 1;
            cmbGiangVien.SelectedItem = 0;
        }


        private void btnSignUp_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == "")
            {
                MessageBox.Show("Vui lòng nhập mật khẩu !!!", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            if (txtUsername.Text == "")
            {
                MessageBox.Show("Vui lòng nhập username !!!", "Thông báo", MessageBoxButtons.OK);
                return;
            }

            if (cmbGiangVien.ValueMember == Program.username)
            {
                MessageBox.Show("Tai khoan nay dang dang nhap !!!", "Thông báo", MessageBoxButtons.OK);
                return;
            }
            MessageBox.Show(cmbGiangVien.Text);

            if (Program.conn.State == ConnectionState.Closed)
                Program.conn.Open();
            String strLenh1 = "dbo.sp_KiemTraTaiKhoanDaDangKy";
            Program.sqlcmd = Program.conn.CreateCommand();
            Program.sqlcmd.CommandType = CommandType.StoredProcedure;
            Program.sqlcmd.CommandText = strLenh1;
            Program.sqlcmd.Parameters.Add("@TENUSER", SqlDbType.NChar).Value = cmbGiangVien.Text.Trim();
            Program.sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            Program.sqlcmd.ExecuteNonQuery();
            Program.conn.Close();
            String Ret = Program.sqlcmd.Parameters["@Ret"].Value.ToString();
            if (Ret.Equals("1"))
            {
                MessageBox.Show("LOGINNAME đã tồn tại!!!", "Thông báo");
                return;
            }
            if (Ret.Equals("-1"))
            {
                MessageBox.Show("LOGINNAME không có ID!!!", "Thông báo");
                return;
            }

            if (Program.conn.State == ConnectionState.Closed)
                Program.conn.Open();
            String strLenh = "dbo.sp_TaoTaiKhoan";
            Program.sqlcmd = Program.conn.CreateCommand();
            Program.sqlcmd.CommandType = CommandType.StoredProcedure;
            Program.sqlcmd.CommandText = strLenh;
            Program.sqlcmd.Parameters.Add("@LGNAME", SqlDbType.VarChar).Value = txtUsername.Text.Trim();
            Program.sqlcmd.Parameters.Add("@PASS", SqlDbType.VarChar).Value = txtPassword.Text.Trim();
            Program.sqlcmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = cmbGiangVien.Text.Trim();
            Program.sqlcmd.Parameters.Add("@ROLE ", SqlDbType.VarChar).Value = cmbQuyen.Text.Trim();
            Program.sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            Program.sqlcmd.ExecuteNonQuery();
            Program.conn.Close();
            String Ret1 = Program.sqlcmd.Parameters["@Ret"].Value.ToString();
            if (Ret1.Equals("1"))
            {
                MessageBox.Show("LOGINNAME bị trùng!!!", "Thông báo");
                cmbGiangVien.Focus();
                return;
            }
            else if (Ret1.Equals("2"))
            {
                MessageBox.Show("USERNAME bị trùng !!!", "Thông báo");
                return;
            }
            else
            {
                MessageBox.Show("Thêm thành công", "thông báo");
                txtUsername.Text = "";
                txtPassword.Text = "";
                return;
            }
        }

    }
}
