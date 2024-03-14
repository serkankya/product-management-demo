using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace productManager
{
	public static class CreatePersonalNumber
	{
		public static string Create(int EntryYear, int BirthYear)
		{
			#region Başlangıç tarihi ve doğum tarihine ek olarak eklenen random sayı ile Personel numarası oluşturulması
			Random r = new Random();
			string num1 = EntryYear.ToString();
			string num2 = BirthYear.ToString();
			string num3 = r.Next(1000, 10000).ToString();

			string ResultNumber = num1 + num2 + num3;
			#endregion

			#region Oluşturulan personel numarasının halihazırda kullanılıp kullanılmadığını kontrol eden SQL Sorgusu
			using (SqlConnection connection = Database.GetConnection())
			{
				//Oluşan ResultNumber ile herhangi bir personel numarasının eşleşip eşleşmediğini kontrol edecek SQL Sorgusu
				string Query = "SELECT PersonalNumber FROM tbl_Personals WHERE PersonalNumber = @PNumber";
				using (SqlCommand cmd = new SqlCommand(Query, connection))
				{
					cmd.Parameters.AddWithValue("@PNumber", ResultNumber);

					connection.Open();

					using (SqlDataReader dr = cmd.ExecuteReader())
					{
						if (!dr.Read())
						{
							return ResultNumber; //Eğer eşleşmiyorsa başarı ile ResultNumber string türünde dönecek
						}
						else
						{
							return Create(EntryYear, BirthYear); //Eğer eşleşiyorsa yeni bir numara oluşturulup işlemler tekrar edilecek
						}
					}
				}
			}
			#endregion
		}
	}
}
