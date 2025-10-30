using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Win32;

namespace CV_Material_Import
{
	/// <summary>
	/// Represents a command to open and load a CSV file.
	/// </summary>
	class OpenCSVFileCommand
	{
		public void Execute()
		{
			OpenFileDialog ofd = new()
			{
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = "Text Files|*.csv;*.txt;"
			};
		}
	}
}
