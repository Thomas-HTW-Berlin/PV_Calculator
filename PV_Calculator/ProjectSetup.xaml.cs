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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Net.Security;

namespace PV_Calculator
{
    /// <summary>
    /// Interaktionslogik für ProjectSetup.xaml
    /// </summary>
    public partial class ProjectSetup : Window
    {
        Solar_Data data;
        MainWindow _mainWindow;
        public String Default_tilt { get; set; } = ""; // 
        public String Default_south_angle { get; set; } = ""; // 
        public String Default_cooling_factor { get; set; } = ""; // 
        public String Default_gps_north { get; set; }= ""; // 
        public String Default_gps_east { get; set; } = ""; // 
        public String Default_panel_number { get; set; } = ""; // 
        public String Default_panel_power { get; set; } = ""; // 
        public String Default_consumption { get; set; } = "3,5"; // 
        public String Default_consumption_type { get; set; } = "1"; // 
        public String Default_cost { get; set; } = "0,35"; // 
        public String Default_feed_in { get; set; } = "0,0796"; // 
        public String Default_pv_cost { get; set; } = "1100"; // 
        public String Default_battery_cost { get; set; } = "300"; // 
        public String Default_derating_factor { get; set; } = "0,85"; //
        public String Default_albedo { get; set; } = "0,2"; //
        public String Default_delta_T { get; set; } = "-0,48"; // 
        public String Default_Name { get; set; } = "Project 1"; // 
        public String Default_battery_capacety { get; set; } = "5"; // 
        public String Default_battery_converter_Power { get; set; } = "1"; // 
        public String Default_HeatpumpConsumption { get; set; } = "10"; // 
        public String Default_HeatpumpJAZ { get; set; } = "3,5"; // 
        public String Default_HeatpumpcostCurrent { get; set; } = "0,25"; // 
        public String Default_HotWhater_Current { get; set; } = "1"; // 
        public String Default_Inflation { get; set; } = "2"; // 
        public String Default_AverageTemp { get; set; } = "10,7"; // 



