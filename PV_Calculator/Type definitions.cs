
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using PV_Calculator;



public struct Solar_Data  // hourly data of a Year
{
    public String Name; // Name of the Project
    public double[] time;
    public double[] day;// attention 0 to 360 
    public double[] daily_power;
    public double[] monthly_power;

    public double[] hour_angle;
    public double[] declination;
    public double[] TEQ_min; 
    public double[] MLT_adj;
    public double[] TLT_adj;
    public double[] sun_elevation_angle;
    public double[] adjusted_solarconstant;
    public double[] I;
    public double[] K;
   
    public double[] DHI;
    public double[] BHI;
    public double[] sun_azimuth;
    public double[] angle_of_incidence;
    public double[] BNI;
    public double[] r_hor_total;
    public double[] temperature;
    public double[] r_tilted_total;
    public double[] t_modul;
    public double[] yield_adjusted;


    public double[] GHI; //GHI to be loaded from File
    public double[] Beam_horizontal_irradiation; // BHI to be loaded from File
    public double[] Diffuse_horizontal_irradiation; // DHI to be loaded from File
    public double[] Ambient_Temperature; //  to be loaded from File  addd 2° for Berlin
    public double[] Current_consumption;
    public double[] Heat_consumption;
    public double[] Waterheat_consumption;
    public double[] monthly_heat;
    public double[] monthly_water;
    public double[] monthly_current;
    public double[] Loadprofile_home;
    public double[] Loadprofile_office;
    public double[] Loadprofile_1shift;
    public double[] Loadprofile_2shift;



    public Int32 count;
    public double tilt;  // angel of PV Panel to Horizon
    public double south_angle;  // south =0   West = 90  North = 180 East =270 or -90;
    public double cooling_factor; // roof-parallel, well ventilated	29,0  roof-integrated - back-ventilated	32  roof-integrated - not back-ventilated	43  elevated – roof-top	28  elevated – ground-mounted	22
    public double delta_T; // temperatur coeeficient  typ -0,48
    public double gps_north; // eg. Berlin = 52,532   https://gps-koordinaten.com  Singapore = 1.3147 Grad
    public double gps_east;  // eg. Berlin = 13,392   Singapore = 103.8454093
    public double derating_factor; // typical 0,85   for aging, Dust, converterloses, Cablelosses
    public double peak_power;  // Number of pv-panels times Rated STC power of each panel in kW
    public double albedo; // losses thrue reflection on Pannel. Typical 0,2
    public bool calc_ok; // true propper calculation done
    public String date; // date of calculation
    public double panel_number;  //number of pv panels
    public double panel_power; // nominal power of a single pv panel at STC (standart test conditions) in W
    //public double consumption; // Anlaul el. power consumption of the load in  MWh
    public Int32 consumption_type; // 1:   private home   2: private home with PV-battery 3:private home with el.heat pump 4: office 8 a.m.to 5 p.m. 5: Industry 24/7 3 shifts
    public double el_cost;// kost per kWh for el Power from the grid
    public double feed_in_tarif; // pay back per kWh for el Power feeded in the grid
    public double pv_cost; // price for solar systems per kWpeak
    public double battery_cost; // price for battery systems per kWhel
    public double anual_poroduction;
    public double battery_capacity;  // Ah
    public double battery_actual_level; //ah
    public double Battery_power; // pbidirectional converter power in W
    public double grid_feed_in;
    public double direct_consumption;
    public double grid_consumption;
    public double Annual_current_consumption; // kWh
    public double Annual_Heatpower_consumption; // kWh
    public double Annual_WhaterHeatpower_consumption; // kWh
    public double JAZ; // typical 3.5 for air to water heatpunps
    public double el_cost_heatpump; // price for Heatpump current; hofully a little cheaper than standart current
    public double PV_Heatpump_Transfer; // price for Heatpump current; hofully a little cheaper than standart current
    public double inflation;
    public double AverageTemp;// annual average Temperature of the Town of the PV System
    public bool   Load_new_NASA_Data; // just required if GPS coordinates have changed


}