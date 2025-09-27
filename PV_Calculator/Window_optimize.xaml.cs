using ScottPlot;
using ScottPlot.Plottables;
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
using System.Windows.Markup;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PV_Calculator
{
    /// <summary>
    /// Interaktionslogik für Window_optimize.xaml
    /// </summary>
    public partial class Window_optimize : Window
    {
        public Window_optimize(Solar_Data Data, double[] value, double[] av_cost, double[] benefit, int mode)
        {
            InitializeComponent();
            // this.WindowState = WindowState.Maximized;
            OpPlot.Plot.YLabel("Levelized Anual cost [€]", 12);
            double maxCost = 0;
            double maxValue = 0;
            double maxBenefit = 0;
            for (int i = 0; i < 20; i++)
            {
                if (av_cost[i] > maxCost) maxCost = av_cost[i];
                if (value[i] > maxValue) maxValue =value[i];
                if(benefit[i] > maxBenefit) maxBenefit = benefit[i];
                //
                //  Data.time[i] = i;
            }
            maxCost = Suport.round_up(maxCost);
            maxValue = Suport.round_up(maxValue) ;

            // first axis
            var cha = OpPlot.Plot.Add.Scatter(value, av_cost); // first plot
            cha.Axes.YAxis = OpPlot.Plot.Axes.Left;
            OpPlot.Plot.Axes.Left.Label.ForeColor = cha.Color;

            // second axis
            
            var vol = OpPlot.Plot.Add.Scatter(value, benefit); //second plot
            vol.Color = ScottPlot.Color.FromHex("FF0000");//    .Gray(0);
            
            vol.Axes.YAxis = OpPlot.Plot.Axes.Right;

            
            OpPlot.Plot.Axes.Right.Label.ForeColor = vol.Color;
            OpPlot.Plot.Axes.Right.Label.Text = "Payback Period [years]";
        
            cha.LegendText = "Levelized Anual cost [€]";
            vol.LegendText = " Payback Period [years]";
            OpPlot.Plot.Legend.Alignment = Alignment.UpperCenter;



            switch (mode)
            {
                case 1:

                    OpPlot.Plot.XLabel("Nominal Peak Power of PV System [kW]");
                    OpPlot.Plot.Title("File: " + Data.Name + "          PV Peak Power Optimization for battery with capacity [kWh] : "+ Data.battery_capacity.ToString(".0"), 24);
                    


                    break;
                case 2:

                    OpPlot.Plot.XLabel("Battery Capacity in [kWh]");
                    OpPlot.Plot.Title("File: " + Data.Name + "          Battery capacity Optimization for System with Peak Power [kW] :  " + Data.peak_power.ToString(".0"), 24);

                    break;

            }

            


            PixelPadding padding = new PixelPadding(100, 100, 100, 50);
            OpPlot.Plot.Layout.Fixed(padding);
           
            OpPlot.Refresh();
        }
    }
}
