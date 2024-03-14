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
	public partial class frmStockControl : Form
	{
		public frmStockControl()
		{
			InitializeComponent();
		}

		//Anlık olarak ürün aramasının gerçekleştirilmesi
		private void txtProductInfo_TextChanged(object sender, EventArgs e)
		{
			SearchProduct(); //İlgili metodun çağrılması
		}

		#region Ürün silme işleminin gerçekleştiği metot
		private void DeleteProduct()
		{
			using (SqlConnection connection = new SqlConnection())
			{
				string DeleteQuery = "UPDATE tbl_Products SET ProductStatus = @P_NewStatus WHERE ProductID = @P_ID FROM dbo_productManager";
				using (SqlCommand cmd = new SqlCommand(DeleteQuery, connection))
				{
					cmd.Parameters.AddWithValue("@P_NewStatus", false); //Durumu false'a çevrilerek, ilişkili tabloların zarar görmemesi sağlanıyor
					cmd.Parameters.AddWithValue("@P_ID", SelectedProductID);

					connection.Open();

					cmd.ExecuteNonQuery();

					MessageBox.Show($"{SelectedProductName} adlı ürün başarı ile silindi.", "Silme İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}
		#endregion

		#region Ürün arama işleminin anlık olarak gerçekleştiği metot
		private void SearchProduct()
		{
			using (SqlConnection connect = Database.GetConnection())
			{
				//Aktiflik durumu true olan ve aranan kelimeleri içeren verilerin çekilmesi
				string Query = "SELECT * FROM tbl_Products WHERE ProductStatus = @Status AND (ProductName LIKE '%' + @Words + '%' OR ProductNumber LIKE '%' + @Words + '%')";

				using (SqlCommand cmd = new SqlCommand(Query, connect))
				{
					cmd.Parameters.AddWithValue("@Status", true);
					cmd.Parameters.AddWithValue("@Words", txtProductInfo.Text);

					connect.Open();

					//Çekilen verilerin veri kaynağı olarak datagridview'e aktarılıp, listelenmesi
					SqlDataAdapter da = new SqlDataAdapter(cmd);
					DataSet ds = new DataSet();

					da.Fill(ds);

					dataGridProducts.DataSource = ds.Tables[0];
				}
			}
		}
		#endregion

		#region Atama işlemleri ve DataGridView'de tıklanan verinin bellekte tutulması işlemi
		int SelectedProductID;
		int SelectedProductQuantity;
		string SelectedProductName;
		string SelectedProductNumber;
		decimal SelectedProductPrice = 0;
		DateTime SelectedProductEntryDate;
		DateTime SelectedProductUpdateDate;
		DateTime SelectedProductDeleteDate;
		bool SelectedProductStatus;
		private void dataGridProducts_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			int selectedIndex = dataGridProducts.SelectedCells[0].RowIndex;
			SelectedProductID = Convert.ToInt32(dataGridProducts.Rows[selectedIndex].Cells["ProductID"].Value);
			SelectedProductNumber = dataGridProducts.Rows[selectedIndex].Cells["ProductNumber"].Value.ToString();
			SelectedProductName = dataGridProducts.Rows[selectedIndex].Cells["ProductName"].Value.ToString();
			SelectedProductQuantity = Convert.ToInt32(dataGridProducts.Rows[selectedIndex].Cells["ProductQuantity"].Value);
			SelectedProductPrice = Convert.ToDecimal(dataGridProducts.Rows[selectedIndex].Cells["ProductPrice"].Value);
			SelectedProductEntryDate = Convert.ToDateTime(dataGridProducts.Rows[selectedIndex].Cells["ProductEntryDate"].Value);
			SelectedProductUpdateDate = Convert.ToDateTime(dataGridProducts.Rows[selectedIndex].Cells["ProductUpdateDate"].Value);
			SelectedProductDeleteDate = Convert.ToDateTime(dataGridProducts.Rows[selectedIndex].Cells["ProductDeleteDate"].Value);
			SelectedProductStatus = Convert.ToBoolean(dataGridProducts.Rows[selectedIndex].Cells["ProductStatus"].Value);
		}
		#endregion

		//Update işleminin gerçekleşeceği popup'ın çağrılması
		private void btnUpdateProducts_Click(object sender, EventArgs e)
		{
			if (SelectedProductID != 0)
			{
				frmUpdateStockPopUp Update = new frmUpdateStockPopUp(SelectedProductID, SelectedProductQuantity, SelectedProductNumber, SelectedProductName, SelectedProductEntryDate, SelectedProductDeleteDate, SelectedProductUpdateDate, SelectedProductPrice, SelectedProductStatus);
				Update.Show();
			}
			else
			{
				MessageBox.Show("Lütfen bir ürün seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		//Silme işleminin çağrılıp gerçekleşmesi
		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (SelectedProductID != 0)
			{
				DeleteSelectedProduct();
			}
			else
			{
				MessageBox.Show("Lütfen bir ürün seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		#region Silme işleminin gerçekleştiği metot
		private void DeleteSelectedProduct()
		{
			using (SqlConnection connection = Database.GetConnection())
			{
				string Query = "UPDATE tbl_Products SET ProductStatus = @NewStatus, ProductDeleteDate = @NewDeleteDate WHERE ProductID = @ID";
				using (SqlCommand cmd = new SqlCommand(Query, connection))
				{
					cmd.Parameters.AddWithValue("@ID", SelectedProductID);
					cmd.Parameters.AddWithValue("@NewStatus", false);
					cmd.Parameters.AddWithValue("@NewDeleteDate", DateTime.Now);

					connection.Open();

					cmd.ExecuteNonQuery();

					MessageBox.Show($"{SelectedProductName} adlı ürün silindi.", "Silme İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}
		#endregion

		private void btnMain_Click(object sender, EventArgs e)
		{
			frmMain MAIN = new frmMain();
			MAIN.Show();
			this.Hide();
		}
	}
}