        public ProjectSetup( Solar_Data dat, MainWindow mainWindow)
        {
            InitializeComponent();
            DataContext = this; // Festlegen des Data Contexts auf dieses Fenster
            _mainWindow = mainWindow;
            mainWindow.DisableMenu();
        

            data =  dat;
            if (data.Name==null && (Properties.Settings.Default.Default_Name != "" || Properties.Settings.Default.Default_Name != null)) // and if default contains some project data 
                {   // Then  use stored project data from last time
                    Properties.Settings.Default.Reload(); // einmalk neu laden damit sie aktiv werden
                    Default_tilt = Properties.Settings.Default.tilt.ToString();
                    Default_south_angle = Properties.Settings.Default.south_angle.ToString();
                    Default_cooling_factor = Properties.Settings.Default.cooling_factor.ToString();
                    Default_gps_north = Properties.Settings.Default.GPS_North.ToString();
                    Default_gps_east = Properties.Settings.Default.GPS_East.ToString();
                    Default_panel_number = Properties.Settings.Default.panel_number.ToString();
                    Default_panel_power = Properties.Settings.Default.panel_power.ToString();
                    Default_consumption = Properties.Settings.Default.consumption.ToString();
                    //Default_consumption_type = Properties.Settings.Default.consumption_type.ToString();
                    data.consumption_type = Properties.Settings.Default.consumption_type;
                    data.cooling_factor = Properties.Settings.Default.cooling_factor;
                    Default_cost = Properties.Settings.Default.el_cost.ToString();
                    Default_pv_cost = Properties.Settings.Default.pv_cost.ToString();
                    Default_feed_in = Properties.Settings.Default.feed_in.ToString();
                    Default_battery_cost = Properties.Settings.Default.battery_cost.ToString();
                    Default_delta_T = Properties.Settings.Default.delta_T.ToString();
                    Default_derating_factor = Properties.Settings.Default.derating_factor.ToString();
                    Default_albedo = Properties.Settings.Default.albedo.ToString();
                    Default_Name = Properties.Settings.Default.Default_Name;
                    Default_battery_capacety = Properties.Settings.Default.battery_capacety.ToString();  // 
                    Default_battery_converter_Power= Properties.Settings.Default.battery_converter_Power.ToString();  // 
                    Default_HeatpumpConsumption= Properties.Settings.Default.HeatpumpConsumption.ToString(); ; // 
                    Default_HeatpumpJAZ = Properties.Settings.Default.HeatpumpJAZ.ToString(); // 
                    Default_HeatpumpcostCurrent  = Properties.Settings.Default.Heatpumpcost.ToString(); // 
                    Default_HotWhater_Current = Properties.Settings.Default.HotWhater_Current.ToString(); // 
                    Default_Inflation= Properties.Settings.Default.Inflation.ToString();
                    Default_AverageTemp= Properties.Settings.Default.AverageTemp.ToString();

    }
                else
                {
                    Default_tilt = data.tilt.ToString();
                    Default_south_angle = data.south_angle.ToString();
                    Default_cooling_factor = data.cooling_factor.ToString();
                    Default_gps_north = data.gps_north.ToString();
                    Default_gps_east = data.gps_east.ToString();
                    Default_panel_number = data.panel_number.ToString();
                    Default_panel_power = data.panel_power.ToString();
                    Default_consumption = data.Annual_current_consumption.ToString();
                    Default_cost = data.el_cost.ToString();
                    Default_pv_cost = data.pv_cost.ToString();
                    Default_feed_in = data.feed_in_tarif.ToString();
                    Default_battery_cost = data.battery_cost.ToString();
                    Default_delta_T = data.delta_T.ToString();
                    Default_derating_factor = data.derating_factor.ToString();
                    Default_albedo = data.albedo.ToString();
                    Default_Name = data.Name;
                    Default_battery_capacety =data.battery_capacity.ToString(); // 
                    Default_battery_converter_Power =data.Battery_power.ToString(); // 
                    Default_HeatpumpConsumption = data.Annual_Heatpower_consumption.ToString(); // 
                    Default_HeatpumpJAZ =data.JAZ.ToString(); // 
                    Default_HeatpumpcostCurrent = data.el_cost_heatpump.ToString(); // 
                    Default_HotWhater_Current = data.Annual_WhaterHeatpower_consumption.ToString(); // 
                    Default_Inflation =data.inflation.ToString();
                    Default_AverageTemp = data.AverageTemp.ToString();


            }
            switch (data.cooling_factor)
                {
                    case 29: B1.IsChecked = true; break;
                    case 32: B2.IsChecked = true; break;
                    case 43: B3.IsChecked = true; break;
                    case 28: B4.IsChecked = true; break;
                    case 22: B5.IsChecked = true; break;
                    default: break;
                }
                switch (data.consumption_type)
                {
                    case 1: C1.IsChecked = true; break;
                    case 2: C2.IsChecked = true; break;
                    case 3: C3.IsChecked = true; break;
                    case 4: C4.IsChecked = true; break;
                    case 5: C5.IsChecked = true; break;
                    case 6: C6.IsChecked = true; break;

                default: break;
                }

            switch (data.consumption_type)
            {
                case 0:
                case 1: //"private home":
                case 2: /// home with battery
                case 3: //   private home with PV-battery and heat pump":
                case 4: // "office 8 a.m. to 5 p.m. with PV-battery":
                case 5: // "Industry 1 shifts with PV-battery":
                case 6: // "Industry 2 shifts with PV-battery":
                    Properties.Settings.Default.Reload(); // einmalk neu laden damit sie aktiv werden
                    Default_battery_capacety = Properties.Settings.Default.battery_capacety.ToString();  // 
                    Default_battery_converter_Power = Properties.Settings.Default.battery_converter_Power.ToString();  // 
                    Default_HeatpumpConsumption = Properties.Settings.Default.HeatpumpConsumption.ToString(); ; // 
                    Default_HeatpumpJAZ = Properties.Settings.Default.HeatpumpJAZ.ToString(); // 
                    Default_HeatpumpcostCurrent = Properties.Settings.Default.Heatpumpcost.ToString(); // 
                    Default_HotWhater_Current = Properties.Settings.Default.HotWhater_Current.ToString(); // 
                    break;
            }


        }




