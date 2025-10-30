using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV_Material_Import;

/// <summary>
/// Represents a CV Material.
/// </summary>
public class Material
{
	public string? Name { get; set; }
	public string? Description { get; set; }
	//public string? SKU { get; set; }
	public decimal DefaultCost { get; set; }
	public decimal SellPrice { get; set; }
	public double Width { get; set; }
	public double Length { get; set; }
	public double Thickness { get; set; }

	public override string ToString()
	{
		StringBuilder sb = new();
		sb.AppendLine($"{nameof(Name)}: {Name}");
		sb.AppendLine($"{nameof(Description)}: {Description}");
		//sb.AppendLine($"{nameof(SKU)}: {SKU}");
		sb.AppendLine($"{nameof(DefaultCost)}: {DefaultCost:C}");
		sb.AppendLine($"{nameof(SellPrice)}: {SellPrice:C}");
		sb.AppendLine($"{nameof(Width)}: {Width} mm");
		sb.AppendLine($"{nameof(Length)}: {Length} mm");
		sb.AppendLine($"{nameof(Thickness)}: {Thickness} mm");
		return sb.ToString();
	}

}

