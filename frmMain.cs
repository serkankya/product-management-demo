using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace productManager
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private void btnAddProduct_Click(object sender, EventArgs e)
		{
			frmAddProduct ADD = new frmAddProduct();
			ADD.Show();
			this.Hide();
		}

		private void btnAddPersonal_Click(object sender, EventArgs e)
		{
			frmAddPersonal ADD = new frmAddPersonal();
			ADD.Show();
			this.Hide();
		}

		private void btnMakeSale_Click(object sender, EventArgs e)
		{
			frmSalePage SALE = new frmSalePage();
			SALE.Show();
			this.Hide();
		}

		private void btnStockControl_Click(object sender, EventArgs e)
		{
			frmStockControl STOCK = new frmStockControl();
			STOCK.Show();
			this.Hide();
		}

		private void btnPersonalManagement_Click(object sender, EventArgs e)
		{
			frmPersonalManagement PM = new frmPersonalManagement();
			PM.Show();
			this.Hide();
		}
	}
}
