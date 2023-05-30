using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Consts = Sputnik_V.Model.ParameterConsts;

namespace Sputnik_V.Model
{
    public class FlyWheelEngine
    {
        /// <summary>
        /// Разгон двигателя маховика
        /// </summary>
        public void Overclocking(ref double x, ref double dx, ref double t, double dt, ref double w, ref double dw, int steps, Dictionary<string, List<double>> valuesDict, bool isWrite) 
        {
            foreach (var i in Enumerable.Range(0, steps))
            {
                var M_f = Consts.Imx * dw;
                dx += (Consts.M_p_ - (M_f / Consts.I0)) * dt;
                x += dx * dt;
                t += dt;
                if (isWrite)
                {
                    valuesDict["xe"].Add(t);
                    valuesDict["ex"].Add(x);
                    valuesDict["edx"].Add(dx);
                }
            }
            dw = Consts.A0 * x + Consts.A1 * dx;
            w += dw * (dt * steps);
        }

        /// <summary>
        /// Разгрузка двигателя маховика
        /// </summary>
        public void Unloading(ref double x, ref double dx, ref double t, double dt, ref double w, double dw, int steps, ref Dictionary<string, List<double>> valuesDict, double dx_eiler, int mode, double t_error, int fix_status, int flag)
        {
            var time_out = 3 * (dt * steps);
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
            w -= Consts.W_1_b * (dt * steps);
        }

        private double relay_function(double x, double dx)
        {
            var fi = Consts.K0 * x + Consts.K1 * dx;
            return Math.Abs(fi) > Consts.A ? (fi > 0 ? 1 : -1) : 0;
        }
    }
}
