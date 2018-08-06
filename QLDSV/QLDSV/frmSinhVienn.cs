using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Collections;

namespace QLDSV
{
    public partial class frmSinhVienn : DevExpress.XtraEditors.XtraForm
    {

        const int THEM = 0;
        const int HIEU_CHINH = 1;
        const int XOA = 2;

        public Stack st = new Stack();
        int choose = -1;
        int vitri = 0;
        string maLop = "";
        String maSV = "";
        bool isDangThaoTac = false;

        public frmSinhVienn()
        {
            InitializeComponent();
        }

        private void kHOABindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
         
        }

        private void frmSinhVienn_Load(object sender, EventArgs e)
        {
            DS2.EnforceConstraints = false;
            // TODO: This line of code loads data into the 'DS2.SINHVIEN' table. You can move, or remove it, as needed.
            this.SinhVienTableAdapter.Fill(this.DS2.SINHVIEN);
            // TODO: This line of code loads data into the 'qLDSVDS.LOP' table. You can move, or remove it, as needed.
            this.LopTableAdapter.Fill(this.DS2.LOP);

            maLop = ((DataRowView)bdsSinhVien[0])["MALOP"].ToString();
            this.LopTableAdapter.Update(this.DS2.LOP);
            this.SinhVienTableAdapter.Update(this.DS2.SINHVIEN);
            cmbKhoa.DataSource = Program.bds_dspm;  // sao chép bds_dspm đã load ở form đăng nhập  qua
            cmbKhoa.DisplayMember = "TENKHOA";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mChinhanh;
            cmbLop.DataSource = this.bdsLop;
            cmbLop.SelectedItem = 1;
            cmbLop.SelectedItem = 0;

        }

        private void cmbLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.KetNoi() == 0)
                MessageBox.Show("Lỗi kết nối về chi nhánh mới", "", MessageBoxButtons.OK);
            else
            {

                this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.SinhVienTableAdapter.Fill(this.DS2.SINHVIEN);
            }
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
                this.LopTableAdapter.Fill(this.DS2.LOP);
                cmbLop.SelectedItem = 1;
                cmbLop.SelectedItem = 0;
                //maLop = ((DataRowView)bdsSinhVien[0])["MALOP"].ToString();

                //if (choose == THEM)
                //{
                //    this.bdsSinhVien.AddNew();
                //    txtMaLop.Text = maLop;
                //}
            }
        }
    }
}