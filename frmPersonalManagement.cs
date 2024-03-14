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

namespace productManager
{
	public partial class frmPersonalManagement : Form
	{
		public frmPersonalManagement()
		{
			InitializeComponent();
		}

		#region Personal bilgilerinin anlık olarak çekilmesi ve datagrid'e yazılması
		private void SearchPersonal()
		{
			using (SqlConnection connection = Database.GetConnection())
			{
				string Query = "SELECT * FROM tbl_Personals WHERE PersonalStatus = @PStatus AND (PersonalNumber LIKE '%' + @Personal + '%' OR PersonalName LIKE '%' + @Personal + '%' OR PersonalSurname LIKE '%' + @Personal +'%')";
				using (SqlCommand cmd = new SqlCommand(Query, connection))
				{
					//Verinin Sql sorgusu ile aranıp bulunması işlemi
					cmd.Parameters.AddWithValue("@PStatus", true);
					cmd.Parameters.AddWithValue("@Personal", txtSearchForPersonal.Text);

					connection.Open();

					//Gelen verilerin anlık olarak DataGridView'e yazılması
					SqlDataAdapter da = new SqlDataAdapter(cmd);
					DataSet ds = new DataSet();

					da.Fill(ds);

					datagridPersonal.DataSource = ds.Tables[0];
				}
			}
		}
		#endregion

		//Personal bilgilerinin anlık olarak SearhPersonal metodu çağrılarak çekilmesi
		private void txtSearchForPersonal_TextChanged(object sender, EventArgs e)
		{
			SearchPersonal();
		}

		#region Seçilen personel bilgilerinin atanması ve eşleştirilmesi
		int SelectedPersonalID;
		string SelectedPersonalNumber, SelectedPersonalName, SelectedPersonalSurname, SelectedPersonalDistrict, SelectedPersonalCity, SelectedPersonalAddress;
		DateTime SelectedPersonalBirthDate, SelectedPersonalEntryDate;
		bool SelectedPersonalStatus;
		private void datagridPersonal_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			int selectedIndex = datagridPersonal.SelectedCells[0].RowIndex;
			SelectedPersonalID = Convert.ToInt32(datagridPersonal.Rows[selectedIndex].Cells["PersonalID"].Value);
			SelectedPersonalNumber = datagridPersonal.Rows[selectedIndex].Cells["PersonalNumber"].Value.ToString();
			SelectedPersonalName = datagridPersonal.Rows[selectedIndex].Cells["PersonalName"].Value.ToString();
			SelectedPersonalSurname = datagridPersonal.Rows[selectedIndex].Cells["PersonalSurname"].Value.ToString();
			SelectedPersonalDistrict = datagridPersonal.Rows[selectedIndex].Cells["PersonalDistrict"].Value.ToString();
			SelectedPersonalCity = datagridPersonal.Rows[selectedIndex].Cells["PersonalCity"].Value.ToString();
			SelectedPersonalAddress = datagridPersonal.Rows[selectedIndex].Cells["PersonalAddress"].Value.ToString();
			SelectedPersonalBirthDate = Convert.ToDateTime(datagridPersonal.Rows[selectedIndex].Cells["PersonalBirthDate"].Value);
			SelectedPersonalEntryDate = Convert.ToDateTime(datagridPersonal.Rows[selectedIndex].Cells["PersonalEntryDate"].Value);
			SelectedPersonalStatus = Convert.ToBoolean(datagridPersonal.Rows[selectedIndex].Cells["PersonalStatus"].Value);
		}
		#endregion


		//Güncelleme popup'ının açılması
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			if (SelectedPersonalID != 0) //Seçim yapılmış mı kontrolü
			{
				frmUpdatePersonalPopUp Update = new frmUpdatePersonalPopUp(SelectedPersonalID, SelectedPersonalNumber, SelectedPersonalName, SelectedPersonalSurname, SelectedPersonalDistrict, SelectedPersonalCity, SelectedPersonalAddress, SelectedPersonalBirthDate, SelectedPersonalEntryDate, SelectedPersonalStatus);
				Update.ShowDialog();
			}
			else
			{
				MessageBox.Show("Lütfen bir personel seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		//Ana sayfa
		private void btnMain_Click(object sender, EventArgs e)
		{
			frmMain MAIN = new frmMain();
			MAIN.Show();
			this.Hide();
		}

	}
}
