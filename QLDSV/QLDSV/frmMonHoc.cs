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
    public partial class frmMonHoc : Form
    {
        const int THEM = 0;
        const int HIEU_CHINH = 1;
        const int XOA = 2;

        public Stack st = new Stack();
        int choose = -1;
        int vitri = 0;
        string maMH = "";
        string tenMH;
        bool isDangThem = false;

        public frmMonHoc()
        {
            InitializeComponent();
        }

        private void mONHOCBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsMonHoc.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dS);

        }

        private void frmMonHoc_Load(object sender, EventArgs e)
        {
            dS.EnforceConstraints = false;
            this.MonHocTableAdapter.Connection.ConnectionString = Program.connstr;
            // TODO: This line of code loads data into the 'dS.MONHOC' table. You can move, or remove it, as needed.
            this.MonHocTableAdapter.Fill(this.dS.MONHOC);
            this.MonHocTableAdapter.Update(this.dS.MONHOC);

            groupBox.Enabled = false;
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsMonHoc.Position;
            groupBox.Enabled = true;
            bdsMonHoc.AddNew();
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gcMonHoc.Enabled = false;
            txtMaMH.Enabled = true;
            txtMaMH.Focus();
            choose = THEM;
            isDangThem = true;
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (isDangThem)
            {
                capNhatBtnPhucHoi();
                reload();
                isDangThem = false;
            }
          
            bdsMonHoc.CancelEdit();
            if (btnThem.Enabled == false || btnHieuChinh.Enabled == false) bdsMonHoc.Position = vitri;
            gcMonHoc.Enabled = true;
            groupBox.Enabled = false;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = btnPhucHoi.Enabled = false;

            if (st.Count == 0) return;

            UndoMonHoc objUndo = (UndoMonHoc)st.Pop();
            Object obj = objUndo.getObj();
            switch (objUndo.getType())
            {
                case THEM:
                    {
                        Program.ExecSqlDataReader(obj.ToString());
                        this.MonHocTableAdapter.Fill(this.dS.MONHOC);
                        capNhatBtnPhucHoi();
                        break;
                    }
                case HIEU_CHINH:
                    {
                        if (Program.conn.State == ConnectionState.Closed)
                            Program.conn.Open();

                        MonHoc monHocHieuChinh = (MonHoc)obj;

                        String sqlHieuChinh = "exec sp_CapNhatMonHoc N'" + monHocHieuChinh.getMaMH() + "',N'" + monHocHieuChinh.getTenMH() + "'";
                        Program.ExecSqlDataTable(sqlHieuChinh);
                       
                        Program.conn.Close();
                        capNhatBtnPhucHoi();
                        reload();
                        break;
                    }
                case XOA:
                    MonHoc monHocXoa = (MonHoc)obj;
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();

                    string sql = "exec sp_ThemMonHoc N'" + monHocXoa.getMaMH() + "', N'" + monHocXoa.getTenMH() + "'";
                    Program.ExecSqlDataTable(sql);

                    reload();
                    break;
            }
        }

        private void btnHieuChinh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            vitri = bdsMonHoc.Position;
            groupBox.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            txtMaMH.Enabled = false;
            gcMonHoc.Enabled = false;
            tenMH = txtTenMH.Text;
            choose = HIEU_CHINH;

            MonHoc monHoc = new MonHoc(txtMaMH.Text.ToString(), txtTenMH.Text.ToString());
            UndoMonHoc undo = new UndoMonHoc(HIEU_CHINH, monHoc);
            st.Push(undo);
        }

        private void reload()
        {
            try
            {
                this.MonHocTableAdapter.Fill(this.dS.MONHOC);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Reload :" + ex.Message, "", MessageBoxButtons.OK);
                return;
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

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            switch (choose)
            {
                case THEM:
                    {
                        if (txtMaMH.Text.Trim() == "")
                        {
                            MessageBox.Show("Mã môn học rỗng!");
                            txtMaMH.Focus();
                            return;
                        }
                        if (txtTenMH.Text.Trim() == "")
                        {
                            MessageBox.Show("Tên môn học rỗng");
                            txtTenMH.Focus();
                            return;
                        }

                        if (Program.conn.State == ConnectionState.Closed)
                            Program.conn.Open();

                        String strLenh = "dbo.sp_KTMaMonHoc";
                        Program.sqlcmd = Program.conn.CreateCommand();
                        Program.sqlcmd.CommandType = CommandType.StoredProcedure;
                        Program.sqlcmd.CommandText = strLenh;
                        Program.sqlcmd.Parameters.Add("@X", SqlDbType.Text).Value = txtMaMH.Text;
                        Program.sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        Program.sqlcmd.ExecuteNonQuery();
                        String Ret = Program.sqlcmd.Parameters["@Ret"].Value.ToString();

                        if (Ret.Equals("1"))
                        {
                            MessageBox.Show("Mã môn học bị trùng!", "", MessageBoxButtons.OK);
                            txtMaMH.Focus();
                            Program.conn.Close();
                            return;
                        }

                        string sql = "exec sp_KiemTraTenMonHoc N'" + txtTenMH.Text + "'";
                        DataTable table = Program.ExecSqlDataTable(sql);

                        if (table.Rows.Count > 0)
                        {
                            MessageBox.Show("Tên môn học bị trùng!");
                            txtTenMH.Focus();
                            Program.conn.Close();
                            return;
                        }

                        try
                        {
                            bdsMonHoc.EndEdit(); //Kết thúc quá trình hiệu chỉnh và gởi các dữ liệu đã thay đổi về dữ liệu nguồn(Data Set)
                            bdsMonHoc.ResetCurrentItem();
                            this.MonHocTableAdapter.Connection.ConnectionString = Program.connstr;
                            this.MonHocTableAdapter.Update(this.dS.MONHOC);

                            String lenhXoaMH = "exec sp_XoaMonHoc '" + txtMaMH.Text + "'";
                            UndoMonHoc undo = new UndoMonHoc(THEM, lenhXoaMH);
                            st.Push(undo);
                            capNhatBtnPhucHoi();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi ghi lớp.\n" + ex.Message, "", MessageBoxButtons.OK);
                            return;
                        }
                        isDangThem = false;
                        updateViewMode();
                        break;
                    }
                case HIEU_CHINH:
                    {
                        if (isTenMonHocEmpty()) return;
                     
                        if (txtTenMH.Text.Equals(tenMH))
                        {
                            updateViewMode();
                            return;
                        }


                        if (Program.conn.State == ConnectionState.Closed)
                            Program.conn.Open();
                        String strKTTenMonHoc = "exec dbo.sp_KiemTraTenMonHoc N'" + txtTenMH.Text + "'";
                        DataTable table = Program.ExecSqlDataTable(strKTTenMonHoc);

                        if (table.Rows.Count > 0)
                        {
                            MessageBox.Show("Tên môn học bị trùng!");
                            txtTenMH.Focus();
                            return;
                        }

                        try
                        {
                            

                            bdsMonHoc.EndEdit();
                            bdsMonHoc.ResetCurrentItem();
                            this.MonHocTableAdapter.Connection.ConnectionString = Program.connstr;
                            this.MonHocTableAdapter.Update(this.dS.MONHOC);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi hiệu chỉnh môn học.\n" + ex.Message, "", MessageBoxButtons.OK);
                            return;
                        }

                        updateViewMode();
                        break;
                    }
            }
        }

        private void updateViewMode()
        {
            gcMonHoc.Enabled = true;
            btnThem.Enabled = btnHieuChinh.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = true;
            btnGhi.Enabled = false;

            groupBox.Enabled = false;
        }

        private bool isTenMonHocEmpty()
        {
            if (txtTenMH.Text.Trim() == "")
            {
                MessageBox.Show("Tên môn học không được thiếu!", "", MessageBoxButtons.OK);
                txtTenMH.Focus();
                return true;
            }
            return false;
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn xóa môn học này ?? ", "Xác nhận",
                       MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    UndoMonHoc undo = new UndoMonHoc(XOA, new MonHoc(txtMaMH.Text, txtTenMH.Text));
                    st.Push(undo);

                    maMH = ((DataRowView)bdsMonHoc[bdsMonHoc.Position])["MAMH"].ToString(); // giữ lại để khi xóa bij lỗi thì ta sẽ quay về lại
                    bdsMonHoc.RemoveCurrent();
                    bdsMonHoc.ResetCurrentItem();
                    this.MonHocTableAdapter.Connection.ConnectionString = Program.connstr;
                    this.MonHocTableAdapter.Update(this.dS.MONHOC);

                    capNhatBtnPhucHoi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa môn học. Bạn hãy xóa lại\n" + ex.Message, "",
                        MessageBoxButtons.OK);
                    this.MonHocTableAdapter.Fill(this.dS.MONHOC);
                    bdsMonHoc.Position = bdsMonHoc.Find("MAMH", maMH);
                    return;
                }
            }

            if (bdsMonHoc.Count == 0) btnXoa.Enabled = false;
        }

    }

    public class UndoMonHoc
    {
        private int type;
        private Object obj;

        public UndoMonHoc(int type, Object obj)
        {
            this.type = type;
            this.obj = obj;
        }

        public int getType() { return type; }
        public Object getObj() { return obj; }
    }

    public class MonHoc
    {
        private string maMH;
        private string tenMH;

        public MonHoc(string maMH, string tenMH){
            this.maMH = maMH;
            this.tenMH = tenMH;
        }
        public string getMaMH() { return maMH; }
        public string getTenMH() { return tenMH; }
    }
}
