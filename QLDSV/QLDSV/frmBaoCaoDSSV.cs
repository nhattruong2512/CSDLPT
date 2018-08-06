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
    public partial class frmBaoCaoDSSV : Form
    {
        private string maLop = "";
        private string tenLop = "";

        public frmBaoCaoDSSV()
        {
            InitializeComponent();
        }

        public void setMaLop(string maLop) { this.maLop = maLop; }

        public string getMaLop() { return maLop; }

        public void setTenLop(string tenLop) { this.tenLop = tenLop; }

        public string getTenLop() { return tenLop; }

        private void frmBaoCaoDSSV_Load(object sender, EventArgs e)
        {
            string strLenh;
            DataTable MyTable;
            strLenh = "EXEC sp_InDsSinhVienTheoLop '" + maLop + "'";

            MyTable = Program.ExecSqlDataTable(strLenh);

            ReportDSSV rp = new ReportDSSV();
            rp.SetDataSource(MyTable);
            rp.SetParameterValue("TENLOP", tenLop);
            crystalReportViewer1.ReportSource = rp;

        }
    }
}
