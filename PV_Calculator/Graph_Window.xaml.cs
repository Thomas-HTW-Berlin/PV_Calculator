using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators.Financial;
using ScottPlot.WPF;
using SkiaSharp;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PV_Calculator
{
    /// <summary>
    /// Interaktionslogik für Graph.xaml
    /// </summary>
    /// 

    public partial class Graph_Window : Window
    {
        public Graph_Window(Solar_Data Data)
        {
            InitializeComponent();
            // this.WindowState = WindowState.Maximized;
            PVPlot.Plot.YLabel("Hourly Power Production [W]", 12);
            double maxPower = 0;
            double MaxConsumption = 0;
            double[] load=new double[8760];
            for(int i=0;i< Data.count;i++)
            {
                load[i] = Data.Current_consumption[i]*1000000*Data.Annual_current_consumption;
                if (Data.yield_adjusted[i] > maxPower)maxPower = Data.yield_adjusted[i];
                if (load[i] > MaxConsumption)MaxConsumption = load[i];
                Data.time[i] = i;
            }
            // beide axen gleich
            if(maxPower<MaxConsumption) maxPower = MaxConsumption;
            if(maxPower>MaxConsumption) MaxConsumption = maxPower;
            
            maxPower = Suport.round_up(maxPower);
            MaxConsumption = Suport.round_up(MaxConsumption);


            PVPlot.Plot.Axes.SetLimits(0, 9000, 0, maxPower);


            var cha = PVPlot.Plot.Add.Scatter(Data.time, Data.yield_adjusted);
            var vol = PVPlot.Plot.Add.Scatter(Data.time, load );
            vol.Color = ScottPlot.Color.FromHex("FF0000");//    .Gray(0);
            vol.MarkerStyle = MarkerStyle.None;
           
            cha.Axes.YAxis = PVPlot.Plot.Axes.Left;
           // vol.Axes.YAxis = PVPlot.Plot.Axes.Left;
           vol.Axes.YAxis = PVPlot.Plot.Axes.Right;

            
            PVPlot.Plot.Axes.Left.Label.ForeColor = cha.Color;
            PVPlot.Plot.Axes.Right.Label.ForeColor = vol.Color;
            PVPlot.Plot.Axes.Right.Label.Text = " Hourly Current Consumption [W]";
            //PVPlot.Plot.Axes.SetLimits(0, 9000, 0, MaxConsumption, PVPlot.Plot.Axes.Bottom, PVPlot.Plot.Axes.Right) ; // secondary Y axis
            PVPlot.Plot.XLabel(" hours of the year");

            /*
             CoordinateRange yRange = new CoordinateRange(0, 10);

            // define its width using date units
            DateTime start = new DateTime(2024, 01, 01);
            DateTime end = new DateTime(2025, 01, 01);

            CoordinateRange xRange = new CoordinateRange(start.ToOADate(), end.ToOADate());

            // apply width and height to the heatmap
            cha.Plot.Rectangle = new CoordinateRect(xRange, yRange);
            */

            PixelPadding padding = new PixelPadding(100, 100, 100, 50);
            PVPlot.Plot.Layout.Fixed(padding);
            PVPlot.Plot.Title("File: " + Data.Name + "          Anual Production [kWh] :  "+ Data.anual_poroduction.ToString("N0"),24);
            
            double own = Data.direct_consumption / Data.anual_poroduction *100;
            double gridfeed = Data.grid_feed_in / Data.anual_poroduction*100;
            double total_need = (Data.Annual_current_consumption + (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ) * 1000;// change from MWh to kWh
            double grid = Data.grid_consumption / total_need*100;
            double PVtoHeat= Data.PV_Heatpump_Transfer/Data.anual_poroduction*100;
           /*
            * double gridconsumption_Heatpump = (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ * 1000 - Data.PV_Heatpump_Transfer;
            double gridconsumption_el = Data.grid_consumption - gridconsumption_Heatpump;
            double PV_Bat_cost = Data.pv_cost * Data.peak_power + Data.battery_capacity * Data.battery_cost;


            double cost_n = PV_Bat_cost;
            cost_n += Data.el_cost * gridconsumption_el * 20;
            cost_n += Data.el_cost_heatpump * gridconsumption_Heatpump * 20;
            cost_n -= Data.grid_feed_in * Data.feed_in_tarif * 20;
            cost_n /= 20;*/

            double cost_n = Suport.calcLevelized_AnnualCost(Data);
            /*
            var Text = PVPlot.Plot.Add.Text("Direct consumption:", 8000, 0.9 * maxPower);
            Text.LabelFontColor =  ScottPlot.Color.FromHex("0000FF");//    Blue
            Text.LabelFontSize = 16;
            Text.LabelBold = true;*/

            cha.LegendText = "PV Production";
            vol.LegendText = "Current Consumption";
            PVPlot.Plot.ShowLegend();

            //------------- Annotation Box top right ------------------------------------------
            Annotation anno;
            if (Data.consumption_type == 3)
            { // with heatpump
                anno = PVPlot.Plot.Add.Annotation("Direct consumption [%]:   " + own.ToString(".0") +
                                                 " \nGrid feedin [%]:                 " + gridfeed.ToString(".0") +
                                                 " \nGrid consumption [%]:      " + grid.ToString(".0") +
                                                 " \nPV+Battery power transfered to heatpump [%]: " + PVtoHeat.ToString(".0") +
                                                 " \nLevelized annual cost for PV, Battery and Grid consumption [€]: " + cost_n.ToString("N0")
                                                 , Alignment.UpperRight);
            }
            else
            {
                anno = PVPlot.Plot.Add.Annotation("Direct consumption [%]:   " + own.ToString(".0") +
                                                 " \nGrid feedin [%]:                 " + gridfeed.ToString(".0") +
                                                 " \nGrid consumption [%]:      " + grid.ToString(".0") +
                                                 " \nLevelized annual cost for PV, Battery and Grid consumption [€]: " + cost_n.ToString("N0")
                                                 , Alignment.UpperRight);
            }

            anno.LabelFontSize = 16;
            anno.LabelBackgroundColor = ScottPlot.Colors.White.WithAlpha(.3);
            anno.LabelFontColor = ScottPlot.Colors.Blue;
            anno.LabelBorderColor = ScottPlot.Colors.Blue;
          //  anno.LabelBorderWidth = 3;
            anno.LabelShadowColor = ScottPlot.Colors.Transparent;
            anno.OffsetY = 10;
            anno.OffsetX = 10;


            //------------- Annotation Box top left ------------------------------------------
            Annotation anno2;
            switch(Data.consumption_type)
            { case 1: // home no batterey
              default:
                anno2 = PVPlot.Plot.Add.Annotation("Load profile: Private home, no battery or heatpump"+
                                                    " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0")+
                                                    " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 2: // home with battery
                    anno2 = PVPlot.Plot.Add.Annotation("Load profile: Private home with battery" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0")+
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0")+
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") +" / "+ Data.Battery_power.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 3: // home with battery and heatpump
                    anno2 = PVPlot.Plot.Add.Annotation("Load profile: Private home with battery and Heatpump" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0") +
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") + " / " + Data.Battery_power.ToString(".0")+
                                                        " \nHeat [kWh] / water [kW] / efficiency: " + Data.Annual_Heatpower_consumption.ToString(".0") + " / " + Data.Annual_WhaterHeatpower_consumption.ToString(".0") + " / " + Data.JAZ.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 4: // Office
                    anno2 = PVPlot.Plot.Add.Annotation("Load profile: Office with battery" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0") +
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") + " / " + Data.Battery_power.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 5: // Office
                    anno2 = PVPlot.Plot.Add.Annotation("Load profile: Industry 1 shift with battery" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0") +
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") + " / " + Data.Battery_power.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 6: // Office
                    anno2 = PVPlot.Plot.Add.Annotation("Load profile: Industry 2 shift with battery" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0") +
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") + " / " + Data.Battery_power.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;


            }

            anno2.LabelFontSize = 16;
            anno2.LabelBackgroundColor = ScottPlot.Colors.White.WithAlpha(.3);
            anno2.LabelFontColor = ScottPlot.Colors.Black;
            //anno2.LabelBorderColor = ScottPlot.Colors.Blue;
            //anno2.LabelBorderWidth = 3;
            anno2.LabelShadowColor = ScottPlot.Colors.Transparent;
            anno2.OffsetY = 10;
            anno2.OffsetX = 10;



            PVPlot.Refresh();
        }
        
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            return;
        }

        
       

    }
}
