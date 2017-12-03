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
    public partial class frmBaoCaoDanhSachDiem : Form
    {
        private string maLop;
        private string tenLop;
        private string maMonHoc;
        private string tenMonHoc;
        private string lanThi;

        public void setMaLop(string maLop) { this.maLop = maLop; }
        public string getMaLop() { return maLop; }

        public void setTenLop(string tenLop) { this.tenLop = tenLop; }
        public string getTenLop() { return tenLop; }

        public void setMaMonHoc(string maMonHoc) { this.maMonHoc = maMonHoc; }
        public string getMaMonHoc() { return maMonHoc; }

        public void seTenMonHoc(string tenMonHoc) { this.tenMonHoc = tenMonHoc; }
        public string getTenMonHoc() { return tenMonHoc; }

        public void setLanThi(string lanThi) { this.lanThi = lanThi; }

        public string getLanThi(string lanThi) { return lanThi; }

        public frmBaoCaoDanhSachDiem()
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            string strLenh;
            DataTable MyTable;
            strLenh = "EXEC sp_BangDiem '" + maLop + "','" + maMonHoc + "','" + lanThi + "'";

            MyTable = Program.ExecSqlDataTable(strLenh);

            rptDanhSachDiem rp = new rptDanhSachDiem();
            rp.SetDataSource(MyTable);
            crystalReportViewer1.ReportSource = rp;
        }
    }
}
