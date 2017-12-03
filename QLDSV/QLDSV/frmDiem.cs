using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLDSV
{
    public partial class frmDiem : Form
    {

        const bool hienBangDiem = true;

        public frmDiem()
        {
            InitializeComponent();
        }

        private void lOPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLop.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dS);

        }

        private void frmDiem_Load(object sender, EventArgs e)
        {
            dS.EnforceConstraints = false;
            this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.SinhVienTableAdapter.Fill(this.dS.SINHVIEN);
            this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
            this.LopTableAdapter.Fill(this.dS.LOP);
            this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
            this.MonHocTableAdapter.Fill(this.dS.MONHOC);

            this.LopTableAdapter.Update(this.dS.LOP);
            this.SinhVienTableAdapter.Update(this.dS.SINHVIEN);
            this.MonHocTableAdapter.Update(this.dS.MONHOC);

            cmbKhoa.DataSource = Program.bds_dspm;  // sao chép bds_dspm đã load ở form đăng nhập  qua
            cmbKhoa.DisplayMember = "TENKHOA";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mChinhanh;

            cmbLop.DataSource = this.bdsLop;
            cmbLop.DisplayMember = "TENLOP";
            cmbLop.DisplayMember = "MALOP";

            cmbMonHoc.DataSource = this.dS.MONHOC;
            cmbMonHoc.DisplayMember = "TENMH";
            cmbMonHoc.ValueMember = "MAMH";

            var lanThi = new [] {1,2};
            cmbLanThi.DataSource = lanThi;

            if (Program.mGroup == "PGV")
                cmbKhoa.Enabled = true;

            if (Program.mGroup == "PGV" || Program.mGroup == "Khoa")
                cmbLop.Enabled = true;
            else
                cmbLop.Enabled = false;

            gcNhapDiem.Hide();
        }

        private void cmbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbKhoa.SelectedValue == null) return;

            if (cmbKhoa.SelectedValue.ToString() == "System.Data.DataRowView") return;
            Program.servername = cmbKhoa.SelectedValue.ToString();

            if (cmbKhoa.SelectedIndex != Program.mChinhanh)
            {
                Program.mlogin = Program.remotelogin;
                Program.password = Program.remotepassword;
            }
            else
            {
                Program.mlogin = Program.mloginDN;
                Program.password = Program.passwordDN;
            }
            if (Program.KetNoi() == 0)
                MessageBox.Show("Lỗi kết nối về chi nhánh mới", "", MessageBoxButtons.OK);
            else
            {
                this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
                this.LopTableAdapter.Fill(this.dS.LOP);

                cmbLop.SelectedItem = 1;
                cmbLop.SelectedItem = 0;
            }
        }

        private void updateView(bool b)
        {
            cmbKhoa.Enabled = cmbMonHoc.Enabled = cmbLop.Enabled = cmbLanThi.Enabled= btnBatDau.Enabled = !b;
            btnLuu.Enabled = b;

            if (Program.mGroup != "PGV")
            {
                cmbKhoa.Enabled = false;
            }

            if (Program.mGroup == "User")
            {
                cmbLop.Enabled = false;
            }

            if (b == !hienBangDiem)
                gcNhapDiem.Hide();
            else
                gcNhapDiem.Show();
        }

        private void initDataGcNhapDiem()
        {

            try
            {
                this.NhapDiemTableAdapter.Connection.ConnectionString = Program.connstr;
                this.NhapDiemTableAdapter.Fill(
                    this.dS.sp_BangDiem,
                    cmbLop.SelectedValue.ToString(),
                    cmbMonHoc.SelectedValue.ToString(),
                    short.Parse(cmbLanThi.SelectedValue.ToString())
               );
            }
            catch (System.Exception ex)
            {
                string ss = cmbLop.SelectedValue.ToString() + cmbMonHoc.SelectedValue.ToString() + short.Parse(cmbLanThi.SelectedValue.ToString());
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            this.NhapDiemTableAdapter.Connection.ConnectionString = Program.connstr;

            DataTable data = this.NhapDiemTableAdapter.GetData(
                                                  cmbLop.SelectedValue.ToString(),
                                                  cmbMonHoc.SelectedValue.ToString(),
                                                  short.Parse(cmbLanThi.SelectedValue.ToString())
                             );

            if (data.Rows.Count == 0)
            {
                MessageBox.Show("Data null");
                updateView(!hienBangDiem);
            }
        }

        private void btnBatDau_Click(object sender, EventArgs e)
        {
            gcNhapDiem.Show();
            updateView(hienBangDiem);
            initDataGcNhapDiem();
        }

        private void cmbLop_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            DataTable table = new DataTable("DiemThayDoi");
            table.Columns.Add("MASV", typeof(string));
            table.Columns.Add("MAMH", typeof(string));
            table.Columns.Add("LAN", typeof(int));
            table.Columns.Add("DIEM", typeof(float));

            String maSV = "";
            String maMH = cmbMonHoc.SelectedValue.ToString();
            int lan = Int32.Parse(cmbLanThi.SelectedValue.ToString());
            float diem = 0;
            foreach (DataRowView row in bdsNhapDiem)
            {
                maSV = row["MASV"].ToString().Trim();
                if (row["DIEM"].ToString().Trim() != "")
                {
                    diem = float.Parse(row["DIEM"].ToString());
                    table.Rows.Add(maSV, maMH, lan, diem);
                }
            }

            if (Program.conn.State == ConnectionState.Closed)
                Program.conn.Open();
            String strLenhKiemTraTenLop = "dbo.sp_CapNhatDiem";
            Program.sqlcmd = Program.conn.CreateCommand();
            Program.sqlcmd.CommandType = CommandType.StoredProcedure;
            Program.sqlcmd.CommandText = strLenhKiemTraTenLop;
            Program.sqlcmd.Parameters.AddWithValue("@TEMP_TABLE", table);

            SqlDataAdapter dpt = new SqlDataAdapter(Program.sqlcmd);
            DataTable ds = new DataTable();
            dpt.Fill(ds);

            if (ds.Rows.Count > 0)
                MessageBox.Show("CẬP NHẬT ĐIỂM THÀNH CÔNG!");
            else
                MessageBox.Show("CẬP NHẬT ĐIỂM THẤT BẠI!");

            updateView(!hienBangDiem);
        }

        private void cmbLanThi_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnInDanhSachDiem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBaoCaoDanhSachDiem f = new frmBaoCaoDanhSachDiem();
            f.setMaLop(cmbLop.SelectedValue.ToString());
            f.setTenLop(cmbLop.Text.ToString());
            f.setMaMonHoc(cmbMonHoc.SelectedValue.ToString());
            f.seTenMonHoc(cmbMonHoc.Text.ToString());
            f.setLanThi(cmbLanThi.Text.ToString());
            f.ShowDialog();
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }
       
    }
}
