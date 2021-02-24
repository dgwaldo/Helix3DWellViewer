using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Helix3DPipe.Viewer
{
	public class Tube3DGeometryViewModel
	{
		/// <summary> Tube diameter in coordinates relative to XYZ path. </summary>
		public double Diameter { get; set; }

		/// <summary> Number of sections around the circle. </summary>
		public double ThetaDiv { get; set; }

		/// <summary> XYZ Point collection. </summary>
		public Point3DCollection Path { get; set; }

		/// <summary> Brush used for fill, if set will also be used for the material. </summary>
		public Brush Fill { get; set; }

		/// <summary> Texture coordinates, define the value to show from a color gradient, at each XYZ point, normalized between 0 and 1. </summary>
		public DoubleCollection TextureCoordinates { get; set; }

		/// <summary> Material, uses the fill brush to create a material, can be used in place of fill. </summary>
		public Material Material
		{
			get { return MaterialHelper.CreateMaterial(Fill); }
		}
	}
}
