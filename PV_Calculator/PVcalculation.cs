using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.ES20;
using ScottPlot;
using ScottPlot.TickGenerators.TimeUnits;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Shapes;
using static OpenTK.Graphics.OpenGL.GL;

namespace PV_Calculator
{
    internal class Project_Calculation
    {

        public Project_Calculation(ref Solar_Data d)
        {

           

            if (d.time == null)
            {
                d.time = new double[8760];
                d.day = new double[8760];
                d.daily_power = new double[365];
                d.monthly_power = new double[12];
                d.hour_angle = new double[8760];
                d.count = 8760;
                d.declination = new double[8760];
                d.TEQ_min = new double[8760];
                d.MLT_adj = new double[8760];
                d.TLT_adj = new double[8760];
                d.sun_elevation_angle = new double[8760];
                d.adjusted_solarconstant = new double[8760];
                d.I = new double[8760];
                d.K = new double[8760];
                d.BHI = new double[8760];
                d.BNI = new double[8760];
                d.DHI = new double[8760];
                d.sun_azimuth = new double[8760];
                d.angle_of_incidence = new double[8760];

                d.r_hor_total = new double[8760];
                d.temperature = new double[8760];
                d.r_tilted_total = new double[8760];
                d.GHI = new double[8760];  // Achtung muss aus CSV geladen werden
                d.Beam_horizontal_irradiation = new double[8760]; // BHI to be loaded from File
                d.Diffuse_horizontal_irradiation = new double[8760]; // DHI to be loaded from File
                d.Ambient_Temperature = new double[8760]; //  to be loaded from File  addd 2° for Berlin
                d.t_modul = new double[8760];
                d.yield_adjusted = new double[8760];
                d.Current_consumption = new double[8760];
                d.Heat_consumption = new double[8760];
                d.Waterheat_consumption = new double[8760];
                d.Loadprofile_home = new double[8760];
                d.Loadprofile_office = new double[8760];
                d.Loadprofile_1shift = new double[8760];
                d.Loadprofile_2shift = new double[8760];

                
            }
            for (int i = 0; i < 365; i++)
            {
                d.daily_power[i] = 0;
            }

            for (int i = 0; i < 12; i++)
            {
                d.monthly_power[i] = 0;
            }
            d.anual_poroduction = 0;
            readStandardRadiation(ref d, "Data\\help.csv"); // loads some yearly standardata from file
            switch (d.consumption_type)
            {
                case 0: 
                case 1: //"private home":

                    d.Annual_Heatpower_consumption= 0;
                    d.JAZ = 1;
                    d.Annual_WhaterHeatpower_consumption = 0;
                    d.battery_capacity = 0;
                    d.Battery_power = 0;
                    for(int i = 0;i <8760;i++) d.Current_consumption[i] = d.Loadprofile_home[i];
                    break;
                case 2: // "private home with PV-battery":
                    d.Annual_Heatpower_consumption = 0;
                    d.JAZ = 1;
                    d.Annual_WhaterHeatpower_consumption = 0;
                    //d.battery_capacity = 0;
                    // d.Battery_power = 0;
                    for (int i = 0; i < 8760; i++) d.Current_consumption[i] = d.Loadprofile_home[i];
                    break;

                case 3: //   private home with PV-battery and heat pump":
                    for (int i = 0; i < 8760; i++) d.Current_consumption[i] = d.Loadprofile_home[i];
                    break;
                case 4: // "office 8 a.m. to 5 p.m. with PV-battery":
                    d.Annual_Heatpower_consumption = 0;
                    d.JAZ = 1;
                    d.Annual_WhaterHeatpower_consumption = 0;
                    for (int i = 0; i < 8760; i++) d.Current_consumption[i] = d.Loadprofile_office[i];
                    //d.battery_capacity = 0;
                    // d.Battery_power = 0;

                    break;
                case 5: // "Industry 24/7 3 shifts with PV-battery":
                    d.Annual_Heatpower_consumption = 0;
                    d.JAZ = 1;
                    d.Annual_WhaterHeatpower_consumption = 0;
                    //d.battery_capacity = 0;
                    // d.Battery_power = 0;
                    for (int i = 0; i < 8760; i++) d.Current_consumption[i] = d.Loadprofile_1shift[i];

                    break;
                case 6: // "Industry 24/7 3 shifts with PV-battery":
                    d.Annual_Heatpower_consumption = 0;
                    d.JAZ = 1;
                    d.Annual_WhaterHeatpower_consumption = 0;
                    //d.battery_capacity = 0;
                    // d.Battery_power = 0;
                    for (int i = 0; i < 8760; i++) d.Current_consumption[i] = d.Loadprofile_2shift[i];

                    break;
            }

            for (int i = 0; i < 8760; i++)
            {
                d.day[i] = calcDay(i);
                d.declination[i] = calcDeclination(d.day[i]);
                d.TEQ_min[i] = calcTEQ_min(d.day[i]);
                d.MLT_adj[i] = calcMLT_adj(i, d.gps_east);
                d.TLT_adj[i] = calcTLT_adj(i, d.MLT_adj[i], d.TEQ_min[i]);
                d.hour_angle[i] = calchour_angle(d.TLT_adj[i]);
                d.sun_elevation_angle[i] = calcsun_elevation_angle(d.hour_angle[i], d.declination[i], d.gps_north);
                d.adjusted_solarconstant[i] = calcadjusted_solarconstant(d.day[i]);
                d.I[i] = calcI(d.adjusted_solarconstant[i], d.sun_elevation_angle[i]);
                d.K[i] = calcK(d.GHI[i], d.I[i]);
                d.DHI[i] = calcDHI(d.GHI[i], d.K[i]);// not checked
                d.BHI[i] = calcBHI(d.DHI[i], d.GHI[i]); // not checked

                d.sun_azimuth[i] = calcsun_anzimuth((i + 1) % 24, 90 - d.gps_north, BOGENMASS(d.sun_elevation_angle[i]), d.declination[i]);
                d.angle_of_incidence[i] = calcangle_of_incidence(BOGENMASS(d.sun_elevation_angle[i]), BOGENMASS(d.sun_azimuth[i]), d.tilt, d.south_angle + 180);
                d.BNI[i] = calcBNI(d.sun_elevation_angle[i], BOGENMASS(d.sun_elevation_angle[i]), d.BHI[i], true, d.Beam_horizontal_irradiation[i]);            // not checked
                d.r_hor_total[i] = calcr_hor_total(true, d.BNI[i], d.DHI[i], d.Diffuse_horizontal_irradiation[i]);  // not checked
                d.r_tilted_total[i] = calcr_tilted_total(d.BNI[i], d.angle_of_incidence[i], d.DHI[i], d.tilt, d.r_hor_total[i], d.albedo); // not checked
                d.t_modul[i] = calct_modul(d.Ambient_Temperature[i]+d.AverageTemp-10.7, d.cooling_factor, d.r_tilted_total[i]);
                d.yield_adjusted[i] = calcyield_adjusted(d.peak_power, d.derating_factor, d.r_tilted_total[i], d.delta_T, d.t_modul[i]);
                d.daily_power[(int)Math.Truncate(i / 24.0)] += d.yield_adjusted[i];
                d.monthly_power[calcMonth(i)] += d.yield_adjusted[i] / 1000; // and scale to kWh
                d.anual_poroduction += d.yield_adjusted[i];



            }
            d.calc_ok = true;
            d.anual_poroduction /= 1000; // change unite to kWh
            d.battery_actual_level = 0;
            d.grid_feed_in = 0;
            d.direct_consumption = 0;
            d.grid_consumption = 0;
            d.PV_Heatpump_Transfer = 0;

            d.monthly_heat= new double[12];
            d.monthly_water =new double[12];
            d.monthly_current=new double[12];
           

            for (int i = 0; i < 8760; i++)
            {
                double consumption = (d.Annual_current_consumption * d.Current_consumption[i] + (d.Annual_Heatpower_consumption * d.Heat_consumption[i] + d.Annual_WhaterHeatpower_consumption * d.Waterheat_consumption[i]) / d.JAZ)*1000; // change from MWh to kWh
                double production = d.yield_adjusted[i] / 1000;  // change from W to kW
                d.monthly_current[calcMonth(i)] += d.Annual_current_consumption * d.Current_consumption[i];
                d.monthly_heat[calcMonth(i)]+= d.Annual_Heatpower_consumption * d.Heat_consumption[i] / d.JAZ;
                d.monthly_water[calcMonth(i)] += d.Annual_WhaterHeatpower_consumption * d.Waterheat_consumption[i] / d.JAZ; // change from MWh to kWh


            if (production > consumption)
                {   // calculate how much PV power is used for heatpump
                    d.PV_Heatpump_Transfer += (d.Annual_Heatpower_consumption * d.Heat_consumption[i] + d.Annual_WhaterHeatpower_consumption * d.Waterheat_consumption[i]) / d.JAZ * 1000;
                    // clculate direct PV Power consumption
                    d.direct_consumption += consumption;
                    if (d.battery_actual_level < d.battery_capacity) // load battery
                    {
                        if (production - consumption > d.Battery_power)
                        {
                            d.battery_actual_level += d.Battery_power;
                            d.grid_feed_in += production - consumption - d.Battery_power;
                        }
                        else d.battery_actual_level += production - consumption;
                    }
                    else // feed grid
                    {
                        d.grid_feed_in += production - consumption;
                    }
                }
                else
                { // get additional power from Battery or grid
                    double on_top_need = consumption - production;
                    
                    // calculate amount of PV used for Heatpump
                    double actual_current_onsumption = d.Current_consumption[i];
                    double heat_pump_need = (d.Annual_Heatpower_consumption * d.Heat_consumption[i] + d.Annual_WhaterHeatpower_consumption * d.Waterheat_consumption[i]) / d.JAZ * 1000;
                    // calculate battery 
                    if (d.battery_actual_level > 0.1 * d.battery_capacity + d.Battery_power)
                    {  // battery consumption
                        if (on_top_need > d.Battery_power)
                        {
                            d.direct_consumption += d.Battery_power + production;
                            d.grid_consumption += on_top_need - d.Battery_power;
                            d.battery_actual_level -= d.Battery_power;
                            if (production + d.Battery_power > consumption)
                            {
                                if (heat_pump_need + actual_current_onsumption< production + d.Battery_power )
                                {
                                    d.PV_Heatpump_Transfer += heat_pump_need;
                                }
                                else
                                    d.PV_Heatpump_Transfer += production + d.Battery_power - actual_current_onsumption;
                            }

                        }
                        else  // just al little needed from Battery
                        {
                            d.direct_consumption += on_top_need + production;
                            d.battery_actual_level -= on_top_need;
                            d.grid_consumption += 0;
                            if (production + on_top_need > actual_current_onsumption)
                            {
                                if (heat_pump_need + actual_current_onsumption < production + on_top_need )
                                {
                                    if (heat_pump_need > 0)
                                       d.PV_Heatpump_Transfer += heat_pump_need;
                                }
                                else 
                                    d.PV_Heatpump_Transfer += production + on_top_need - actual_current_onsumption;


                            }
                        }
                    }
                    else  // pure grid add on
                    {
                        d.direct_consumption += production;
                        d.grid_consumption += on_top_need;

                    }


                       }
            } // for loop

            /*  double own = d.direct_consumption / d.anual_poroduction;
              double gridfeed = d.grid_feed_in / d.anual_poroduction;
              double total_need =(d.Annual_current_consumption + (d.Annual_Heatpower_consumption + d.Annual_WhaterHeatpower_consumption) / d.JAZ)*1000;// change from MWh to kWh
              double grid = d.grid_consumption / total_need;

              double PVtoHeat= d.PV_Heatpump_Transfer/ d.anual_poroduction;
              double a=0;
              for (int i = 0; i < 12; i++)
                  a += d.monthly_heat[i] * d.JAZ;*/
         

            return;
        }
    
        
        private double calct_modul(double AI4, double temp_coeff, double AM4)
        {
            // =AI4+$B$33*(AM4/1000)
            double T = AI4+temp_coeff*(AM4/1000);
            return T;
        }
        private double calcyield_adjusted(double P, double derating, double r_tilt , double dt, double T_Module)
        {
            // =$B$29*$B$13*AM4*(1+($B$30/100)*(AN4-25))
            double T = P*derating*r_tilt*(1+(dt/100)*(T_Module-25));
            return T;
        }
        private double calcr_tilted_total(double AF4 , double AD4, double AG4, double B11, double AH4, double B32)
        {
            // r_tilted_dir=MAX(AF4* COS(AD4);0)
            double T = AF4 * Math.Cos(BOGENMASS(AD4));
            if (T < 0 )T = 0;

            //r_tilted_dif=AG4*0,5*(1+COS(BOGENMASS($B$11)))
            double T2 = AG4 * 0.5 * (1 + Math.Cos(BOGENMASS(B11)));

            //r_tilted_ref= AH4 *$B$32 * 0,5 * (1 - COS(BOGENMASS($B$11)))
            double T3 = AH4 * B32 * 0.5*(1 - Math.Cos(BOGENMASS(B11)));

            //r_tilted_total
            T = T + T2 + T3;

            return T; 

        }

