using System;
using System.Collections;
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
    public partial class frmLop : Form
    {
        const int THEM = 0;
        const int HIEU_CHINH = 1;
        const int XOA = 2;

        public Stack st = new Stack();
        int choose = -1;
        int vitri = 0;
        string maKhoa = "";
        bool isDangThaoTac = false;

        public frmLop()
        {
            InitializeComponent();
        }

        private void lOPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLop.EndEdit();
            this.LopAdapterManager.UpdateAll(this.DS);

        }

        private void frmLop_Load(object sender, EventArgs e)
        {
            
            DS.EnforceConstraints = false;
            this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
            this.LopTableAdapter.Fill(this.DS.LOP);
            this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.SinhVienTableAdapter.Fill(this.DS.SINHVIEN);

            //this.lOPTableAdapter.Fill(this.dS.LOP);
            maKhoa = ((DataRowView)bdsLop[0])["MAKH"].ToString();
            this.LopTableAdapter.Update(this.DS.LOP);
            cmbKhoa.DataSource = Program.bds_dspm;  // sao chép bds_dspm đã load ở form đăng nhập  qua
            cmbKhoa.DisplayMember = "TENKHOA";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mChinhanh;

            if (Program.mGroup == "PGV")
                cmbKhoa.Enabled = true;  // bật tắt theo phân quyền
            else
                cmbKhoa.Enabled = false;
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsLop.Position;
            groupBox1.Enabled = true;
            bdsLop.AddNew();
            txtMaKhoa.Text = maKhoa;
            txtMaLop.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcLop.Enabled = false;
            choose = THEM;
            isDangThaoTac = true;
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsLop.Position;
            groupBox1.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            txtMaLop.Enabled = txtMaKhoa.Enabled = false;
            gcLop.Enabled = false;
            choose = HIEU_CHINH;
            isDangThaoTac = true;

            Lop lop = new Lop(txtMaLop.Text, txtTenLop.Text, txtMaKhoa.Text);
            UndoLop undo = new UndoLop(HIEU_CHINH, lop);
            st.Push(undo);
        }

        private void capNhatBtnPhucHoi()
        {
            if (st.Count == 0)
            {
                btnPhucHoi.Enabled = false;
            }
            else
            {
                btnPhucHoi.Enabled = true;
            }
        }

        private String convertStringToUTF8(String s)
        {
            var dbEnc = Encoding.UTF8;
            var uniEnc = Encoding.Unicode;
            byte[] dbByte = dbEnc.GetBytes(s);
            byte[] uniBytes = Encoding.Convert(dbEnc, uniEnc, dbByte);

            return uniEnc.GetString(uniBytes);
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            switch (choose)
            {
                case THEM:
                    if (txtMaLop.Text.Trim() == "")
                    {
                        MessageBox.Show("Mã lớp không được thiếu!", "", MessageBoxButtons.OK);
                        txtMaLop.Focus();
                        return;
                    }

                    if (txtTenLop.Text.Trim() == "")
                    {
                        MessageBox.Show("Tên lớp không được thiếu!", "", MessageBoxButtons.OK);
                        txtTenLop.Focus();
                        return;
                    }

                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strLenh = "dbo.sp_KTMALOP";
                    Program.sqlcmd = Program.conn.CreateCommand();
                    Program.sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.sqlcmd.CommandText = strLenh;
                    Program.sqlcmd.Parameters.Add("@MALOP", SqlDbType.Text).Value = txtMaLop.Text;
                    Program.sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    Program.sqlcmd.ExecuteNonQuery();
                    String Ret = Program.sqlcmd.Parameters["@Ret"].Value.ToString();

                    if (Ret.Equals("1"))
                    {
                        MessageBox.Show("Mã lớp bị trùng!", "", MessageBoxButtons.OK);
                        txtMaLop.Focus();
                        Program.conn.Close();
                        return;
                    }

                    String sql = "exec dbo.sp_KTTENLOP N'" + txtTenLop.Text + "'";
                    DataTable dataTable = Program.ExecSqlDataTable(sql);

                    if (dataTable.Rows.Count > 0)
                    {
                        MessageBox.Show("Tên lớp bị trùng!");
                        txtTenLop.Focus();
                        Program.conn.Close();
                        return;
                    }

                    try
                    {
                        bdsLop.EndEdit();
                        bdsLop.ResetCurrentItem();
                        this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.LopTableAdapter.Update(this.DS.LOP);

                        String lenhXoaLop = "exec sp_XOALOP '" + txtMaLop.Text + "'";
                        UndoLop undo = new UndoLop(THEM, lenhXoaLop);
                        st.Push(undo);
                        capNhatBtnPhucHoi();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi ghi lớp.\n" + ex.Message, "", MessageBoxButtons.OK);
                        return;
                    }
                    gcLop.Enabled = true;
                    btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = true;
                    btnGhi.Enabled = false;
                    groupBox1.Enabled = false;
                    isDangThaoTac = false;
                    break;

                case HIEU_CHINH:
                    if (txtTenLop.Text.Trim() == "")
                    {
                        MessageBox.Show("Tên lớp không được thiếu!", "", MessageBoxButtons.OK);
                        txtTenLop.Focus();
                        return;
                    }

                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();

                    String strLenhKiemTraTenLop = "exec dbo.sp_KTTENLOP N'" + txtTenLop.Text + "'";
                    DataTable dtKTTenLop = Program.ExecSqlDataTable(strLenhKiemTraTenLop);

                    if (dtKTTenLop.Rows.Count > 0)
                    {
                        MessageBox.Show("Tên lớp bị trùng!");
                        txtTenLop.Focus();
                        Program.conn.Close();
                        return;
                    }

                    try
                    {
                        bdsLop.EndEdit();
                        bdsLop.ResetCurrentItem();
                        this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.LopTableAdapter.Update(this.DS.LOP);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi hiệu chỉnh nhân viên.\n" + ex.Message, "", MessageBoxButtons.OK);
                        st.Pop();
                        return;
                    }
                    gcLop.Enabled = true;
                    btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = true;
                    btnGhi.Enabled = false;
                    groupBox1.Enabled = false;
                    isDangThaoTac = false;
                    break;
            }
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcLop.Enabled = true;
            groupBox1.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = false;

            if (isDangThaoTac)
            {
                if (choose == HIEU_CHINH) st.Pop();
                reload();
                capNhatBtnPhucHoi();
                isDangThaoTac = false;
                return;
            }

            bdsLop.CancelEdit();
            if (btnThem.Enabled == false) bdsLop.Position = vitri;

            if (st.Count == 0)
            {
                btnPhucHoi.Enabled = false;
                return;
            }

            UndoLop objUndo = (UndoLop)st.Pop();
            Object obj = objUndo.obj;

            if (st.Count > 0)
            {
                btnPhucHoi.Enabled = true;
            }

            switch (objUndo.chucNang)
            {
                case THEM:
                    Program.ExecSqlDataReader(obj.ToString());
                    this.LopTableAdapter.Fill(this.DS.LOP);
                    Program.conn.Close();
                    capNhatBtnPhucHoi();
                    break;
                case HIEU_CHINH:
                    Lop lopHieuChinh = (Lop)obj;
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();

                    String sqlHieuChinh = "exec sp_PhucHoiLopHieuChinh N'" + lopHieuChinh.maLop + "',N'" + lopHieuChinh.tenLop + "'";
                    Program.ExecSqlDataTable(sqlHieuChinh);
                    Program.conn.Close();
                    reload();
                    break;
                case XOA:
                    Lop lopXoa = (Lop)obj;
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();

                    String sql = "exec sp_ThemLop N'" + lopXoa + "',N'" + lopXoa.tenLop + "',N'" + lopXoa.maKhoa + "'";
                    Program.conn.Close();
                    reload();
                    break;
            }
        }
       
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string maLop = "";
            if (bdsSinhVien.Count > 0)
            {
                MessageBox.Show("Không thể xóa lớp này vì đã lập phiếu", "",
                       MessageBoxButtons.OK);
                return;
            }
            if (MessageBox.Show("Bạn có thật sự muốn xóa lớp này ?? ", "Xác nhận",
                       MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    UndoLop undo = new UndoLop(XOA, new Lop(txtMaLop.Text, txtTenLop.Text, txtMaKhoa.Text));
                    st.Push(undo);

                    maLop = ((DataRowView)bdsLop[bdsLop.Position])["MALOP"].ToString(); // giữ lại để khi xóa bij lỗi thì ta sẽ quay về lại
                    bdsLop.RemoveCurrent();
                    this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.LopTableAdapter.Update(this.DS.LOP);

                    capNhatBtnPhucHoi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa lớp. Bạn hãy xóa lại\n" + ex.Message, "",
                        MessageBoxButtons.OK);
                    this.LopTableAdapter.Fill(this.DS.LOP);
                    bdsLop.Position = bdsLop.Find("MALOP", maLop);
                    return;
                }
            }

            if (bdsLop.Count == 0) btnXoa.Enabled = false;

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
                this.LopTableAdapter.Fill(this.DS.LOP);
                maKhoa = ((DataRowView)bdsLop[0])["MAKH"].ToString();
            }

            if (choose == THEM)
            {
                bdsLop.AddNew();
                txtMaKhoa.Text = maKhoa;
            }
        }

        private void reload()
        {
            try
            {
                this.LopTableAdapter.Fill(this.DS.LOP);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload :" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

    }

    public class Lop
    {
        public String maLop;
        public String tenLop;
        public String maKhoa;

        public Lop(String maLop, String tenLop, String maKhoa)
        {
            this.maLop = maLop;
            this.tenLop = tenLop;
            this.maKhoa = maKhoa;
        }
    }

    public class UndoLop
    {
        public int chucNang;
        public Object obj;

        public UndoLop(int chucNang, Object obj)
        {
            this.chucNang = chucNang;
            this.obj = obj;
        }

    }
}
