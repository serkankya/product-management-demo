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
	public partial class frmAddProduct : Form
	{
		public frmAddProduct()
		{
			InitializeComponent();
		}
		//Ürünün eklenmesinin tamamlanması ve kontroller
		private void btnSubmit_Click(object sender, EventArgs e)
		{
			if (InputControl.Check(this.Controls))
			{
				if (numPrice.Value == 0)
				{
					DialogResult res = MessageBox.Show($"{txtProductName.Text} ürününün fiyatını 0 TL olarak belirlemek istediğinize emin misiniz ?", "Ürün Fiyatı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (res == DialogResult.Yes)
					{
						AddProductMethod(); //Ürün ekleme işleminin tamamlandığı method
					}
					else
					{
						MessageBox.Show("İşlem iptal edildi.", "İşlem İptali", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void AddProductMethod()
		{
			#region İlgili verilerin formdan çekilmesi
			string PName = txtProductName.Text;
			DateTime PDate = DateTime.Now;
			decimal PPrice = numPrice.Value;
			int PQuantity = Convert.ToInt32(numQuantity.Value);

			//Benzersiz bir ürün numarası oluşturmak için kullanılan sınıfa başvuru
			string PNumber = CreateProductNumber.Create(PDate.Year);
			#endregion

			#region Kullanıcıdan alınan verilerin veritabanına kayıt edilmesi işlemleri
			using (SqlConnection connect = Database.GetConnection())
			{
				//Ürünleri ekleyen SQL Sorgusu
				string Query = "INSERT INTO tbl_Products (ProductNumber,ProductName,ProductEntryDate,ProductPrice,ProductQuantity) VALUES (@PNumber,@PName,@PDate,@PPrice,@PQuantity)";
				using (SqlCommand cmd = new SqlCommand(Query, connect))
				{
					try
					{
						cmd.Parameters.AddWithValue("@PNumber", PNumber);
						cmd.Parameters.AddWithValue("@PName", PName);
						cmd.Parameters.AddWithValue("@PDate", PDate);
						cmd.Parameters.AddWithValue("@PPrice", PPrice);
						cmd.Parameters.AddWithValue("@PQuantity", PQuantity);

						connect.Open();

						cmd.ExecuteNonQuery();
						MessageBox.Show("Ürün başarı ile eklendi.", "Ürün Ekleme", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					catch (Exception ex) //Hatalı durumda hata mesajını gösterecek olan kısım
					{
						MessageBox.Show("Hata : " + ex, "HATA");
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

