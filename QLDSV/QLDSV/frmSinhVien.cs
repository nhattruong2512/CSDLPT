using System;
using System.Collections;
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
    public partial class frmSinhVien : Form
    {
        const int THEM = 0;
        const int HIEU_CHINH = 1;
        const int XOA = 2;

        public Stack st = new Stack();
        int choose = -1;
        int vitri = 0;
        string maLop = "";
        String maSV = "";
        bool isChangeKhoa;

        public frmSinhVien()
        {
            InitializeComponent();
        }

        private void kHOABindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();

        }

        private void lOPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLop.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'DS.LOP' table. You can move, or remove it, as needed.
            // TODO: This line of code loads data into the 'DS.SINHVIEN' table. You can move, or remove it, as needed.
            DS.EnforceConstraints = false;
            // TODO: This line of code loads data into the 'dS.SINHVIEN' table. You can move, or remove it, as needed.
            this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.SinhVienTableAdapter.Fill(this.DS.SINHVIEN);
            // TODO: This line of code loads data into the 'dS.LOP' table. You can move, or remove it, as needed.
            this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
            this.LopTableAdapter.Fill(this.DS.LOP);

            maLop = ((DataRowView)bdsSinhVien[0])["MALOP"].ToString();
            this.LopTableAdapter.Update(this.DS.LOP);
            this.SinhVienTableAdapter.Update(this.DS.SINHVIEN);
            cmbKhoa.DataSource = Program.bds_dspm;  // sao chép bds_dspm đã load ở form đăng nhập  qua
            cmbKhoa.DisplayMember = "TENKHOA";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mChinhanh;
            cmbLop.DataSource = this.bdsLop;
            cmbLop.SelectedItem = 1;
            cmbLop.SelectedItem = 0;
            if (Program.mGroup == "PGV")
                cmbKhoa.Enabled = true;  
            else
                cmbKhoa.Enabled = false;
            if (Program.mGroup == "PGV" || Program.mGroup == "Khoa")
                cmbLop.Enabled = true;
            else
                cmbLop.Enabled = false;

        }

        bool isChange = false;

        private void cmbLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.KetNoi() == 0)
                MessageBox.Show("Lỗi kết nối về chi nhánh mới", "", MessageBoxButtons.OK);
            else
            {

                this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.SinhVienTableAdapter.Fill(this.DS.SINHVIEN);
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
                this.LopTableAdapter.Fill(this.DS.LOP);
                maLop = ((DataRowView)bdsSinhVien[0])["MALOP"].ToString();

                if (choose == THEM)
                {
                    this.bdsSinhVien.AddNew();
                    txtMaLop.Text = maLop;
                }
            }
        }

        private void pHAILabel_Click(object sender, EventArgs e)
        {

        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsSinhVien.Position;
            cmbKhoa.Enabled = cmbLop.Enabled = false;
            groupBox1.Enabled = true;
            bdsSinhVien.AddNew();
            txtMaLop.Text = maLop;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            txtMaLop.Enabled = false;
            gcSinhVien.Enabled = false;
            choose = THEM;
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsLop.Position;
            groupBox1.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            txtMaLop.Enabled = txtMASV.Enabled = false;
            gcSinhVien.Enabled = false;
            choose = HIEU_CHINH;

            UndoLop undo = new UndoLop(HIEU_CHINH, getInfoSinhVien());
            st.Push(undo);
        }

        private bool checkEmtyInfo()
        {
            if (txtMASV.Text.Trim() == "")
            {
                MessageBox.Show("Mã sinh viên rỗng!");
                return false;
            }
            if (txtHo.Text.Trim() == "")
            {
                MessageBox.Show("Họ rỗng!");
                return false;
            }
            if (txtTen.Text.Trim() == "")
            {
                MessageBox.Show("Tên rỗng");
                return false;
            }
            if (txtMaLop.Text.Trim() == "")
            {
                MessageBox.Show("Mã lớp rỗng!");
                return false;
            }
            if (txtNgaySinh.Text.Trim() == "")
            {
                MessageBox.Show("Ngày sinh rỗng!");
                return false;
            }
            if (txtNoiSinh.Text.Trim() == "")
            {
                MessageBox.Show("Nơi sinh rỗng!");
                return false;
            }
           
            return true;
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn xóa sinh viên này ?? ", "Xác nhận",
                       MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                   
                    UndoLop undo = new UndoLop(XOA, getInfoSinhVien());
                    st.Push(undo);

                    maSV = ((DataRowView)bdsSinhVien[bdsSinhVien.Position])["MASV"].ToString(); // giữ lại để khi xóa bij lỗi thì ta sẽ quay về lại
                    bdsSinhVien.RemoveCurrent();
                    this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.SinhVienTableAdapter.Update(this.DS.SINHVIEN);
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

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!checkEmtyInfo()) return;

            switch (choose)
            {
                case THEM:
                    {
                        if (Program.conn.State == ConnectionState.Closed)
                            Program.conn.Open();

                        String strLenh = "sp_KTMaSV";
                        Program.sqlcmd = Program.conn.CreateCommand();
                        Program.sqlcmd.CommandType = CommandType.StoredProcedure;
                        Program.sqlcmd.CommandText = strLenh;
                        Program.sqlcmd.Parameters.Add("@MASV", SqlDbType.Text).Value = txtMASV.Text;
                        Program.sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        Program.sqlcmd.ExecuteNonQuery();
                        String Ret = Program.sqlcmd.Parameters["@Ret"].Value.ToString();

                        if (Ret.Equals("1"))
                        {
                            MessageBox.Show("Mã sinh viên bị trùng!", "", MessageBoxButtons.OK);
                            txtMaLop.Focus();
                            Program.conn.Close();
                            return;
                        }

                        try
                        {
                            bdsSinhVien.EndEdit();
                            bdsSinhVien.ResetCurrentItem();
                            this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
                            this.SinhVienTableAdapter.Update(this.DS.SINHVIEN);

                            String lenhXoaSV = "exec sp_XoaSinhVien '" + txtMASV.Text + "'";
                            UndoSinhVien undoSV = new UndoSinhVien(THEM, lenhXoaSV);
                            st.Push(undoSV);
                            //capNhatBtnPhucHoi();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi ghi sinh viên.\n" + ex.Message, "", MessageBoxButtons.OK);
                            return;
                        }
                        gcSinhVien.Enabled = true;
                        btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = true;
                        btnGhi.Enabled = false;
                        groupBox1.Enabled = false;
                        capNhatBtnPhucHoi();
                        break;
                    }

                case HIEU_CHINH:
                    {

                        try
                        {
                            bdsSinhVien.EndEdit();
                            bdsSinhVien.ResetCurrentItem();
                            this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
                            this.SinhVienTableAdapter.Update(this.DS.SINHVIEN);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi hiệu chỉnh sinh viên.\n" + ex.Message, "", MessageBoxButtons.OK);
                            return;
                        }
                        gcSinhVien.Enabled = true;
                        btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = true;
                        btnGhi.Enabled = btnPhucHoi.Enabled = false;
                        groupBox1.Enabled = false;
                        capNhatBtnPhucHoi();
                        break;
                    }
                   
            }
        }

        private SinhVien getInfoSinhVien()
        {
            SinhVien sv = new SinhVien();
            sv.setMaSV(txtMASV.Text.ToString());
            sv.setHo(txtHo.Text.ToString());
            sv.setTen(txtTen.Text.ToString());
            sv.setMaLop(txtMaLop.Text.ToString());
            sv.setNgaySinh(txtNgaySinh.DateTime);
            sv.setNoiSinh(txtNoiSinh.Text.ToString());
            sv.setDiaChi(txtDiaChi.Text.ToString());
            sv.setPhai(chkPhai.Checked);
            sv.setNghiHoc(chkNghiHoc.Checked);
            return sv;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bdsSinhVien.CancelEdit();
            if (btnThem.Enabled == false) bdsSinhVien.Position = vitri;
            gcSinhVien.Enabled = true;
            groupBox1.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            if (st.Count == 0) return;

            UndoSinhVien objUndo = (UndoSinhVien)st.Pop();
            Object obj = objUndo.getObject();
            switch (objUndo.getType())
            {
                case THEM:
                    Program.ExecSqlDataReader(obj.ToString());
                    this.LopTableAdapter.Fill(this.DS.LOP);
                    capNhatBtnPhucHoi();
                    break;
                case HIEU_CHINH:
                    Lop lopHieuChinh = (Lop)obj;
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strPhucHoiHieuChinh = "sp_PhucHoiLopHieuChinh";
                    Program.sqlcmd = Program.conn.CreateCommand();
                    Program.sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.sqlcmd.CommandText = strPhucHoiHieuChinh;
                    Program.sqlcmd.Parameters.Add("@MALOP", SqlDbType.Text).Value = lopHieuChinh.maLop;
                    Program.sqlcmd.Parameters.Add("@TENLOP", SqlDbType.Text).Value = lopHieuChinh.tenLop;
                    Program.sqlcmd.ExecuteNonQuery();
                    Program.conn.Close();
                    reload();
                    break;
                case XOA:
                    Lop lopXoa = (Lop)obj;
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strPhucHoiXoa = "sp_ThemLop";
                    Program.sqlcmd = Program.conn.CreateCommand();
                    Program.sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.sqlcmd.CommandText = strPhucHoiXoa;
                    Program.sqlcmd.Parameters.Add("@MALOP", SqlDbType.Text).Value = lopXoa.maLop;
                    Program.sqlcmd.Parameters.Add("@TENLOP", SqlDbType.Text).Value = lopXoa.tenLop;
                    Program.sqlcmd.Parameters.Add("@MAKHOA", SqlDbType.Text).Value = lopXoa.maKhoa;
                    Program.sqlcmd.ExecuteNonQuery();
                    reload();
                    break;
            }
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

        private void reload()
        {
            try
            {
                this.SinhVienTableAdapter.Fill(this.DS.SINHVIEN);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload :" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }
    }

    public class UndoSinhVien
    {
        private int type;
        private Object obj;

        public UndoSinhVien() { }

        public UndoSinhVien(int type, Object obj)
        {
            this.type = type;
            this.obj = obj;
        }

        public int getType()
        {
            return type;
        }

        public Object getObject()
        {
            return obj;
        }
    }

    public class SinhVien
    {
        private String maSV;
        private String ho;
        private String ten;
        private String maLop;
        private DateTime ngaySinh;
        private String noiSinh;
        private String diaChi;
        private bool phai;
        private bool nghiHoc;

        public String getMaSV() { return maSV; }

        public void setMaSV(String maSV) { this.maSV = maSV; }

        public String getHo() { return ho; }
        public void setHo(String ho) { this.ho = ho; }
        public String getTen() { return ten; }
        public void setTen(String ten) { this.ten = ten; }
        public String getMaLop() { return maLop; }
        public void setMaLop(String maLop) { this.maLop = maLop; }
        public DateTime getNgaySinh() { return ngaySinh; }
        public void setNgaySinh(DateTime ngaySinh) { this.ngaySinh = ngaySinh; }
        public String getNoiSinh() { return noiSinh; }
        public void setNoiSinh(String noiSinh) { this.noiSinh = noiSinh; }
        public String getDiaChi() { return diaChi; }
        public void setDiaChi(String diaChi) { this.diaChi = diaChi; }
        public bool isPhai() { return phai; }
        public void setPhai(bool phai) { this.phai = phai; }
        public bool isNghiHoc() { return nghiHoc; }
        public void setNghiHoc(bool nghiHoc) { this.nghiHoc = nghiHoc; }

    }
}
