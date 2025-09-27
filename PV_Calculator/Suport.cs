using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace PV_Calculator
{
    public static class Suport
    {
        public static double round_up(double x)
        {  // x aufrunden auf einen graden wert
            double dez_points = Math.Log10(x);
            dez_points = Math.Floor(dez_points);
            dez_points = Math.Pow(10, dez_points);
            x = x / dez_points;
            if (x > 8) return 10 * dez_points;
            if (x > 6) return 8 * dez_points;
            if (x > 5) return 6 * dez_points;
            if (x > 4) return 5 * dez_points;
            if (x > 3) return 4 * dez_points;
            if (x > 2.5f) return 3 * dez_points;
            if (x > 2.0f) return 2.5f * dez_points;
            if (x > 1.5f) return 2.0f * dez_points;
            return 1.5f * dez_points;
        }

        public static double calcLevelized_AnnualCost(Solar_Data Data)
        {

            double PV_Bat_cost = Data.pv_cost * Data.peak_power + Data.battery_capacity * Data.battery_cost*2; // in 20 years 2 batteries are needed
            double gridconsumption_Heatpump = (Data.Annual_Heatpower_consumption + Data.Annual_WhaterHeatpower_consumption) / Data.JAZ * 1000 - Data.PV_Heatpump_Transfer;
            double gridconsumption_el = Data.grid_consumption - gridconsumption_Heatpump;
           // Data.inflation = 0.02;
            double discount_factor = 0;
            for (int i = 0; i < 20; i++)
            {
                discount_factor += Math.Pow(1 + Data.inflation/100, i);
            }

            double LCO = PV_Bat_cost;   // levelized cost
            LCO += Data.el_cost * gridconsumption_el * discount_factor;
            LCO += Data.el_cost_heatpump * gridconsumption_Heatpump * discount_factor;
            LCO -= Data.grid_feed_in * Data.feed_in_tarif * discount_factor;
            LCO /= discount_factor;
            return LCO;
        }
    }
}
