using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PV_Calculator
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class HeatmapWindow : Window
    {
        public HeatmapWindow(Solar_Data Data, double[,] ROI, double PV_size, double Bat_size )
        {
            InitializeComponent();
           // double[,] data = SampleData.MonaLisa();
            var cha=HeatPlot.Plot.Add.Heatmap(ROI);
            PixelPadding padding = new PixelPadding(100, 150, 100, 50);
            HeatPlot.Plot.Layout.Fixed(padding);
            cha.Axes.YAxis = HeatPlot.Plot.Axes.Left;
            cha.Axes.YAxis.Label.Text="Battery capacity [kWh]";
            double consumption = Data.Annual_current_consumption + (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ;

            HeatPlot.Plot.XLabel("Nominal Peak Power of PV System [kW]");
            HeatPlot.Plot.Title("File: " + Data.Name + "          PV Peak Power and Battery Capacity optimization for a load of [MW]: " + consumption.ToString(".0"), 24);
   
            cha.Colormap = new ScottPlot.Colormaps.Turbo();
            var cb =HeatPlot.Plot.Add.ColorBar(cha);

            cb.Label = "Levelized Annual Cost [€]";
            cb.LabelStyle.FontSize = 24;

            CoordinateRange yRange = new CoordinateRange(0, Bat_size);

            
            CoordinateRange xRange = new CoordinateRange(0,PV_size);

            // apply width and height to the heatmap
            cha.Rectangle = new CoordinateRect(xRange, yRange);
            HeatPlot.Plot.Axes.Margins(0, 0);

            HeatPlot.Refresh();
        }
    }
}