        private double calcBNI(double V4, double U4, double AA4, bool IP_E16, double IP_S5)
        {  // das 13 Element ist 0,23
          //  = WENN(V4 > 1; WENN(UND(Input!$E$16 = "yes"; V4 > 0)= WAHR; (AA4 / SIN(V4)); WENN(UND(Input!$E$16 = "no"; Calculation!V4 > 0)= WAHR; (Input!S5 / SIN(Calculation!V4)); 0)); Input!S5)
            double T = 0;
            if (V4 > 1) {
                if (IP_E16 == true && V4 > 0) T = AA4 / Math.Sin(U4);
                else if (IP_E16 == false && U4 > 0) T = IP_S5 / Math.Sin(U4);
            }
            else T = IP_S5;
                
              
            return T;


        }
        private double  calcr_hor_total(bool E16, double BNI, double Z28, double IP_T29)
        { //=WENN(Input!$E$16 = "yes";Calculation!Z28;Input!T29)
            double T =0;
            if (E16 == true) T = Z28; else T= IP_T29;
            T = T + BNI;
            return T;


        }
       
        private double calcDay(int i) // calcultes day but scales 0-360  !!!!!!
        {
            
               

            double day=Math.Truncate((i-0.000001)/24.0)+1 ;
            if (day < 1) day = 1 ;
            return 360.0/365.0*day;

        }
        private double calcDeclination(double day)
        {
            //=BOGENMASS(0,3948-23,2559*COS(BOGENMASS(G11+9,1))-0,3915*COS(BOGENMASS(2*G11+5,4))-0,1764*COS(BOGENMASS(3*G11+26)))
            //-0,402
           
            double result = BOGENMASS(0.3948
                 - 23.2559 * Math.Cos(BOGENMASS(day + 9.1))
                 - 0.3915 * Math.Cos(BOGENMASS(2 * day + 5.4))
                 - 0.1764 * Math.Cos(BOGENMASS(3 * day + 26.0)));


            return result;
        }
        private double calcTEQ_min(double day)
        { //=(0,0066+7,3525*COS(BOGENMASS(G11+85,9))+9,9359*COS(BOGENMASS(2*G11+108,9))+0,3387*COS(BOGENMASS(3*G11+105,2)))
          // 	-3,240
            double result = 0.0066
                + 7.3525 * Math.Cos(BOGENMASS(day + 85.9)) 
                + 9.9359 * Math.Cos(BOGENMASS(2 * day + 108.9)) 
                + 0.3387 * Math.Cos(BOGENMASS(3 * day + 105.2));

          
            return result;

        }
        private double calcMLT_adj(int i, double gps_East)
        {
            //83,57
            double T = (i%24) * 60.0 + 30.0 + gps_East*4.0;
            return T;

        }

