using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;
using Consts = Sputnik_V.Model.ParameterConsts;
namespace Sputnik_V.Model
{
    public class BCVM
    {
        /// <summary>
        /// Словарь, содержащий набор списков со значениями переменных на определенном шаге
        /// </summary>
        public Dictionary<string, List<double>> valuesDict;

        public BCVM()
        {
            valuesDict = new Dictionary<string, List<double>>()
            {
                {"x", new List<double>() },
                {"yv", new List<double>() },
                {"xe", new List<double>() },
                {"ex", new List<double>() },
                {"edx", new List<double>() },
                {"yx", new List<double>() },
                {"ydx", new List<double>() },
                {"x2", new List<double>() },
                {"cx", new List<double>() },
                {"cdx", new List<double>() },
            };
        }

        private Func<double, double> v_speed = w => w * Consts.R;

        private double relay_function(double x, double dx)
        {
            var fi = Consts.K0 * x + Consts.K1 * dx;
            return Math.Abs(fi) > Consts.A ? (fi > 0 ? 1 : -1) : 0;
        }

        public void Move(double x = 0, double dx = 0, double T = 2, double dt = 0.01, int mode = 0, int isStick = 0, double t_error = 60, int fix_status = 0)
        {
            double t = 0;
            double integral = x;
            var w = (Consts.A0 * x + Consts.A1 * dx) * T;
            var dw = Consts.A0 * x + Consts.A1 * dx;
            valuesDict["cdx"].Add(dx);
            valuesDict["cx"].Add(x);
            valuesDict["x"].Add(t);
            valuesDict["yv"].Add(v_speed(w));
            valuesDict["yx"].Add(x);
            valuesDict["ydx"].Add(dx);

            var steps = (int)(T / dt);
            var x_base = x;
            var dx_base = dx;
            var time_out = 3 * T;
            var flag = 0;
            while (w < Consts.W_max)
            {
                foreach (var i in Enumerable.Range(0, steps))
                {
                    var M_f = Consts.Imx * dw;
                    dx += (Consts.M_p_ - (M_f / Consts.I0)) * dt;
                    x += dx * dt;
                    t += dt;
                    valuesDict["xe"].Add(t);
                    valuesDict["ex"].Add(x);
                    valuesDict["edx"].Add(dx);
                }
                if (mode != 1 || t < t_error || isStick == 2 || flag == 1)
                    x_base = x;
                if (mode != 2 || t < t_error || isStick == 2 || flag == 1)
                    dx_base = dx;
                if (mode == 1 && isStick == 0 && t > t_error)
                    x_base = 0;
                if (mode == 2 && isStick == 0 && t > t_error)
                    dx_base = 0;
                if (mode == 1 && isStick == 2 && t >= t_error && flag == 0)
                {
                    x_base = 0.2;
                    flag = 1;
                }


                if (mode == 2 && isStick == 2 && t >= t_error && flag == 0)
                {
                    dx_base = 0.2;
                    flag = 1;
                }
                if (mode == 1 && fix_status == 1 && t >= t_error + time_out && isStick != 2)
                    x_base = valuesDict["cx"].Last();
                if (mode == 2 && fix_status == 1 && t >= t_error + time_out && isStick != 2)
                    dx_base = valuesDict["cdx"].Last();


                dw = Consts.A0 * x_base + Consts.A1 * dx_base;
                w += dw * T;
                valuesDict["x"].Add(t);
                valuesDict["yx"].Add(x_base);
                valuesDict["ydx"].Add(dx_base);
                valuesDict["yv"].Add(v_speed(w));

                if (valuesDict["yx"].Count > 1)
                {
                    valuesDict["cdx"].Add((valuesDict["yx"].Last() - valuesDict["yx"][valuesDict["yx"].Count - 2]) / T);
                    integral += (valuesDict["ydx"].Last() + valuesDict["ydx"][valuesDict["ydx"].Count - 2]) * T / 2;
                    valuesDict["cx"].Add(integral);
                }
            }
            var dx_eiler = dx_base;
            var t_max = t;

            while (w > 0)
            {
                if (mode == 0 || t < t_error || flag == 1)
                {
                    dx = dx_eiler = dx_base;
                    x = x_base;
                }


                if (mode == 1 && t > t_error && (t < t_error + time_out || fix_status == 0) && isStick != 2)
                    dx = dx_eiler = dx_base;


                if (mode == 2 && t > t_error && (t < t_error + time_out || fix_status == 0) && isStick != 2)
                {
                    dx_eiler += dt * (Consts.M_p_ + Consts.M_b_ - Consts.M_rc_ * relay_function(x, dx_base));
                    x = x_base;
                }
                if (fix_status == 1 && t >= t_error + time_out && isStick != 2)
                {
                    dx = dx_eiler = dx_base;
                    x = x_base;
                }

                if (mode == 1 && t > t_error && flag == 2)
                {
                    dx = dx_eiler = dx_base;
                    flag = 1;
                }
                if (mode == 2 && t > t_error && flag == 2)
                {
                    x = x_base;
                    flag = 1;
                }

                foreach (var i in Enumerable.Range(0, steps))
                {
                    if (mode != 2 || t < t_error || (t >= t_error + time_out && fix_status == 1) || flag == 1)
                        dx += dt * (Consts.M_p_ + Consts.M_b_ - Consts.M_rc_ * relay_function(x, dx));
                    else
                        dx = dx_eiler;
                    x += dx_eiler * dt;
                    t += dt;
                    valuesDict["xe"].Add(t);
                    valuesDict["ex"].Add(x);
                    valuesDict["edx"].Add(dx);
                }
                if (mode != 1 || t < t_error)
                    x_base = x;
                if (mode != 2 || t < t_error)
                    dx_base += T * (Consts.M_p_ + Consts.M_b_ - Consts.M_rc_ * relay_function(x_base, dx_base));

                if (mode == 1 && isStick == 0 && t > t_error && (t < t_error + time_out || fix_status == 0))
                    x_base = 0;
                if (mode == 2 && isStick == 0 && t > t_error && (t < t_error + time_out || fix_status == 0))
                    dx_base = 0;
                if (mode == 1 && fix_status == 1 && t >= t_error + time_out && isStick != 2)
                    x_base = x;
                if (mode == 2 && fix_status == 1 && t >= t_error + time_out && isStick != 2)
                    dx_base += T * (Consts.M_p_ + Consts.M_b_ - Consts.M_rc_ * relay_function(x_base, valuesDict["cdx"].Last()));

                if (flag == 1)
                {
                    x_base = x;
                    dx_base += T * (Consts.M_p_ + Consts.M_b_ - Consts.M_rc_ * relay_function(x_base, dx_base));
                }

                if (mode == 1 && isStick == 2 && t >= t_error && flag == 0)
                {
                    x_base = 0.2;
                    flag = 2;
                }
                if (mode == 2 && isStick == 2 && t >= t_error && flag == 0)
                {
                    dx_base = 0.2;
                    flag = 2;
                }

                w -= Consts.W_1_b * T;
                valuesDict["x"].Add(t);
                valuesDict["yx"].Add(x_base);
                valuesDict["ydx"].Add(dx_base);
                valuesDict["yv"].Add(v_speed(w));

                valuesDict["cdx"].Add((valuesDict["yx"].Last() - valuesDict["yx"][valuesDict["yx"].Count - 2]) / T);
                integral += (valuesDict["ydx"].Last() + valuesDict["ydx"][valuesDict["ydx"].Count - 2]) * T / 2;
                valuesDict["cx"].Add(integral);
            }
        }

    }
}
