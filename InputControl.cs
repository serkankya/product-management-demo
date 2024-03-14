using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace productManager
{
	public static class InputControl
	{
		#region Çağrıldığı formda, tamamlanan işlem sonrası boş input alanı var mı kontrolü
		public static bool Check(Control.ControlCollection controls)
		{
			foreach (Control item in controls)
			{
				if (item is TextBox || item is NumericUpDown || item is ComboBox)
				{
					if (string.IsNullOrEmpty(item.Text))
					{
						MessageBox.Show("Lütfen boş yerleri doldurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return false;
					}
				}
			}
			return true; //Eğer yok ise true dönecek
		}
		#endregion
	}
}
