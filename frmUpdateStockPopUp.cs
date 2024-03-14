using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace productManager
{
	public partial class frmUpdateStockPopUp : Form
	{
		#region Yapıcı metodumuzda çekilen verilerin ilgili alanlara atanması ve ilgili kontrollerin gerçekleştirilmesi
		public frmUpdateStockPopUp(int _SelectedProductID, int _SelectedProductQuantity, string _SelectedProductNumber, string _SelectedProductName, DateTime _SelectedProductEntryDate, DateTime _SelectedProductDeleteDate, DateTime _SelectedProductUpdateDate, decimal _SelectedProductPrice, bool _SelectedProductStatus)
		{
			InitializeComponent();
			txtID.Text = _SelectedProductID.ToString();
			numericQuantity.Value = _SelectedProductQuantity;
			txtNumber.Text = _SelectedProductNumber;
			txtName.Text = _SelectedProductName;
			txtEntryDate.Text = _SelectedProductEntryDate.ToString();
			numericPrice.Value = _SelectedProductPrice;

			//Önceden silme işleminin yapılıp yapılmadığının kontrolü
			if (_SelectedProductDeleteDate.ToString() == "1.01.2000 00:00:00")
			{
				txtDeleteDate.Text = "Henüz bir işlem yapılmadı.";
				txtDeleteDate.Enabled = false;
			}
			else
			{
				txtDeleteDate.Text = _SelectedProductDeleteDate.ToString();
			}

			//Önceden güncelleme işleminin yapılıp yapılmadığının kontrolü
			if (_SelectedProductUpdateDate.ToString() == "1.01.2000 00:00:00")
			{
				txtUpdateDate.Text = "Henüz bir işlem yapılmadı.";
			}
			else
			{
				txtUpdateDate.Text = _SelectedProductUpdateDate.ToString();
			}

			//Silinmiş mi kontrol ederek ilgili radiobuttonların atanması
			if (_SelectedProductStatus == true)
			{
				rbActive.Checked = true;
			}
			else
			{
				rbDeleted.Checked = true;
			}
		}
		#endregion

		//Load metodunda TextBox ayarlarının yapılması
		private void frmUpdatePopUp_Load(object sender, EventArgs e)
		{
			txtID.Enabled = false;
			txtNumber.Enabled = false;
			txtUpdateDate.Enabled = false;
		}

		#region Güncelleme işleminin tamamlandığı metot
		private void UpdateAll()
		{
			using (SqlConnection connect = Database.GetConnection())
			{
				//İlgili güncelleme sorgusu
				string UpdateQuery = "UPDATE tbl_Products SET ProductName = @NewName, ProductEntryDate = @NewEntryDate , ProductPrice = @NewPrice ,ProductQuantity = @NewQuantity , ProductUpdateDate = @NewUpdateDate , ProductDeleteDate = @NewDeleteDate , ProductStatus = @NewStatus WHERE ProductID = @ID";
				using (SqlCommand cmd = new SqlCommand(UpdateQuery, connect))
				{
					//İlgili parametrelere değerlerin atanması işlemi
					cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(txtID.Text));
					cmd.Parameters.AddWithValue("@NewName", txtName.Text);
					cmd.Parameters.AddWithValue("@NewEntryDate", Convert.ToDateTime(txtEntryDate.Text));
					cmd.Parameters.AddWithValue("@NewPrice", Convert.ToDecimal(numericPrice.Value));
					cmd.Parameters.AddWithValue("@NewQuantity", numericQuantity.Text);
					cmd.Parameters.AddWithValue("@NewUpdateDate", DateTime.Now);

					//RadioButtonlar üzerinden silinme işleminin gerçekleşip gerçekleşmediğinin kontrolü ve buna bağlı DateTime ataması
					if (rbActive.Checked)
					{
						cmd.Parameters.AddWithValue("@NewStatus", true);
						cmd.Parameters.AddWithValue("@NewDeleteDate", "2000-01-01 00:00:00.000");
					}
					else
					{
						cmd.Parameters.AddWithValue("@NewStatus", false);
						if (txtDeleteDate.Text == "Henüz bir işlem yapılmadı.")
						{
							cmd.Parameters.AddWithValue("@NewDeleteDate", DateTime.Now);
						}
					}

					connect.Open();

					cmd.ExecuteNonQuery();

					MessageBox.Show($"{txtNumber.Text} numaralı ürün bilgisi başarıyla güncellendi.", "Güncelleme İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}
		#endregion

		//Güncelleme metodunun çağrılıp, işlemin tamamlanması
		private void btnUpdate_Click(object sender, EventArgs e)
		{
			if (InputControl.Check(this.Controls) && (rbActive.Checked || rbDeleted.Checked))
			{
				UpdateAll();
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
