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
        public frmBaoCaoDSSV()
        {
            InitializeComponent();
        }

        

        private void frmBaoCaoDSSV_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dS.LOP' table. You can move, or remove it, as needed.
            dS.EnforceConstraints = false;
            this.lOPTableAdapter.Connection.ConnectionString = Program.connstr;
            this.lOPTableAdapter.Fill(this.dS.LOP);

        }
        private void btnReport_Click(object sender, EventArgs e)
        {
            ReportDSSV rp = new ReportDSSV();

            string strLenh;
            DataTable MyTable;
            strLenh = "EXEC sp_DanhSachSinhVien '" + comboBox1.SelectedValue.ToString().Trim() + "'";
            
            MyTable = Program.ExecSqlDataTable(strLenh);

            rp.SetDataSource(MyTable);

            crystalReportViewer1.ReportSource = rp;

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
