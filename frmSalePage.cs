using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace productManager
{
	public partial class frmSalePage : Form
	{
		public frmSalePage()
		{
			InitializeComponent();
		}

		#region Farklı metotlarda gerekli olacak olan atamalar
		decimal price = 0;
		int selectedProductID = 0;
		decimal selectedProductPrice = 0;
		string productName = null;
		int selectedPersonalID = 0;
		#endregion

		#region (LOAD) Load kısmında ilgili combobox'lara verilerin getirilmesi ve KeyValuePair ile ID değerinin tutulması
		private void frmSalePage_Load(object sender, EventArgs e)
		{
			using (SqlConnection connect = Database.GetConnection())
			{
				connect.Open();
				string ProductQuery = "SELECT * FROM tbl_Products WHERE ProductQuantity > 0";
				string PersonalQuery = "SELECT * FROM tbl_Personals WHERE PersonalStatus = @Status";

				using (SqlCommand cmdProduct = new SqlCommand(ProductQuery, connect))
				{
					using (SqlDataReader drProduct = cmdProduct.ExecuteReader())
					{
						while (drProduct.Read())
						{
							int productID = Convert.ToInt32(drProduct["ProductID"]);
							productName = drProduct["ProductName"].ToString();
							string productQuantity = drProduct["ProductQuantity"].ToString();

							//Key value pair ile combobox'ın seçilen iteminin value'sunda id değerimiz saklanacak
							KeyValuePair<string, int> productItem = new KeyValuePair<string, int>($"{productName} - {productQuantity} Adet", productID);
							cmbProducts.Items.Add(productItem);
						}
					}
				}

				using (SqlCommand cmdPersonal = new SqlCommand(PersonalQuery, connect))
				{
					cmdPersonal.Parameters.AddWithValue("@Status", true);

					using (SqlDataReader drPersonal = cmdPersonal.ExecuteReader())
					{
						while (drPersonal.Read())
						{
							string PersonalName = $"{drPersonal["PersonalName"]} {drPersonal["PersonalSurname"]}";
							int PersonalID = Convert.ToInt32(drPersonal["PersonalID"]);

							//Key value pair ile combobox'ın seçilen iteminin value'sunda id değerimiz saklanacak
							KeyValuePair<string, int> personalInfo = new KeyValuePair<string, int>($"{PersonalName}", PersonalID);
							cmbPersonals.Items.Add(personalInfo);
						}
					}
				}

			}
		}
		#endregion

		#region (PRICECALCULATOR)Adet sayısı girildikten sonra anlık olarak toplam satış fiyatını hesaplayıp, sayfada gösteren metot
		private decimal PriceCalculator(int productID, decimal productQuantity)
		{
			using (SqlConnection connect = Database.GetConnection())
			{
				connect.Open();
				string Query = "SELECT ProductPrice FROM tbl_Products WHERE ProductID = @ProductID";
				using (SqlCommand cmd = new SqlCommand(Query, connect))
				{
					cmd.Parameters.AddWithValue("@ProductID", selectedProductID);

					using (SqlDataReader dr = cmd.ExecuteReader())
					{
						if (dr.Read())
						{
							price = Convert.ToDecimal(dr["ProductPrice"]);
						}
					}
				}
			}

			decimal totalPrice = price * productQuantity;
			lblTotal.Text = totalPrice.ToString();
			return totalPrice;
		}
		#endregion

		#region (CMBPRODUCTSCHANGED)Seçilen ürünün key value pair ile fiyatının veri tabanından çekilmesinin yapıldığı metot
		private void cmbProducts_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Key value pair ile ilgili ürünün ID'si bulunuyor
			KeyValuePair<string, int> selectedProduct = (KeyValuePair<string, int>)cmbProducts.SelectedItem;

			if (!string.IsNullOrEmpty(selectedProduct.Value.ToString()) || !string.IsNullOrEmpty(selectedProduct.Key))
			{
				selectedProductID = selectedProduct.Value;

				using (SqlConnection connect = Database.GetConnection())
				{
					string Query = "SELECT ProductPrice FROM tbl_Products WHERE ProductID = @ProductID";
					connect.Open();
					using (SqlCommand cmd = new SqlCommand(Query, connect))
					{
						cmd.Parameters.AddWithValue("@ProductID", selectedProductID);

						using (SqlDataReader dr = cmd.ExecuteReader())
						{
							if (dr.Read())
							{
								selectedProductPrice = Convert.ToDecimal(dr["ProductPrice"]);
							}
						}
					}
				}
			}
		}
		#endregion

		#region (NUMERICLEAVE)Satışı yapılacak adet sayısının alınması ve PriceCalcular metodunun çağrılması
		private void numericQuantity_Leave(object sender, EventArgs e)
		{
			decimal quantity = numericQuantity.Value;
			PriceCalculator(selectedProductID, quantity);
		}
		#endregion

		#region (BTNSUBMIT)Onayla butonun tıklanmasıyla çalışacak olan metot
		private void btnSubmit_Click(object sender, EventArgs e)
		{
			//Boş yer bırakılmış mı kontrolü
			if (InputControl.Check(this.Controls) && (rbCard.Checked || rbCash.Checked))
			{
				if (rbCard.Checked == true || rbCash.Checked == true)
				{
					int ProductID = selectedProductID;
					int PersonalID = selectedPersonalID;
					int Quantity = Convert.ToInt32(numericQuantity.Value);
					decimal TotalPrice = price;
					string Payment = null;
					string CustomerName = txtCustomer.Text;

					if (rbCard.Checked)
						Payment = "Card";
					else if (rbCash.Checked)
						Payment = "Cash";

					if (ProductID == 0 || PersonalID == 0 || Quantity == 0 || Payment == null || string.IsNullOrEmpty(CustomerName))
					{
						MessageBox.Show("Lütfen bilgileri eksiksiz doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						using (SqlConnection connect = Database.GetConnection())
						{
							//Veri tabanına ilgili verilerin eklenmesi işlemi
							string Query = "INSERT INTO tbl_SalesInfo (SaleProductID,SalePersonalID,SaleQuantity,SaleTotalPrice,SalePayment,SaleCustomer) VALUES (@S_ProductID,@S_PersonalID,@S_Quantity,@S_TotalPrice,@S_Payment,@S_Customer)";
							using (SqlCommand cmd = new SqlCommand(Query, connect))
							{
								cmd.Parameters.AddWithValue("@S_ProductID", ProductID);
								cmd.Parameters.AddWithValue("@S_PersonalID", PersonalID);
								cmd.Parameters.AddWithValue("@S_Quantity", Quantity);
								cmd.Parameters.AddWithValue("@S_TotalPrice", TotalPrice);
								cmd.Parameters.AddWithValue("@S_Payment", Payment);
								cmd.Parameters.AddWithValue("@S_Customer", CustomerName);

								connect.Open();

								DialogResult res = MessageBox.Show($"{productName} adlı ürünün {CustomerName} adlı müşteriye satışını onaylamak istiyor musunuz ?", "Satış İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
								if (res == DialogResult.Yes)
								{
									//Veri tabanındaki ürün miktarının satış adedi kadar düşürülmesi işlemi
									string QuantityReductionQuery = "UPDATE tbl_Products SET ProductQuantity = ProductQuantity - @P_Reduce WHERE ProductID = @P_ID";
									using (SqlCommand cmdReduce = new SqlCommand(QuantityReductionQuery, connect))
									{
										cmdReduce.Parameters.AddWithValue("@P_Reduce", Quantity);
										cmdReduce.Parameters.AddWithValue("@P_ID", selectedProductID);

										cmdReduce.ExecuteNonQuery();
									}

									cmd.ExecuteNonQuery();
									MessageBox.Show($"{productName} ürününün satışı başarı ile gerçekleşti", "Satış Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
								}
								else
								{
									MessageBox.Show($"{productName} ürününün satışı iptal edildi.", "İptal İşlemi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
								}
							}
						}
					}
				}
				else
				{
					MessageBox.Show("Lütfen ödeme türünü seçiniz.", "Ödeme Türü", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}
		#endregion

		#region (CMBPERSONALCHANGED)Seçilen personel id'sinin key value pair ile veri tabanından çekilmesi
		private void cmbPersonals_SelectedIndexChanged(object sender, EventArgs e)
		{
			////Key value pair ile işlemi yapan personelin id'si seçiliyor
			KeyValuePair<string, int> selectedPersonal = (KeyValuePair<string, int>)cmbPersonals.SelectedItem;

			if (!string.IsNullOrEmpty(selectedPersonal.Key) || !string.IsNullOrEmpty(selectedPersonal.Value.ToString()))
			{
				selectedPersonalID = selectedPersonal.Value;
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