        private void SaveButton_Checked_1(object sender, RoutedEventArgs e)
        {
            data.tilt=read_double(DoubleTextTilt.Text);
            data.gps_east = read_double(DoubleTextGpsEast.Text);
            data.gps_north = read_double(DoubleTextGpsNorth.Text);
            data.panel_number = read_double(DoubleTextNumber.Text);
            data.panel_power = read_double(DoubleTextPower.Text);
            data.peak_power = data.panel_number * data.panel_power/1000;
            data.tilt = read_double(DoubleTextTilt.Text);
            data.south_angle = read_double(DoubleTextSouth.Text);
            data.derating_factor = read_double(DoubleTextDerating.Text);
            data.delta_T = read_double(DoubleTextCoolingCoeff.Text);
            data.albedo = read_double(DoubleTextAlbedo.Text);
            data.Annual_current_consumption = read_double(DoubleTextConsumption.Text);
            data.el_cost= read_double(DoubleTextElCost.Text);
            data.pv_cost = read_double(DoubleTextPVcost.Text);
            data.feed_in_tarif = read_double(DoubleTextFeedIn.Text);
            data.inflation=read_double(DoubleTextInflation.Text);
            data.battery_cost = read_double(DoubleTextBatteryCost.Text);
            data.Name = DoubleTextName.Text;

            data.battery_capacity= read_double(DoubleTextBatteryCapacity.Text); // 
            data.Battery_power = read_double(DoubleTextBatteryConverterPower.Text); // 
            data.Annual_Heatpower_consumption= read_double(DoubleTextHeatpumpConsumption.Text); // 
            data.JAZ= read_double(DoubleTextHeatpumpJAZ.Text); // 
            data.el_cost_heatpump=read_double(DoubleTextCost_HeatpumpCurrent.Text); // 
            data.Annual_WhaterHeatpower_consumption= read_double(DoubleTextCost_HotWaterCurrent.Text); // 
            data.AverageTemp = read_double(DoubleTextAverageTemp.Text);



            Properties.Settings.Default.tilt = data.tilt;
            Properties.Settings.Default.south_angle = data.south_angle;
            Properties.Settings.Default.GPS_North = data.gps_north;
            Properties.Settings.Default.GPS_East = data.gps_east;
            Properties.Settings.Default.panel_number = data.panel_number ;
            Properties.Settings.Default.panel_power = data.panel_power;
            Properties.Settings.Default.consumption = data.Annual_current_consumption;
            Properties.Settings.Default.consumption_type = data.consumption_type;
            Properties.Settings.Default.cooling_factor= data.cooling_factor;
            Properties.Settings.Default.el_cost = data.el_cost;
            Properties.Settings.Default.feed_in = data.feed_in_tarif;
            Properties.Settings.Default.pv_cost = data.pv_cost;
            Properties.Settings.Default.battery_cost = data.battery_cost;
            Properties.Settings.Default.delta_T = data.delta_T ;
            Properties.Settings.Default.derating_factor = data.derating_factor ;
            Properties.Settings.Default.albedo = data.albedo ;
            Properties.Settings.Default.Default_Name= Default_Name ;

            Properties.Settings.Default.battery_capacety = data.battery_capacity;
            Properties.Settings.Default.battery_converter_Power = data.Battery_power;
            Properties.Settings.Default.HeatpumpConsumption = data.Annual_Heatpower_consumption;
            Properties.Settings.Default.HeatpumpJAZ = data.JAZ;
            Properties.Settings.Default.Heatpumpcost = data.el_cost_heatpump;
            Properties.Settings.Default.HotWhater_Current = data.Annual_WhaterHeatpower_consumption;
            Properties.Settings.Default.Inflation = data.inflation;
            Properties.Settings.Default.AverageTemp = data.AverageTemp;




           Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload(); // einmalk neu laden damit sie aktiv werden
            _mainWindow.EnableMenu();

            _mainWindow.Update_Data(ref data);

            Close();

        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                string Text = $"{radioButton.Content}";

                switch (Text)
                {
                    case "29 roof-parallel, well ventilated":
                        data.cooling_factor = 29;
                        break;
                    case "32 roof-integrated - back-ventilated":
                        data.cooling_factor =32;
                        break;

                    case "43 roof-integrated - not back-ventilated":
                        data.cooling_factor =43 ;
                        break;
                    case "28 elevated – roof-top":
                        data.cooling_factor =28 ;
                        break;
                    case "22 elevated – ground-mounted":
                        data.cooling_factor =22 ;
                        break;
                }
            }
        }