        private double calcTLT_adj(int i, double MLT_adj, double TEQ_Min)
        {
            //80,33
            double T = MLT_adj + TEQ_Min;
           
            return T;

        }

     
         private double calchour_angle( double TLT_adj)
        {
            //159,92
            double T =BOGENMASS(15*(12- TLT_adj / 60));
            T = Grad(T);
            return T;

        }

        private double calcsun_elevation_angle(double hour_angle, double declination, double GPS_North)
        {//-1,1786 -67,53
            double T = Math.Asin(Math.Cos(BOGENMASS(hour_angle)) * Math.Cos(declination) * Math.Cos(BOGENMASS(90- GPS_North)) + Math.Sin(BOGENMASS(90- GPS_North)) * Math.Sin(declination));
            T=Grad(T);
            return T;
        }
        private double calcadjusted_solarconstant( double day)
        { //1403,60

            double T = 1356.5 + 48.5 * Math.Cos(0.01721 * (day*365.0/360.0- 15));
                 return T;
        }
        private double calcI(double adjusted_solarconstant, double sun_elevation_angle)
        { //-1297,03


            double T = adjusted_solarconstant*Math.Sin(BOGENMASS(sun_elevation_angle));
            return T;
        }
        private double calcK(double E4, double X4)
        { //0
             double T = 0;
            if (X4 > 0 && E4 > 0) T = E4 / X4;
            return T;
        }
        private double calcDHI(double GHI,double K )
        { //
            double T = GHI * 0.147;
            // = WENN(UND(Y4 < 0, 3; Y4 >= 0); GHI * (1, 02 - 0, 249 * Y4);
            // WENN(UND(Y4 > 0, 3; Y4 < 0,78); GHI * (1, 45 - 1, 67 * Y4); GHI * 0,147))
            if (K < 0.3 && K >= 0) T = GHI * (1.02 - 0.249 * K);
            if (K > 0.3 && K < 0.78) T = GHI * (1.45 - 1.67 * K);
            return T;
        }
        private double calcBHI( double DHI,double GHI)
        { //
            double T = 0;
            if ((GHI - DHI) < 0) T = 0;
            else T = GHI - DHI;
            return T;
        }
       


