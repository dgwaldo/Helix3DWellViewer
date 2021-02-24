using GalaSoft.MvvmLight;
using Helix3DPipe.Viewer;
using HelixToolkit.Wpf;
using System.Collections.Generic;

namespace Helix3DPipe.ViewModel
{
	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// <para>
	/// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
	/// </para>
	/// <para>
	/// You can also use Blend to data bind with the tool's support.
	/// </para>
	/// <para>
	/// See http://www.galasoft.ch/mvvm
	/// </para>
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			////if (IsInDesignMode)
			////{
			////    // Code runs in Blend --> create design time data.
			////}
			////else
			////{
			////    // Code runs "for real"
			////}

			Pipe3DVm = new Plot3DPipeValuesViewModel();
			SetupPlot();
		}

		public Plot3DPipeValuesViewModel Pipe3DVm { get; set; }

		public void SetupPlot()
		{
			// Note: Need same number of points as are in pipe path...
			// Check the Plot3DPipeViewModel.CalculateGlobalCoordinates() method.
			var valuesAtLengthAlongPipe = new Dictionary<double, double>();
			for (var i = 0; i < 100; i++)
			{
				valuesAtLengthAlongPipe.Add(500 * i, .001 * i);
			}

			var fill = HelixToolkit.Wpf.BrushHelper.CreateRainbowBrush();
			Pipe3DVm.CreateWell3DPlot();
			Pipe3DVm.CreatePipeIn3DWell(5000, valuesAtLengthAlongPipe, fill);

		}

	}
}