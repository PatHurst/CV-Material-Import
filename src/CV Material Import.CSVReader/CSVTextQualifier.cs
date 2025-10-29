using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Material_Import.CSVReader;

/// <summary>
/// Options for character surrounding text.
/// </summary>
public enum CSVTextQualifier
{
	None = 0,
	DoubleQuotation = 34,
	SingleQuotation = 39
}
