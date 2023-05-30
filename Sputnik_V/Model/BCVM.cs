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

        /// <summary>
        /// Двигатель маховик
        /// </summary>
        public FlyWheelEngine Engine { get; set; }

        /// <summary>
        /// Генератор ошибок
        /// </summary>
        public ErrorGenerator ErrorGen { get; set; }

        /// <summary>
        /// Корректор ошибок
        /// </summary>
        public ErrorCorrector ErrorCorr { get; set; }

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

            Engine = new FlyWheelEngine();
        }

        private Func<double, double> v_speed = w => w * Consts.R;

        public void Move(double x = 0, double dx = 0, double T = 2, double dt = 0.01, int mode = 0, int isStick = 2, int t_error = 60, int fix_status = 0)
        {
            ErrorGen = new ErrorGenerator(mode, isStick, t_error, fix_status);
            ErrorCorr = new ErrorCorrector(Engine, mode, isStick, t_error, fix_status);
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
                double prevX = x, prevW = w, prevDx = dx;
                Engine.Overclocking(ref x, ref dx, ref t, dt, ref w, ref dw, steps, valuesDict, true);
                (x, w) = ErrorGen.GenerateError(x, w, t, valuesDict);
                (x, w, t) = ErrorCorr.CorrectError(x, dx, w, dw, prevX, prevDx, prevW, t, dt, steps, valuesDict);
                

                valuesDict["x"].Add(t);
                valuesDict["yx"].Add(x);
                valuesDict["ydx"].Add(dx);
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
            mode = 0;
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

                Engine.Unloading(ref x, ref dx, ref t, dt, ref w, dw, steps, ref valuesDict, dx_eiler, mode, t_error, fix_status, flag);
                
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

                valuesDict["x"].Add(t);
                valuesDict["yx"].Add(x_base);
                valuesDict["ydx"].Add(dx_base);
                valuesDict["yv"].Add(v_speed(w));

                valuesDict["cdx"].Add((valuesDict["yx"].Last() - valuesDict["yx"][valuesDict["yx"].Count - 2]) / T);
                integral += (valuesDict["ydx"].Last() + valuesDict["ydx"][valuesDict["ydx"].Count - 2]) * T / 2;
                valuesDict["cx"].Add(integral);
            }
        }
        private double relay_function(double x, double dx)
        {
            var fi = Consts.K0 * x + Consts.K1 * dx;
            return Math.Abs(fi) > Consts.A ? (fi > 0 ? 1 : -1) : 0;
        }

    }
}
