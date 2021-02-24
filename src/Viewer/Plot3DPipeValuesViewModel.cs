using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Helix3DPipe.Viewer
{

	/// <summary>
	/// Extends the WellSurveyPlot3DViewModel to provide methods and properties for 
	/// viewing values along the path of a pipe, internal to the well-bore.
	/// </summary>
	public class Plot3DPipeValuesViewModel : Plot3DPipeViewModel
	{
		private double _maxValueOnPipe;
		private double _minValueOnPipe;

		public Plot3DPipeValuesViewModel()
			: base()
		{
			Pipe = new Tube3DGeometryViewModel();
			PipeLegend = new RangeColorAxisViewModel();
			Well.Fill = new SolidColorBrush(Colors.LightGray) { Opacity = .1 };
			Pipe.Fill = Brushes.DarkGray;
		}

		public Tube3DGeometryViewModel Pipe { get; set; }

		public RangeColorAxisViewModel PipeLegend { get; set; }

		/// <summary>
		/// Creates a pipe that follows the well path, and paints the chosen color gradient as the pipes material.
		/// </summary>
		/// <param name="runDepth"> Run depth in (in). </param>
		/// <param name="valuesAtDepthAlongPipe"> Values are normalized and used as texture coordinates. </param>
		/// <param name="fill"> A linear gradient brush to apply to the pipe. </param>
		public void CreatePipeIn3DWell(double runDepth, Dictionary<double, double> valuesAtDepthAlongPipe, LinearGradientBrush fill)
		{
			if (Well.Path == null || Well.Path.Count < 1) throw new InvalidOperationException("Well must not be null and must have points.");
			Pipe.Path = CalculateGlobalCoordinates();
			fill.StartPoint = new Point(0, 0); //Ensure the fill is horizontal for pipe.
			fill.EndPoint = new Point(1, 0);
			fill.Opacity = 1;
			Pipe.Fill = fill;
			Pipe.TextureCoordinates = NormalizeValues(InterpolatedValues(runDepth, valuesAtDepthAlongPipe));
			Pipe.Diameter = Well.Diameter - 10;
			RaisePropertyChanged("Pipe");
		}

		/// <summary>  Sets up the legend for showing the values along the pipe.
		///  Sets the value step to be 10 equal intervals between the max and min TextureCoordinate values. </summary>
		/// <param name="legendTitle">Title to display above legend. </param>
		/// <param name="legenedBrush">Brush direction for legend should be vertical. </param>
		/// <param name="format"></param>
		public void SetupPipeLegend(string legendTitle, LinearGradientBrush legenedBrush, string format = "")
		{
			if (Pipe.TextureCoordinates == null || Pipe.TextureCoordinates.Count < 1) throw new InvalidOperationException("Pipe texture coordinates must not be null and must have points.");
			PipeLegend.LegendTitle = legendTitle;
			PipeLegend.ColorScheme = legenedBrush;
			PipeLegend.Step = (_maxValueOnPipe - _minValueOnPipe) / 10;
			PipeLegend.Min = _minValueOnPipe;
			PipeLegend.Max = _maxValueOnPipe;
			PipeLegend.MinTextureCoordinate = Pipe.TextureCoordinates.Min();
			PipeLegend.MaxTextureCoordinate = Pipe.TextureCoordinates.Max();
			PipeLegend.AxisValueFormatString = format;
			RaisePropertyChanged("PipeLegend");
		}

		/// <summary>
		/// Interpolates the incoming values at each 3D point along the pipe path.
		/// </summary>
		/// <param name="runDepth"></param>
		/// <param name="valuesAtDepthAlongPipe"></param>
		/// <returns></returns>
		private List<double> InterpolatedValues(double runDepth, Dictionary<double, double> valuesAtDepthAlongPipe)
		{
			var interpolatedValues = new List<double>();
			var keys = new List<double>(valuesAtDepthAlongPipe.Keys);
			var step = runDepth / Pipe.Path.Count - 1;
			for (var i = 0; i < Pipe.Path.Count; i++)
			{
				var depthIndx = Math.Abs(keys.BinarySearch(i * step));
				if (depthIndx >= keys.Count - 1) depthIndx -= 1;
				var nearestDepth = keys[depthIndx];
				var nearestPrvDepth = (i > 0) ? keys[depthIndx - 1] : keys[depthIndx];
				var depth = (nearestDepth + nearestPrvDepth) / 2;
				var val = valuesAtDepthAlongPipe[nearestDepth];
				var preVal = valuesAtDepthAlongPipe[nearestPrvDepth];
				interpolatedValues.Add(Interpolate(preVal, val, nearestPrvDepth, depth, nearestDepth));
			}
			return interpolatedValues;
		}

		/// <summary>
		/// Interpolates or extrapolates to find the unknown X. 
		///           X1,Y1
		/// Solve for X2 Y2
		///           X3,Y3
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="y2">Y-Value for which the function will return X.</param>
		/// <param name="x3"></param>
		/// <param name="y3"></param>
		/// <remarks>http://www.ajdesigner.com/phpinterpolation/linear_interpolation_equation.php</remarks>
		private double Interpolate(double x1, double x3, double y1, double y2, double y3)
		{
			if (Math.Abs(y3 - y1 - 0) < .0001) return x1;
			return x1 + (y2 - y1) * (x3 - x1) / (y3 - y1);
		}

		/// <summary>
		/// Normalize values between 0 & 1 to match scaling of linear gradient color brush.
		/// </summary>
		/// <param name="valuesAtDepthAlongPipe"></param>
		/// <returns></returns>
		private DoubleCollection NormalizeValues(List<double> valuesAtDepthAlongPipe)
		{
			_maxValueOnPipe = valuesAtDepthAlongPipe.Max();
			_minValueOnPipe = valuesAtDepthAlongPipe.Min();
			return new DoubleCollection(valuesAtDepthAlongPipe.Select(d => (d - _minValueOnPipe) / (_maxValueOnPipe - _minValueOnPipe)));
		}

	}

}