using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;

namespace Helix3DPipe.Viewer
{
	public class Plot3DPipeViewModel : ViewModelBase
	{
		private int _xDir;
		private int _yDir;

		public Plot3DPipeViewModel()
		{
			MajorGridSpacing = 300; //Meters
			MinorGridSpacing = 150; //Meters
			MarkerLabels = new List<BillboardTextItem>();
			Well = new Tube3DGeometryViewModel();
		}

		public int MajorGridSpacing { get; set; }
		public int MinorGridSpacing { get; set; }
		public int GridLength { get; set; }
		public int GridLineWidth { get; set; }

		public int Xdir { get { return _xDir; } }
		public int Ydir { get { return _yDir; } }

		public Point3D BottomPlaneCenter { get; private set; }
		public Point3D BackLeftPlaneCenter { get; private set; }
		public Point3D BackRightPlaneCenter { get; private set; }
		public List<BillboardTextItem> GridLabels { get; private set; }
		public List<BillboardTextItem> MarkerLabels { get; private set; }
		public PerspectiveCamera PerspectiveCamera { get; private set; }
		public Tube3DGeometryViewModel Well { get; set; }

		public void CreateWell3DPlot()
		{
			Well.Path = CalculateGlobalCoordinates();
			SetGridLength();
			SetSceneSizesAccordingToGridSize();
			SetXyAxisDirections(Well.Path.Last().X, Well.Path.Last().Y);
			CalculateBackPlaneCenterPoints();
			SetupPerspectiveCamera();
			CreateAxisLables();
			RaisePropertyChanged("");
		}

		/// <summary>
		/// Creates a set of 3D points for the given global coordinates.
		/// </summary>
		/// <param name="depth">Depth given in users current units. </param>
		/// <returns></returns>
		protected Point3DCollection CalculateGlobalCoordinates()
		{
			var pointCollection = new Point3DCollection();
			for (var i = 0; i < 100; i++)
			{
				pointCollection.Add(new Point3D
				{
					X = i *2,
					Y = i * 5,
					Z = -50 *i // Note: this example we are set up to go down a hole, so everything is oriented with negative values...
				});
			}
			return pointCollection;
		}

		private void SetGridLength()
		{
			var maxX = Well.Path.Min(x => x.X);
			var maxY = Well.Path.Min(x => x.Y);
			var minZ = Well.Path.Min(x => x.Z);
			var maxAxis = new[] { Math.Abs(maxX), Math.Abs(maxY), Math.Abs(minZ) }.Max();
			var maxAxisToNearestMajorInterval = (int)RoundToNearest(maxAxis, MajorGridSpacing);
			if (maxAxisToNearestMajorInterval % 2 != 0)
				maxAxisToNearestMajorInterval += MajorGridSpacing; //Ensure odd number so we have true half point
			GridLength = maxAxisToNearestMajorInterval;
		}

		/// <summary> Keeps the tube and grid proportionate no matter the zoom.  </summary>
		private void SetSceneSizesAccordingToGridSize()
		{
			const double gridScaleFactor = .001;
			const double tubeScaleFactor = .01;
			Well.Diameter = (int)(GridLength * tubeScaleFactor);
			GridLineWidth = (int)(GridLength * gridScaleFactor);
		}

		private void CalculateBackPlaneCenterPoints()
		{
			// Logic for grid coordinates is odd, not the typical x,y directions
			//  +x-y |   -x-y
			//       |
			// ------ ------
			//  +x+y |   -x+y
			//       |
			//If xDir and yDir and unmodified, Backplanes for 0 to 90
			var centerPoint = GridLength / 2;
			BackLeftPlaneCenter = new Point3D(0, _yDir * centerPoint, -centerPoint);
			BackRightPlaneCenter = new Point3D(_xDir * centerPoint, 0, -centerPoint);
			BottomPlaneCenter = new Point3D(_xDir * centerPoint, _yDir * centerPoint, -GridLength);
		}

		private void SetupPerspectiveCamera()
		{
			PerspectiveCamera = new PerspectiveCamera
			{
				//Position: will be set when ZoomExtentsWhenLoaded is set to True in Xaml.
				//Position = new Point3D(_xDir * GridLength, _yDir * GridLength, .5 * -GridLength),
				LookDirection = new Vector3D(-1 * _xDir * GridLength, -1 * _yDir * GridLength, .5 * -GridLength),
				UpDirection = new Vector3D(0, 0, 1),
				FieldOfView = 20.0,
			};
		}

		private void SetXyAxisDirections(double lastX, double lastY)
		{
			_xDir = -1;
			_yDir = -1;
			if (lastX < 0 & lastY > 0) //90 to 180 
				_yDir = 1;
			if (lastX > 0 & lastY > 0) //180 to 270
			{ _yDir = 1; _xDir = 1; }
			if (lastX > 0 & lastY < 0) //270 to 0
				_xDir = 1;
		}

		private void CreateAxisLables()
		{
			GridLabels = new List<BillboardTextItem>();
			for (var i = 0; i <= GridLength; i += MajorGridSpacing * 4) //Reduce number of labels by stepping
			{
				GridLabels.Add(new BillboardTextItem
				{
					Text = String.Format("{0} {1}", i.ToString("N0"), "ft"),
					Position = new Point3D(_xDir * i, _yDir * GridLength + 100, -GridLength),
					WorldDepthOffset = 100
				});
				GridLabels.Add(new BillboardTextItem
				{
					Text = String.Format("{0} {1}", i.ToString("N0"), "ft"),
					Position = new Point3D(_xDir * GridLength + 100, 0, -i),
					WorldDepthOffset = 100
				});
			}
		}

		protected static double RoundToNearest(double amount, double roundTo)
		{
			double remainder = amount % roundTo;
			if (remainder < (roundTo / 2))
			{
				amount -= remainder;
			}
			else
			{
				amount += (roundTo - remainder);
			}
			return amount;
		}
	}
}