        private double calcsun_anzimuth(double S4, double Longitude,double V4, double I4)
        { //0,9732	55,76
          // =WENN(S6<=12;PI()-ARCCOS((SIN(V6)*SIN(BOGENMASS($B$9))-SIN(I6))/(COS(V6)*COS(BOGENMASS($B$9))));PI()+ARCCOS((SIN(V6)*SIN(BOGENMASS($B$9))-SIN(I6))/(COS(V6)*COS(BOGENMASS($B$9)))))
            double T = 0;
         if(S4<=12)
               T= Math.PI - Math.Acos((Math.Sin(V4) * Math.Sin(BOGENMASS(Longitude)) - Math.Sin(I4)) / (Math.Cos(V4) * Math.Cos(BOGENMASS(Longitude)))); 
          else T=Math.PI + Math.Acos((Math.Sin(V4) * Math.Sin(BOGENMASS(Longitude)) - Math.Sin(I4)) / (Math.Cos(V4) * Math.Cos(BOGENMASS(Longitude))));
            T = Grad(T);
            return T;
        }
        private double calcsun_anzimuth_falsch(double R4, double Latitude, double U4, double H4)
        { //0,9732	55,76
          // =WENN(S6<=12;PI()-ARCCOS((SIN(V6)*SIN(BOGENMASS($B$9))-SIN(I6))/(COS(V6)*COS(BOGENMASS($B$9))));PI()+ARCCOS((SIN(V6)*SIN(BOGENMASS($B$9))-SIN(I6))/(COS(V6)*COS(BOGENMASS($B$9)))))
            double T = 0;
            //  =WENN(S4<=12; PI()-ARCCOS((SIN(V4)* SIN(BOGENMASS($B$7))-SIN(H4))/(COS(V4)* COS(BOGENMASS($B$7))));PI()+ARCCOS((SIN(V4)* SIN(BOGENMASS($B$7))-SIN(H4))/(COS(V4)* COS(BOGENMASS($B$7)))))
            if (R4 <= 12)
                T = Math.PI - Math.Acos((Math.Sin(U4) * Math.Sin(BOGENMASS(Latitude)) - Math.Sin(H4)) / (Math.Cos(U4) * Math.Cos(BOGENMASS(Latitude))));
            else T = Math.PI + Math.Acos((Math.Sin(U4) * Math.Sin(BOGENMASS(Latitude)) - Math.Sin(H4)) / (Math.Cos(U4) * Math.Cos(BOGENMASS(Latitude))));
            T = Grad(T);
            return T;
        }

