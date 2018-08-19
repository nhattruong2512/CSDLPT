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
        bool isDangThaoTac = false;

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
          
        }

        private void frmSinhVien_Load(object sender, EventArgs e)
        {
            DS.EnforceConstraints = false;
           
            this.LopTableAdapter.Connection.ConnectionString = Program.connstr;
            this.LopTableAdapter.Fill(this.DS.LOP);

            this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.SinhVienTableAdapter.Fill(this.DS.SINHVIEN);

            if (bdsSinhVien.Count > 0)
            {
                maLop = ((DataRowView)bdsSinhVien[0])["MALOP"].ToString();
            }
            
            this.LopTableAdapter.Update(this.DS.LOP);
            this.SinhVienTableAdapter.Update(this.DS.SINHVIEN);
            cmbKhoa.DataSource = Program.bds_dspm;  // sao chép bds_dspm đã load ở form đăng nhập  qua
            cmbKhoa.DisplayMember = "TENKHOA";
            cmbKhoa.ValueMember = "TENSERVER";
            cmbKhoa.SelectedIndex = Program.mChinhanh;
            cmbLop.DataSource = this.bdsLop;
            cmbLop.SelectedItem = 1;
            cmbLop.SelectedItem = 0;

            updateCombobox();

        }

        private void updateCombobox()
        {
            if (Program.mGroup == "PGV")
                cmbKhoa.Enabled = true;
            else
                cmbKhoa.Enabled = false;
            if (Program.mGroup == "PGV" || Program.mGroup == "Khoa")
                cmbLop.Enabled = true;
            else
                cmbLop.Enabled = false;
        }

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


        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsSinhVien.Position;
            cmbKhoa.Enabled = cmbLop.Enabled = false;
            groupBox1.Enabled = true;
            bdsSinhVien.AddNew();
            txtMaLop.Text = maLop;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = txtMASV.Enabled = true;
            txtMaLop.Enabled = false;
            gcSinhVien.Enabled = false;
            choose = THEM;
            isDangThaoTac = true;
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
            isDangThaoTac = true;

            UndoSinhVien undoSV = new UndoSinhVien(HIEU_CHINH, getInfoSinhVien());
            st.Push(undoSV);
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
                MessageBox.Show("Họ sinh viên rỗng!");
                return false;
            }
            if (txtTen.Text.Trim() == "")
            {
                MessageBox.Show("Tên sinh viên rỗng");
                return false;
            }
            if (txtMaLop.Text.Trim() == "")
            {
                MessageBox.Show("Mã lớp rỗng!");
                return false;
            }
            if (txtSinh.Text.Trim() == "")
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
                   
                    UndoSinhVien undo = new UndoSinhVien(XOA, getInfoSinhVien());
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
                        if (txtMASV.Text.Trim() == "")
                        {
                            MessageBox.Show("Mã sinh viên không được thiếu!", "", MessageBoxButtons.OK);
                            txtMASV.Focus();
                            return;
                        }

                        if (txtHo.Text.Trim() == "")
                        {
                            MessageBox.Show("Họ sinh viên không được thiếu!", "", MessageBoxButtons.OK);
                            txtHo.Focus();
                            return;
                        }

                        if (txtTen.Text.Trim() == "")
                        {
                            MessageBox.Show("Tên sinh viên không được thiếu!", "", MessageBoxButtons.OK);
                            txtTen.Focus();
                            return;
                        }

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
                        isDangThaoTac = false;
                        break;
                    }

                case HIEU_CHINH:
                    {
                        if (txtHo.Text.Trim() == "")
                        {
                            MessageBox.Show("Họ sinh viên không được thiếu!", "", MessageBoxButtons.OK);
                            txtHo.Focus();
                            return;
                        }

                        if (txtTen.Text.Trim() == "")
                        {
                            MessageBox.Show("Tên sinh viên không được thiếu!", "", MessageBoxButtons.OK);
                            txtTen.Focus();
                            return;
                        }

                        try
                        {
                            bdsSinhVien.EndEdit();
                            bdsSinhVien.ResetCurrentItem();
                            this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
                            this.SinhVienTableAdapter.Update(this.DS.SINHVIEN);

                        }
                        catch (Exception ex)
                        {
                            st.Pop();
                            MessageBox.Show("Lỗi hiệu chỉnh sinh viên.\n" + ex.Message, "", MessageBoxButtons.OK);
                            return;
                        }
                        gcSinhVien.Enabled = true;
                        btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = true;
                        btnGhi.Enabled = btnPhucHoi.Enabled = false;
                        groupBox1.Enabled = false;
                        capNhatBtnPhucHoi();
                        isDangThaoTac = false;
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
            sv.setNgaySinh(txtSinh.DateTime);
            sv.setNoiSinh(txtNoiSinh.Text.ToString());
            sv.setDiaChi(txtDiaChi.Text.ToString());
            sv.setPhai(chkPhai.Checked);
            sv.setNghiHoc(chkNghiHoc.Checked);
            return sv;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gcSinhVien.Enabled = true;
            groupBox1.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = false;

            updateCombobox();

            if (isDangThaoTac)
            {
                if (choose == HIEU_CHINH)
                {
                    st.Pop();
                    reload();
                }
                else
                {
                    bdsSinhVien.RemoveCurrent();
                }
                capNhatBtnPhucHoi();
                isDangThaoTac = false;
                return;
            }

            this.SinhVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.SinhVienTableAdapter.Update(this.DS.SINHVIEN);

            bdsSinhVien.CancelEdit();
            if (btnThem.Enabled == false) bdsSinhVien.Position = vitri;

            if (st.Count == 0)
            {
                btnPhucHoi.Enabled = false;
                return;
            }

            UndoSinhVien objUndo = (UndoSinhVien)st.Pop();
            Object obj = objUndo.getObject();

            if (st.Count > 0)
            {
                btnPhucHoi.Enabled = true;
            }

            switch (objUndo.getType())
            {
                case THEM:
                    Program.ExecSqlDataReader(obj.ToString());
                    this.SinhVienTableAdapter.Fill(this.DS.SINHVIEN);
                    capNhatBtnPhucHoi();
                    break;
                case HIEU_CHINH:
                    SinhVien svHieuChinh = (SinhVien)obj;
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();

                    String sqlHieuChinh =
                        "exec sp_PhucHoiSinhVien N'" +
                        svHieuChinh.getMaSV() + "',N'" +
                        svHieuChinh.getHo() + "',N'" +
                        svHieuChinh.getTen() + "',N'" +
                        svHieuChinh.isPhai() + "',N'" +
                        svHieuChinh.getNgaySinh() + "',N'" + 
                        svHieuChinh.getNoiSinh() + "',N'" + 
                        svHieuChinh.getDiaChi() + "',N'" + 
                        svHieuChinh.isNghiHoc() + "'";
                    Program.ExecSqlDataTable(sqlHieuChinh);
                    Program.conn.Close();
                    reload();
                    break;
                case XOA:
                    SinhVien svXoa = (SinhVien)obj;
                    bdsSinhVien.EndEdit();
                    bdsSinhVien.ResetCurrentItem();
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strPhucHoiXoa = "exec sp_ThemSinhVien N'" +
                        svXoa.getMaSV() + "',N'" +
                        svXoa.getHo() + "',N'" +
                        svXoa.getTen() + "',N'" +
                        svXoa.getMaLop() + "',N'" +
                        svXoa.isPhai() + "',N'" +
                        svXoa.getNgaySinh() + "',N'" +
                        svXoa.getNoiSinh() + "',N'" +
                        svXoa.getDiaChi() + "',N'" +
                        svXoa.isNghiHoc() + "'";
                    Program.ExecSqlDataTable(strPhucHoiXoa);
                    Program.conn.Close();

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

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void btnInDSSV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmBaoCaoDSSV f = new frmBaoCaoDSSV();
            f.setMaLop(cmbLop.SelectedValue.ToString());
            f.setTenLop(cmbLop.Text);
            f.ShowDialog();
        }

        private void bdsSinhVien_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                //this.SinhVienTableAdapter.FillBy(this.DS.SINHVIEN);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void bdsLop_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void chkPhai_CheckedChanged(object sender, EventArgs e)
        {
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
