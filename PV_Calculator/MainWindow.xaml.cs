/* 
 * Model for the radiation calculation		
D.T. Reindl, W.A. Beckman, J.A.Duffie (1990). Diffuse fraction correlations. Solar Energy 45 (1). 1-7. DOI: 10.1016/0038-092X(90)90060-P		
https://www.sciencedirect.com/science/article/abs/pii/0038092X9090060P		
		
Modified from Excel calculation by Prof. Dr. Henrik te Heesen
 
*/





using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using Path = System.IO.Path;

namespace PV_Calculator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
  
   


    public partial class MainWindow : System.Windows.Window
    {
        // public int d = 0;
        public Solar_Data Data;// contains all hourly data of a Project
       
        public double width = 0;
        public double height = 0;

        

        public MainWindow()
        {
            InitializeComponent();
            // Maximize the window
            this.WindowState = WindowState.Maximized;

          
            // Subscribe to the Loaded event to ensure the window state is applied
            this.Loaded += MainWindow_Loaded;

            

            Data = new Solar_Data();
            Data.Name = null;

           
            Data.tilt=0;
            Data.south_angle = 0;
            Data.cooling_factor = 29; // Standard roof top with ventilation
            Data.delta_T = -0.48;
            Data.gps_north= 52.532;
            Data.gps_east = 13.392;
            Data.derating_factor = 0.85;
            Data.peak_power = 1;
            Data.albedo = 0.2;
            Data.calc_ok = false;
            Data.Load_new_NASA_Data = true;


        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Get actual window size
            width = this.ActualWidth;
            height = this.ActualHeight;
        }

        private async void MenueItemOpen_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PV files (*.pvd; )|*.pvd| All files (*.*)|*.*";
            //sfd.InitialDirectory = @"C:\";
            ofd.Title = "Please select an Filname to store the data.";
            ofd.DefaultExt = "nte";
            ofd.ValidateNames = true;

            if (ofd.ShowDialog(this) == true)
            {// MessageBox.Show(ofd.FileName);
                OverlayWindow overlayWindow = new OverlayWindow
                {
                    Owner = this // Set the owner to the main window
                };
                overlayWindow.Show();
                await Task.Run(() => ReadPV_Data(Data, ofd.FileName));
                overlayWindow.Close();

            }

           
        }
      
        private void MenueItemSave_Click(object sender, RoutedEventArgs e)
        {
            Save_file();

        }
        public  void Save_file() 
        {
            if (Data.calc_ok)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PV files (*.pvd)|*.pvd";
                //sfd.InitialDirectory = @"C:\";
                sfd.Title = "Please select an Filname to store the data.";
                sfd.DefaultExt = "pvd";
                sfd.ValidateNames = true;
                if (sfd.ShowDialog(this) == true)
                {
                    //MessageBox.Show(sfd.FileName);
                    OverlayWindow overlayWindow = new OverlayWindow
                    {
                        Owner = this // Set the owner to the main window
                    };
                    overlayWindow.Show();

                    Save_Data(Data, sfd.FileName);
                    overlayWindow.Close();
                }
            }
            else MessageBox.Show(" No data calciulated yet");
        }

        

       
        
        private void MenueItemDailyProduction_Click(object sender, RoutedEventArgs e) // Charge over Time
        {

            if (Data.calc_ok)
            {
                Graph_Window op_Window = new Graph_Window(Data);  // false = Charge over Time
                op_Window.Owner = this; // Set the owner of the subwindow
                op_Window.Top = this.Top + this.ActualHeight - ContentArea.ActualHeight;
                op_Window.Left = this.Left;
                op_Window.Width = this.Width;
                op_Window.Height = ContentArea.ActualHeight;
                if(Data.calc_ok == true) op_Window.Show();
            }
            else MessageBox.Show(" No Project setup or loaded yet");



        }
      

  private void MenueItemMonthlyProduction_Click(object sender, RoutedEventArgs e) // Charge over Time
        {

            if (Data.calc_ok)
            {
                BarWindow PV_Window = new BarWindow(Data);  // false = Charge over Time
                PV_Window.Owner = this; // Set the owner of the subwindow
                PV_Window.Top = this.Top + this.ActualHeight - ContentArea.ActualHeight;
                PV_Window.Left = this.Left;
                PV_Window.Width = this.Width;
                PV_Window.Height = ContentArea.ActualHeight;
                if (Data.calc_ok == true) PV_Window.Show();
            }
            else MessageBox.Show(" No Project setup or loaded yet");





        }


        private void MenueItemOptimizePeakPower_Click(object sender, RoutedEventArgs e)  // optimize PC Peak power
        {
            if (Data.calc_ok)
            {
                
            double nom_Power = Data.peak_power; // store during optimization

            double[] value = new double[20];
            double[] av_cost = new double[20];
            double[] ROI = new double[20];

            double Basecost = (Data.Annual_current_consumption*Data.el_cost+(Data.Annual_Heatpower_consumption+Data.Annual_WhaterHeatpower_consumption)/Data.JAZ*Data.el_cost_heatpump)*1000;

            for (int i = 0; i< 20; i++)
            {
                value[i] =  nom_Power *4/20*i+1;
                Data.peak_power = value[i];
                Project_Calculation p = new Project_Calculation(ref Data);
                    
                    double PV_Bat_cost = Data.pv_cost * Data.peak_power+ Data.battery_capacity * Data.battery_cost;
                double gridconsumption_Heatpump = (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ*1000 - Data.PV_Heatpump_Transfer;
                double gridconsumption_el = Data.grid_consumption - gridconsumption_Heatpump;

                /*double cost_n = PV_Bat_cost;
                cost_n +=  Data.el_cost * gridconsumption_el * 20 ;
                cost_n += Data.el_cost_heatpump * gridconsumption_Heatpump * 20;
                cost_n -= Data.grid_feed_in * Data.feed_in_tarif * 20;
                cost_n /= 20;*/
                av_cost[i] =  Suport.calcLevelized_AnnualCost(Data);

                    double savings = Basecost - Data.el_cost * gridconsumption_el - Data.el_cost_heatpump * gridconsumption_Heatpump + Data.grid_feed_in * Data.feed_in_tarif;


               if (savings > 0)
                              ROI[i] = PV_Bat_cost / savings;
               else 
                        ROI[i] = 20;
            }

           
                Window_optimize PV_Window = new Window_optimize(Data, value, av_cost, ROI,1) ;  // false = Charge over Time
                PV_Window.Owner = this; // Set the owner of the subwindow
                PV_Window.Top = this.Top + this.ActualHeight - ContentArea.ActualHeight;
                PV_Window.Left = this.Left;
                PV_Window.Width = this.Width;
                PV_Window.Height = ContentArea.ActualHeight;
                if(Data.calc_ok == true) PV_Window.Show();
                
                
                Data.peak_power=nom_Power; // and write old value back
                Project_Calculation p1 = new Project_Calculation(ref Data); // restor project data by new calculation
            }
            else MessageBox.Show(" No Project setup or loaded yet");

             



        }



        private void MenueItemOptimizeBattery_Click(object sender, RoutedEventArgs e)   // optimize Battery size
        {
            if (Data.calc_ok)
            {
                double Bat_Cap = Data.battery_capacity; // store during optimization
                double Bat_Pow = Data.Battery_power; // store during optimization

                double[] value = new double[20];
                double[] av_cost = new double[20];
                double[] ROI = new double[20];

                double Basecost = (Data.Annual_current_consumption * Data.el_cost + (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ * Data.el_cost_heatpump) * 1000;

             


                for (int i = 0; i < 20; i++)
                {
                    value[i] = Data.peak_power * 2 / 20 * i+1;
                  //  value[i] = 5;

                    Data.battery_capacity = value[i];
                    Data.Battery_power= value[i]/5;
                    Project_Calculation p = new Project_Calculation(ref Data);

                    double PV_Bat_cost = Data.pv_cost * Data.peak_power + Data.battery_capacity * Data.battery_cost;
                    double gridconsumption_Heatpump = (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ * 1000 - Data.PV_Heatpump_Transfer;
                    double gridconsumption_el = Data.grid_consumption - gridconsumption_Heatpump;

                 /*   double cost_n = PV_Bat_cost;
                    cost_n += Data.el_cost * gridconsumption_el * 20;
                    cost_n += Data.el_cost_heatpump * gridconsumption_Heatpump * 20;
                    cost_n -= Data.grid_feed_in * Data.feed_in_tarif * 20;
                    cost_n /= 20;*/
                    av_cost[i] = Suport.calcLevelized_AnnualCost(Data);

                    double savings = Basecost - Data.el_cost * gridconsumption_el - Data.el_cost_heatpump * gridconsumption_Heatpump + Data.grid_feed_in * Data.feed_in_tarif;


                    if (savings > 0)
                        ROI[i] = PV_Bat_cost / savings;
                    else 
                        ROI[i] = 20;
                }
              

                Window_optimize PV_Window = new Window_optimize(Data, value, av_cost,ROI, 2);  // false = Charge over Time
                PV_Window.Owner = this; // Set the owner of the subwindow
                PV_Window.Top = this.Top + this.ActualHeight - ContentArea.ActualHeight;
                PV_Window.Left = this.Left;
                PV_Window.Width = this.Width;
                PV_Window.Height = ContentArea.ActualHeight;
                if (Data.calc_ok == true) PV_Window.Show();
                
                Data.battery_capacity = Bat_Cap; // and write old value back
                Data.Battery_power=Bat_Pow; // store during optimization

                Project_Calculation p1 = new Project_Calculation(ref Data); // restor project data by new calculation

            }
            else MessageBox.Show(" No Project setup or loaded yet");
        }
        private async void MenueItemOptimizePV_Battery_Click(object sender, RoutedEventArgs e)
        {
            if (Data.calc_ok)
            { 
         
            double[,] ROI = new double[20, 20];
            double PV_size_max = 0;
                double Bat_size_max = 0;
                OverlayWindow overlayWindow = new OverlayWindow
                {
                    Owner = this // Set the owner to the main window
                };
                overlayWindow.Show();

                await Task.Run(() => OptimizePV_Battery(ref ROI, ref PV_size_max, ref Bat_size_max));
                overlayWindow.Close();

                HeatmapWindow PV_Window = new HeatmapWindow(Data, ROI, PV_size_max, Bat_size_max);  // 
            PV_Window.Owner = this; // Set the owner of the subwindow
            PV_Window.Top = this.Top + this.ActualHeight - ContentArea.ActualHeight;
            PV_Window.Left = this.Left;
            PV_Window.Width = this.Width;
            PV_Window.Height = ContentArea.ActualHeight;
                if (Data.calc_ok == true) PV_Window.Show();

            

            Project_Calculation p1 = new Project_Calculation(ref Data); // restor project data by new calculation

        }
            else MessageBox.Show(" No Project setup or loaded yet");


        }
    private void OptimizePV_Battery(ref double[,] ROI, ref double PV_size_max,ref double Bat_size_max)
        {
            
               

                double nom_Power = Data.peak_power; // store during optimization
                double Bat_Cap = Data.battery_capacity; // store during optimization
                double Bat_Pow = Data.Battery_power; // store during optimization
               
                
                double Basecost = (Data.Annual_current_consumption * Data.el_cost + (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ * Data.el_cost_heatpump) * 1000;

                PV_size_max = 3*( Data.Annual_current_consumption + (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ);
                PV_size_max=Suport.round_up(PV_size_max);
                Bat_size_max = PV_size_max;

                for (int k = 0; k < 20; k++) 
                    for (int i = 0; i <20; i++)
                    {
                        Data.peak_power = PV_size_max / 20.0 * (k+0.5);
                        Data.battery_capacity = Bat_size_max/20.0*(i+0.5);
                        Data.Battery_power = Data.battery_capacity / 5;

                        Project_Calculation p = new Project_Calculation(ref Data);

                        //double PV_Bat_cost = Data.pv_cost * Data.peak_power + Data.battery_capacity * Data.battery_cost;
                        //double gridconsumption_Heatpump = (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ * 1000 - Data.PV_Heatpump_Transfer;
                        //double gridconsumption_el = Data.grid_consumption - gridconsumption_Heatpump;
                        //double savings = Basecost - Data.el_cost * gridconsumption_el - Data.el_cost_heatpump * gridconsumption_Heatpump + Data.grid_feed_in * Data.feed_in_tarif;

                        ROI[19 - i, k] = Suport.calcLevelized_AnnualCost(Data);  // battery = yaxis is reversed in Heatplot

                        /*if (savings > 0)
                            ROI[19 - i, k] = PV_Bat_cost / savings;  // battery = yaxis is reversed in Heatplot
                        else
                        ROI[19-i,k] = 20; // battery = yaxis is reversed in Heatplot
                        */
                    
                    }
            //ROI[1, 2] = 200;

            Data.peak_power = nom_Power; // and write old value back
            Data.battery_capacity = Bat_Cap; //
            Data.Battery_power = Bat_Pow; //


        }
        private void MenueItem_ProjectSetup_Click(object sender, RoutedEventArgs e)
        {
            ProjectSetup ps = new ProjectSetup( Data,this);
            ps.Left = 10;
            ps.Top = 10;

            ps.Show();
            
            
        }

        
        //static async Task 
        void Save_Data(Solar_Data Data, string filePath)
        {
            
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write("PV Project Data Vers. 1.0");
                Data.Name = Path.GetFileName(filePath);
                writer.Write(Data.Name);
                writer.Write(Data.count);
                writer.Write(Data.tilt);
                writer.Write(Data.south_angle);
                writer.Write(Data.cooling_factor);
                writer.Write(Data.delta_T);
                writer.Write(Data.gps_north);
                writer.Write(Data.gps_east); 
                writer.Write(Data.derating_factor);
                writer.Write(Data.peak_power);
                writer.Write(Data.albedo);
                writer.Write(Data.calc_ok);  
                Data.date = DateTime.Now.ToString();
                writer.Write(Data.date);
                writer.Write(Data.panel_number);
                writer.Write(Data.panel_power);
                writer.Write(Data.Annual_current_consumption);
                writer.Write(Data.consumption_type);
                writer.Write(Data.el_cost);
                writer.Write(Data.feed_in_tarif);
                writer.Write(Data.pv_cost);
                writer.Write(Data.battery_cost);

                writer.Write(Data.anual_poroduction);
                writer.Write(Data.battery_capacity);  // Ah
                writer.Write(Data.battery_actual_level); //ah
                writer.Write(Data.Battery_power); // pbidirectional converter power in W
                writer.Write(Data.grid_feed_in);
                writer.Write(Data.direct_consumption);
                writer.Write(Data.grid_consumption);
                writer.Write(Data.Annual_current_consumption); // kWh
                writer.Write(Data.Annual_Heatpower_consumption); // kWh
                writer.Write(Data.Annual_WhaterHeatpower_consumption); // kWh
                writer.Write(Data.JAZ); // heat pump system annual average efficiency typical 3.5 for air to water heatpunps
                writer.Write(Data.el_cost_heatpump); // price for Heatpump current; hofully a little cheaper than standart current
                writer.Write(Data.inflation); // price for Heatpump current; hofully a little cheaper than standart current
                writer.Write(Data.AverageTemp);// average annual Temperature of the pv systems location

                /*

                for (int i = 0; i < Data.count; i++)
                {
                    writer.Write(Data.time[i]);
                    writer.Write(Data.yield_adjusted[i]);
                }
                */
                // Flush the writer to ensure all data is written
                fs.FlushAsync();
            }
        }

        //static async Task 
        void ReadPV_Data(Solar_Data Data, string filePath)
        {

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            { 
                
                Data.Name = reader.ReadString(); //filePath;
                if(Data.Name != "PV Project Data Vers. 1.0")
                {
                    MessageBox.Show(" No valid PV Project File");
                    return ;
                }



                Data.Name = reader.ReadString(); //filePath;
                Data.count = reader.ReadInt32();
                Data.tilt = reader.ReadDouble();
                Data.south_angle = reader.ReadDouble();
                Data.cooling_factor = reader.ReadDouble();
                Data.delta_T = reader.ReadDouble();
                Data.gps_north = reader.ReadDouble();
                Data.gps_east = reader.ReadDouble();
                Data.derating_factor = reader.ReadDouble();
                Data.peak_power = reader.ReadDouble();
                Data.albedo = reader.ReadDouble();
                Data.calc_ok = reader.ReadBoolean ();
                Data.date = reader.ReadString();
                Data.panel_number = reader.ReadInt32();  
                Data.panel_power = reader.ReadDouble();
                Data.Annual_current_consumption = reader.ReadDouble();
                Data.consumption_type = reader.ReadInt32();
                Data.el_cost = reader.ReadDouble();
                Data.feed_in_tarif = reader.ReadDouble();
                Data.pv_cost = reader.ReadDouble();
                Data.battery_cost = reader.ReadDouble();

                Data.anual_poroduction = reader.ReadDouble();
                Data.battery_capacity = reader.ReadDouble();  // Ah
                Data.battery_actual_level = reader.ReadDouble(); //ah
                Data.Battery_power = reader.ReadDouble(); // pbidirectional converter power in W
                Data.grid_feed_in = reader.ReadDouble();
                Data.direct_consumption = reader.ReadDouble();
                Data.grid_consumption = reader.ReadDouble();
                Data.Annual_current_consumption = reader.ReadDouble(); // kWh
                Data.Annual_Heatpower_consumption = reader.ReadDouble(); // kWh
                Data.Annual_WhaterHeatpower_consumption = reader.ReadDouble(); // kWh
                Data.JAZ = reader.ReadDouble(); // typical 3.5 for air to water heatpunps
                Data.el_cost_heatpump = reader.ReadDouble(); // price for Heatpump current; hofully a little cheaper than standart current
                Data.inflation = reader.ReadDouble(); // minflation rate in %
                Data.AverageTemp = reader.ReadDouble();// average annual temperature of the location

                /*
                for (int i = 0; i < Data.count; i++)
                {

                    Data.time[i] = reader.ReadDouble();
                    Data.yield_adjusted[i] = reader.ReadDouble();
                }*/

                return;
            }
        }

        // Method to disable the menu
        public void DisableMenu()
        {
            MainMenu.IsEnabled = false;
        }

        // Method to enable the menu
        public void EnableMenu()
        {
            MainMenu.IsEnabled = true;
        }
        public void Update_Data(ref Solar_Data pv)
        {
            Data = pv;
            OverlayWindow overlayWindow = new OverlayWindow
            {
                Owner = this // Set the owner to the main window
            };
           
             overlayWindow.Show();
            Project_Calculation p = new Project_Calculation(ref  Data);
            overlayWindow.Close();

            Data.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //MenueItemDailyProduction_Click(object sender, RoutedEventArgs e)
            MenueItemDailyProduction_Click(MenueItemDailyProduction, new RoutedEventArgs());
        }

        private void MenueItemAbout_Click(object sender, RoutedEventArgs e)
        {
            About About_Window = new About();  // false = Charge over Time
            /*About_Window.Owner = this; // Set the owner of the subwindow
            About_Window.Top = this.Top + this.ActualHeight - ContentArea.ActualHeight;
            About_Window.Left = this.Left;
            About_Window.Width = this.Width;
            About_Window.Height = ContentArea.ActualHeight;*/
            About_Window.Show();

                        
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //MenueItemDailyProduction_Click(MenueItemDailyProduction, new RoutedEventArgs());
            MenueItem_ProjectSetup_Click(MenueItem_ProjectSetup, new RoutedEventArgs());

        }

        private void MenueItemHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help= new HelpWindow();  // false = Charge over Time
            /*About_Window.Owner = this; // Set the owner of the subwindow
            About_Window.Top = this.Top + this.ActualHeight - ContentArea.ActualHeight;
            About_Window.Left = this.Left;
            About_Window.Width = this.Width;
            About_Window.Height = ContentArea.ActualHeight;*/
            help.Show();
        }
    }
}