        private double calcangle_of_incidence(double U4, double AB4, double B11, double B12)
        { // angle_of_incidence [rad]	angle_of_incidence [°]
           // 2,7865  159,65

            double T = 0;
            T = Math.Acos(-Math.Cos(U4) * Math.Sin(BOGENMASS(B11)) * Math.Cos(AB4 - (BOGENMASS(B12) - Math.PI)) + Math.Sin(U4) * Math.Cos(BOGENMASS(B11)));
            T=Grad(T);
            return T;
        }

        private double BOGENMASS(double winkel)
        {
            return winkel * Math.PI / 180.0;
        }
        private double Grad(double winkel)
        {
            return winkel * 180.0/Math.PI ;
        }

        private int calcMonth(int hourOfYear)
        {
            // Definiere die Anzahl der Tage in jedem Monat (nicht Schaltjahr)
            int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            // Berechne den Tag des Jahres
            int dayOfYear = hourOfYear / 24 + 1; // +1, da wir bei Tag 1 anfangen

            // Finde den Monat
            int month = 1;
            foreach (int days in daysInMonth)
            {
                if (dayOfYear <= days)
                {
                    break;
                }
                dayOfYear -= days;
                month++;
            }

            return month-1;
        }
        private void readStandardRadiation(ref Solar_Data Data, string filePath)
        {
            

            // Absoluten Pfad bestimmen
            //string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            //string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string fullPath = System.IO.Path.Combine(projectDirectory, filePath);
          
            if (!File.Exists(fullPath))
            {
                MessageBox.Show("Data\\help.csv not fund: "+fullPath);
              
                return; 
            }

                using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader reader = new StreamReader(fs))
            {
                

        string line = reader.ReadLine();
                line = reader.ReadLine();
                 double a = 0;
  

                for (int i = 0; i < 8760; i++) 
                {
                    line = reader.ReadLine();

                    // Splitte die Zeile anhand von ';'
                    string[] parts = line.Split(';');
                    CultureInfo germanCulture = CultureInfo.GetCultureInfo("de-DE");
                 
                    if (parts.Length == 10)
                    {
                        try
                        {
                            // Konvertiere jede der 4 Teile in double
                            Data.GHI[i] = double.Parse(parts[0], germanCulture);
                            Data.Beam_horizontal_irradiation[i] = double.Parse(parts[1], germanCulture);
                            Data.DHI[i]= double.Parse(parts[2], germanCulture);
                            Data.Ambient_Temperature[i]= double.Parse(parts[3], germanCulture);
                            Data.Loadprofile_home[i]=double.Parse(parts[4], germanCulture);
                            Data.Heat_consumption[i] = double.Parse(parts[5], germanCulture);
                            Data.Waterheat_consumption[i] = double.Parse(parts[6], germanCulture);
                            Data.Loadprofile_office[i] = double.Parse(parts[7], germanCulture);
                            Data.Loadprofile_1shift[i] = double.Parse(parts[8], germanCulture);
                            Data.Loadprofile_2shift[i] = double.Parse(parts[9], germanCulture);

                            a += Data.Loadprofile_2shift[i];



                        }
                        catch (FormatException)
                        {

                            MessageBox.Show("Fehler bei der Konvertierung der csv-Werte in Double.");
                            return;
                        }
                    }
                   
                }



                // calculate direct consumption, greed feedin , batterie status

              
      
            
                   return;
            }
        }

     }
        
    
}
