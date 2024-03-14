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
	public partial class frmUpdatePersonalPopUp : Form
	{
		#region ilgili verilerin diğer formdan çekilmesi
		public frmUpdatePersonalPopUp(int _SelectedPersonalID, string _SelectedPersonalNumber, string _SelectedPersonalName, string _SelectedPersonalSurname, string _SelectedPersonalDistrict, string _SelectedPersonalCity, string _SelectedPersonalAddress, DateTime _SelectedPersonalBirthDate, DateTime _SelectedPersonalEntryDate, bool _SelectedPersonalStatus)
		{
			InitializeComponent();

			txtID.Text = _SelectedPersonalID.ToString();
			txtNumber.Text = _SelectedPersonalNumber.ToString();
			txtName.Text = _SelectedPersonalName.ToString();
			txtSurname.Text = _SelectedPersonalSurname.ToString();
			txtDistrict.Text = _SelectedPersonalDistrict.ToString();
			txtCity.Text = _SelectedPersonalCity.ToString();
			txtAddress.Text = _SelectedPersonalAddress.ToString();
			txtBirthDate.Text = _SelectedPersonalBirthDate.ToString();
			txtEntryDate.Text = _SelectedPersonalEntryDate.ToString();
			if (_SelectedPersonalStatus == true)
			{
				rbActive.Checked = true;
			}
			else
			{
				rbLeft.Checked = false;
			}

			txtID.Enabled = false;
			txtNumber.Enabled = false;
		}
		#endregion

		//Güncelleme butonu
		private void btnUpdateAll_Click(object sender, EventArgs e)
		{
			if (InputControl.Check(this.Controls) && (rbActive.Checked || rbLeft.Checked))
			{
				UpdateInfos();
			}
		}

		#region Personel bilgilerinin güncellendiği metot
		private void UpdateInfos()
		{
			using (SqlConnection connection = Database.GetConnection())
			{
				string UpdateQuery = "UPDATE tbl_Personals SET PersonalName = @NewName, PersonalSurname=@NewSurname,PersonalDistrict = @NewDistrict, PersonalCity = @NewCity,PersonalAddress = @NewAddress, PersonalBirthDate = @NewBirthDate, PersonalEntryDate = @NewEntryDate, PersonalStatus = @NewStatus WHERE PersonalID = @ID";
				using (SqlCommand cmd = new SqlCommand(UpdateQuery, connection))
				{
					cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(txtID.Text));
					cmd.Parameters.AddWithValue("@NewName", txtName.Text);
					cmd.Parameters.AddWithValue("@NewSurname", txtSurname.Text);
					cmd.Parameters.AddWithValue("@NewDistrict", txtDistrict.Text);
					cmd.Parameters.AddWithValue("@NewCity", txtCity.Text);
					cmd.Parameters.AddWithValue("@NewAddress", txtAddress.Text);
					cmd.Parameters.AddWithValue("@NewBirthDate", Convert.ToDateTime(txtBirthDate.Text));
					cmd.Parameters.AddWithValue("@NewEntryDate", Convert.ToDateTime(txtEntryDate.Text));

					if (rbActive.Checked == true)
					{
						cmd.Parameters.AddWithValue("@NewStatus", true);
					}
					else if (rbLeft.Checked == true)
					{
						cmd.Parameters.AddWithValue("@NewStatus", false);
					}

					connection.Open();
					cmd.ExecuteNonQuery();
					MessageBox.Show($"{txtNumber.Text} numaralı personelin bilgileri başarı ile güncellendi.", "Güncelleme İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}
		#endregion

		//Ana sayfa
		private void btnMain_Click(object sender, EventArgs e)
		{
			frmMain MAIN = new frmMain();
			MAIN.Show();
			this.Hide();
		}
	}
}
