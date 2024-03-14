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
	public partial class frmAddPersonal : Form
	{
		public frmAddPersonal()
		{
			InitializeComponent();
		}

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			if (InputControl.Check(this.Controls))
			{
				AddPersonalMethod(); //Personel ekleme işleminin tamamlandığı method
			}
		}

		private void AddPersonalMethod()
		{
			#region İlgili verilerin formdan çekilmesi
			string PName = txtName.Text;
			string PSurname = txtSurname.Text;
			string PDistrict = txtDistrict.Text;
			string PCity = txtCity.Text;
			string PAddress = txtAddress.Text;
			DateTime PBirthDate = Convert.ToDateTime(txtBirthDate.Text);
			DateTime PEntryDate = DateTime.Now;

			//Benzersiz bir personel numarası oluşturmak için kullanılan sınıfa başvuru
			string PNumber = CreatePersonalNumber.Create(PBirthDate.Year, PEntryDate.Year);
			#endregion

			#region Kullanıcıdan alınan verilerin veritabanına kayıt edilmesi işlemleri
			using (SqlConnection connect = Database.GetConnection())
			{
				//Personel bilgilerini ekleyen SQL sorgusu
				string Query = "INSERT INTO tbl_Personals (PersonalNumber,PersonalName,PersonalSurname,PersonalDistrict,PersonalCity,PersonalAddress,PersonalBirthDate,PersonalEntryDate) VALUES (@PNumber,@PName,@PSurname,@PDistrict,@PCity,@PAddress,@PBirthDate,@PEntryDate)";
				using (SqlCommand cmd = new SqlCommand(Query, connect))
				{
					try
					{
						cmd.Parameters.AddWithValue("@PNumber", PNumber);
						cmd.Parameters.AddWithValue("@PName", PName);
						cmd.Parameters.AddWithValue("@PSurname", PSurname);
						cmd.Parameters.AddWithValue("@PDistrict", PDistrict);
						cmd.Parameters.AddWithValue("@PCity", PCity);
						cmd.Parameters.AddWithValue("@PAddress", PAddress);
						cmd.Parameters.AddWithValue("@PBirthDate", PBirthDate);
						cmd.Parameters.AddWithValue("@PEntryDate", PEntryDate);

						connect.Open();

						cmd.ExecuteNonQuery();
						MessageBox.Show("Başarılı");
					}
					catch (Exception ex) //Hatalı durumda hata mesajını gösterecek olan kısım
					{
						MessageBox.Show("HATA : " + ex);
					}
				}
			}
			#endregion
		}

		private void btnMainForm_Click(object sender, EventArgs e)
		{
			frmMain mainForm = new frmMain();
			mainForm.Show();
			this.Hide();
		}
	}
}
