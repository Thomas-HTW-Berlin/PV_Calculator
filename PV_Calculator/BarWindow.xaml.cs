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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PV_Calculator
{
    /// <summary>
    /// Interaktionslogik für BarWindow.xaml
    /// </summary>
    public partial class BarWindow : Window
    {
        public BarWindow(Solar_Data Data)
        {
            InitializeComponent();
            // this.WindowState = WindowState.Maximized;
            PVBarPlot.Plot.YLabel("Monthly Power Production [kW]", 12);
            double maxPower = 0;

            double[] month=new double[12];
            for (int i = 0; i < 12; i++)
            {
                if (Data.monthly_power[i] > maxPower) maxPower = Data.monthly_power[i];
                if (Data.monthly_heat[i] + Data.monthly_water[i] + Data.monthly_current[i] > maxPower) maxPower = Data.monthly_heat[i] + Data.monthly_water[i] + Data.monthly_current[i];
                month[i] = i + 1;
            }
            maxPower = Suport.round_up(maxPower);


            //double[] values = new double[35];// { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            Tick[] months = {
                 new Tick(1.5, "Jan"),
                 new Tick(5.5, "Feb"),
                 new Tick(9.5, "Mar"),
                 new Tick(13.5, "Apr"),
                 new  Tick(17.5, "May"),
                 new  Tick(21.5, "Jun"),
                new  Tick(25.5, "Jul"),
                 new Tick(29.5, "Aug"),
                new Tick(33.5, "Sep"),
                 new Tick(37.5, "Oct"),
                new Tick(41.5, "Nov"),
                new Tick(45.5, "Dez"),
            };
            ScottPlot.Palettes.Category10 palette = new ScottPlot.Palettes.Category10();

            ScottPlot.Bar[] bars = new ScottPlot.Bar[48];

            

            for (int i = 0; i < 48; i+=4)
            {   double a = Data.monthly_current[i / 4] * 1000;
                double b = Data.monthly_heat[i / 4] * 1000;
                double c = Data.monthly_water[i / 4] * 1000;
                bars[i]   = new ScottPlot.Bar() { Position = i+1, ValueBase = 0, Value = a, FillColor = palette.GetColor(0) };
                bars[i+1] = new ScottPlot.Bar() { Position = i+1, ValueBase = a, Value =a+b, FillColor = palette.GetColor(1) };
                bars[i+2] = new ScottPlot.Bar() { Position = i+1, ValueBase = a+b, Value = a+b+c, FillColor = palette.GetColor(2) };
                bars[i+3] = new ScottPlot.Bar() { Position = i+2, ValueBase = 0, Value = Data.monthly_power[i/4], FillColor = palette.GetColor(3) };
             }

    // sec
            
            
            PVBarPlot.Plot.Add.Bars(bars);

            PVBarPlot.Plot.Axes.Margins(bottom: 0);
            PVBarPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(months);
               PixelPadding padding = new PixelPadding(100, 100, 100, 50);
            PVBarPlot.Plot.Layout.Fixed(padding);
            PVBarPlot.Plot.Title("File: " + Data.Name + "  Anual Production [kWh] :  " + Data.anual_poroduction.ToString("N0"),24);

            double own = Data.direct_consumption / Data.anual_poroduction * 100;
            double gridfeed = Data.grid_feed_in / Data.anual_poroduction * 100;
            double total_need = (Data.Annual_current_consumption + (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ) * 1000;// change from MWh to kWh
            double grid = Data.grid_consumption / total_need * 100;
            double PVtoHeat = Data.PV_Heatpump_Transfer / Data.anual_poroduction * 100;

            /*double gridconsumption_Heatpump = (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ * 1000 - Data.PV_Heatpump_Transfer;
            double gridconsumption_el = Data.grid_consumption - gridconsumption_Heatpump;
            double PV_Bat_cost = Data.pv_cost * Data.peak_power + Data.battery_capacity * Data.battery_cost;


            double cost_n = PV_Bat_cost;
            cost_n += Data.el_cost * gridconsumption_el * 20;
            cost_n += Data.el_cost_heatpump * gridconsumption_Heatpump * 20;
            cost_n -= Data.grid_feed_in * Data.feed_in_tarif * 20;
            cost_n /= 20;*/
            double cost_n = Suport.calcLevelized_AnnualCost(Data);

            // build the legend manually
            PVBarPlot.Plot.Legend.IsVisible = true;
       //     PVBarPlot.Plot.Legend.Alignment = Alignment.LowerLeft;
            PVBarPlot.Plot.Legend.Alignment = Alignment.LowerCenter;
            //PVBarPlot.Plot.Legend.BackgroundColor = ScottPlot.Colors.White.WithAlpha(.3);

            PVBarPlot.Plot.Legend.ManualItems.Add(new LegendItem() { LabelText = "Current consuption [kW]", FillColor = palette.GetColor(0) });
            PVBarPlot.Plot.Legend.ManualItems.Add(new LegendItem() { LabelText = "Heat pump current consuption [kW]", FillColor = palette.GetColor(1) });
            PVBarPlot.Plot.Legend.ManualItems.Add(new LegendItem() { LabelText = "Hot water current consuption [kW]", FillColor = palette.GetColor(2) });
            PVBarPlot.Plot.Legend.ManualItems.Add(new LegendItem() { LabelText = "PV power generation [kW]", FillColor = palette.GetColor(3) });


            //------------- Annotation Box top right ------------------------------------------
            Annotation anno;
            if (Data.consumption_type == 3)
            { // with heatpump
                anno = PVBarPlot.Plot.Add.Annotation("Direct consumption [%]:   " + own.ToString(".0") +
                                                 " \nGrid feedin [%]:                 " + gridfeed.ToString(".0") +
                                                 " \nGrid consumption [%]:      " + grid.ToString(".0") +
                                                 " \nPV+Battery power transfered to heatpump [%]: " + PVtoHeat.ToString(".0") +
                                                 " \nLevelized annual cost for PV, Battery and Grid consumption [€]: " + cost_n.ToString("N0")
                                                 , Alignment.UpperRight);
            }else
            {
                anno = PVBarPlot.Plot.Add.Annotation("Direct consumption [%]:   " + own.ToString(".0") +
                                                 " \nGrid feedin [%]:                 " + gridfeed.ToString(".0") +
                                                 " \nGrid consumption [%]:      " + grid.ToString(".0") +
                                                 " \nLevelized annual cost for PV, Battery and Grid consumption [€]: " + cost_n.ToString("N0")
                                                 , Alignment.UpperRight);
            }

            anno.LabelFontSize = 16;
            anno.LabelBackgroundColor = ScottPlot.Colors.White.WithAlpha(.3);
            anno.LabelFontColor = ScottPlot.Colors.Blue;
            anno.LabelBorderColor = ScottPlot.Colors.Blue;
           // anno.LabelBorderWidth = 3;
            anno.LabelShadowColor = ScottPlot.Colors.Transparent;
            anno.OffsetY = 10;
            anno.OffsetX = 10;

            //------------- Annotation Box top left ------------------------------------------
            Annotation anno2;
            switch (Data.consumption_type)
            {
                case 1: // home no batterey
                default:
                    anno2 = PVBarPlot.Plot.Add.Annotation("Load profile: Private home, no battery or heatpump" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0")
                                                     , Alignment.UpperLeft);
                    break;
                case 2: // home with battery
                    anno2 = PVBarPlot.Plot.Add.Annotation("Load profile: Private home with battery" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0") +
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") + " / " + Data.Battery_power.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 3: // home with battery and heatpump
                    anno2 = PVBarPlot.Plot.Add.Annotation("Load profile: Private home with battery and Heatpump" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0") +
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") + " / " + Data.Battery_power.ToString(".0") +
                                                        " \nHeat [kWh] / water [kW] / efficiency: " + Data.Annual_Heatpower_consumption.ToString(".0") + " / " + Data.Annual_WhaterHeatpower_consumption.ToString(".0") + " / " + Data.JAZ.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 4: // Office
                    anno2 = PVBarPlot.Plot.Add.Annotation("Load profile: Office with battery" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0") +
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") + " / " + Data.Battery_power.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 5: // Office
                    anno2 = PVBarPlot.Plot.Add.Annotation("Load profile: Industry 1 shift with battery" +
                                                        " \nAnnual power consumption [MWh]: " + Data.Annual_current_consumption.ToString(".0") +
                                                        " \nPV peak power [kW]:            " + Data.peak_power.ToString(".0") +
                                                        " \nBattery capacity [kWh] / power [kW]: " + Data.battery_capacity.ToString(".0") + " / " + Data.Battery_power.ToString(".0")
                                                 , Alignment.UpperLeft);
                    break;
                case 6: // Office
                    anno2 = PVBarPlot.Plot.Add.Annotation("Load profile: Industry 2 shift with battery" +
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

            PVBarPlot.Refresh();

        }
    }
}
