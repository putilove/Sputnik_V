using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sputnik_V.Model
{
    public class ParameterConsts
    {
        /// <summary>
        /// Момент инерции спутника
        /// </summary>
        public const double I0 = 20;

        /// <summary>
        /// Масса спутника
        /// </summary>
        public const double M_sp = 350;

        /// <summary>
        /// Размер стороны спутника
        /// </summary>
        public const double Side_sp = 0.8;

        /// <summary>
        /// Момент инерции маховика
        /// </summary>
        public const double Imx = 0.02;

        /// <summary>
        /// Максимальная скорость маховика
        /// </summary>
        public const double W_max = 900;

        /// <summary>
        /// Максимально достижимый кинетический момент маховика
        /// </summary>
        public const double H_max = Imx * W_max;

        /// <summary>
        /// Тяга управляющего ракетного двигателя
        /// </summary>
        public const double P_rc = 0.2;

        /// <summary>
        /// Плечо силы тяги
        /// </summary>
        public const double L_m = 0.4;

        /// <summary>
        /// Управляющее ускорение от УРД
        /// </summary>
        public const double M_rc_ = P_rc * L_m / I0;

        /// <summary>
        /// Максимальное ускорение при разгоне маховика
        /// </summary>
        public const double W_1_max = 10;

        /// <summary>
        /// Максимально возможный управляю-щий момент маховика
        /// </summary>
        public const double M_m_max = Imx * W_1_max;

        /// <summary>
        /// Ускорение торможения маховика при сбросе накопленного кинетического момента
        /// </summary>
        public const double W_1_b = 1;

        /// <summary>
        /// Момент, возникающий при торможении маховика и действующий на спутник
        /// </summary>
        public const double M_b = W_1_b * Imx;

        /// <summary>
        /// Угловое ускорение спутника при торможении маховика
        /// </summary>
        public const double M_b_ = M_b / I0;

        /// <summary>
        /// Возмущающее ускорение
        /// </summary>
        public const double M_p_ = 0.002;

        /// <summary>
        /// Коэффициент перед углом в вычислении dw на участке разгона
        /// </summary>
        public const double A0 = 10;

        /// <summary>
        /// Коэффициент перед угловой скоростью в вычислении dw на участке разгона
        /// </summary>
        public const double A1 = 40;

        /// <summary>
        /// Значение для релейной функции РУД
        /// </summary>
        public const double A = 0.01;

        /// <summary>
        /// Коэффициент перед углом в вычислении фи на участке торможения
        /// </summary>
        public const double K0 = 10;

        /// <summary>
        /// Коэффициент перед угловой скоростью в вычислении фи на участке торможения
        /// </summary>
        public const double K1 = 43;

        /// <summary>
        /// Радиус
        /// </summary>
        public const double R = 0.0004;
    }
}
