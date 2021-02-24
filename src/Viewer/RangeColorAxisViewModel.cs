using System.Windows.Media;

namespace Helix3DPipe.Viewer
{
    public class RangeColorAxisViewModel
    {
        /// <summary> Max range of values to show on legend. </summary>
        public double Max { get; set; }
        /// <summary> Min range of values to show on legend. </summary>
        public double Min { get; set; }
        /// <summary> Step interval between min and max values. </summary>
        public double Step { get; set; }
        /// <summary> Max range of texture coordinates. </summary>
        public double MaxTextureCoordinate { get; set; }
        /// <summary> Min range of texture coordinates. </summary>
        public double MinTextureCoordinate { get; set; }
        /// <summary> Color scheme. </summary>
        public Brush ColorScheme { get; set; }
        /// <summary> Title. </summary>
        public string LegendTitle { get; set; }
        /// <summary> Format string used for axis values. </summary>
        public string AxisValueFormatString { get; set; }
        /// <summary> Flips the color scheme. </summary>
        public bool FlipColorScheme { get; set; }
    }
}