        private void LoadButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton loadButton = sender as RadioButton;
            if( loadButton != null)
            {
                string Text = $"{loadButton.Content}";

                switch (Text)
                {
                    case "private home":
                        data.consumption_type = 1;
                        DoubleTextBatteryCost.IsEnabled=false;
                        DoubleTextBatteryCapacity.IsEnabled = false;
                        DoubleTextBatteryConverterPower.IsEnabled=false;
                        DoubleTextHeatpumpConsumption.IsEnabled=false;
                        DoubleTextCost_HotWaterCurrent.IsEnabled=false;
                        DoubleTextCost_HeatpumpCurrent.IsEnabled = false;
                        DoubleTextHeatpumpJAZ.IsEnabled=false;


                        break;
                    case "private home with PV-battery":
                        data.consumption_type = 2;
                        DoubleTextBatteryCost.IsEnabled = true;
                        DoubleTextBatteryCapacity.IsEnabled = true;
                        DoubleTextBatteryConverterPower.IsEnabled = true;
                        DoubleTextHeatpumpConsumption.IsEnabled = false;
                        DoubleTextCost_HotWaterCurrent.IsEnabled = false;
                        DoubleTextCost_HeatpumpCurrent.IsEnabled = false;
                        DoubleTextHeatpumpJAZ.IsEnabled = false;
                        break;
                    case "private home with PV-battery and heat pump":
                        data.consumption_type = 3;
                        DoubleTextBatteryCost.IsEnabled = true;
                        DoubleTextBatteryCapacity.IsEnabled = true;
                        DoubleTextBatteryConverterPower.IsEnabled = true;
                        DoubleTextHeatpumpConsumption.IsEnabled = true;
                        DoubleTextCost_HotWaterCurrent.IsEnabled = true;
                        DoubleTextCost_HeatpumpCurrent.IsEnabled = true;
                        DoubleTextHeatpumpJAZ.IsEnabled = true;
                        break;
                    case "office 8 a.m. to 5 p.m. with PV-battery":
                        data.consumption_type = 4;
                        DoubleTextBatteryCost.IsEnabled = true;
                        DoubleTextBatteryCapacity.IsEnabled = true;
                        DoubleTextBatteryConverterPower.IsEnabled = true;
                        DoubleTextHeatpumpConsumption.IsEnabled = false;
                        DoubleTextCost_HotWaterCurrent.IsEnabled = false;
                        DoubleTextCost_HeatpumpCurrent.IsEnabled = false;
                        DoubleTextHeatpumpJAZ.IsEnabled = false;

                        break;
                    case "Industry 1 shift with PV-battery":
                        data.consumption_type = 5;
                        DoubleTextBatteryCost.IsEnabled = true;
                        DoubleTextBatteryCapacity.IsEnabled = true;
                        DoubleTextBatteryConverterPower.IsEnabled = true;
                        DoubleTextHeatpumpConsumption.IsEnabled = false;
                        DoubleTextCost_HotWaterCurrent.IsEnabled = false;
                        DoubleTextCost_HeatpumpCurrent.IsEnabled = false;
                        DoubleTextHeatpumpJAZ.IsEnabled = false;

                        break;
                    case "Industry 2 shifts with PV-battery":
                        data.consumption_type = 6;
                        DoubleTextBatteryCost.IsEnabled = true;
                        DoubleTextBatteryCapacity.IsEnabled = true;
                        DoubleTextBatteryConverterPower.IsEnabled = true;
                        DoubleTextHeatpumpConsumption.IsEnabled = false;
                        DoubleTextCost_HotWaterCurrent.IsEnabled = false;
                        DoubleTextCost_HeatpumpCurrent.IsEnabled = false;
                        DoubleTextHeatpumpJAZ.IsEnabled = false;
                        break;
                }
              
            }
        }
     
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.EnableMenu();
            Close();
        }


        private double read_double(string text)
        {
            double d = -9E127;

            if (double.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("de-DE"), out double result))
            {
                d = result;
            }
            else
            {
                MessageBox.Show("Bitte geben Sie eine gültige Double-Zahl ein.", "Ungültige Eingabe", MessageBoxButton.OK, MessageBoxImage.Warning);
                d = 0;
            }

            if (double.TryParse(text, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out double result_us))
            {
                if (d > 0) {
                    if (d > result_us) d = result_us;
                }else 
                    if(d<result_us) d = result_us;

            }
            return d;
        }
    
    

    }
}
