using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace productManager
{
	public static class CreateProductNumber
	{
		public static string Create(int EntryDate)
		{
			#region Başlangıç tarihine ek olarak eklenen iki adet random sayı ile Ürün numarası oluşturulması
			Random r = new Random();
			string p1 = EntryDate.ToString();
			string p2 = r.Next(1000, 10000).ToString();
			string p3 = r.Next(1000, 10000).ToString();

			string Result = p1 + p2 + p3;
			#endregion

			#region Oluşturulan ürün numarasının halihazırda kullanılıp kullanılmadığını kontrol eden SQL Sorgusu
			using (SqlConnection connect = Database.GetConnection())
			{
				//Oluşan Result ile herhangi bir ürün numarasının eşleşip eşleşmediğini kontrol edecek SQL Sorgusu
				string Query = "SELECT ProductNumber FROM tbl_Products WHERE ProductNumber = @PNumber";
				using (SqlCommand cmd = new SqlCommand(Query, connect))
				{
					cmd.Parameters.AddWithValue("@PNumber", Result);

					connect.Open();

					using (SqlDataReader dr = cmd.ExecuteReader())
					{
						if (!dr.Read())
						{
							return Result; //Eğer eşleşmiyorsa başarı ile Result string türünde dönecek
						}
						else
						{
							return Create(EntryDate); //Eğer eşleşiyorsa yeni bir numara oluşturulup işlemler tekrar edilecek
						}
					}
				}
			}
			#endregion
		}
	}
}