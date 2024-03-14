using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace productManager
{
	public static class Database
	{
		private static string ConnectionString = "Data Source=SERKANKAYA;Initial Catalog=dbo_productManager;Integrated Security=True";

		public static SqlConnection GetConnection() //Otomatik olarak bağlantıyı açacak olan SQL bağlantı metodu
		{
			return new SqlConnection(ConnectionString);
		}
	}
}
