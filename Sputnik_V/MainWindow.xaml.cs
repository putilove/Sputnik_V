using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using Sputnik_V.Model;
//using ScottPlot.Renderable;
//using ScottPlot.WPF;

namespace Sputnik_V
{
    public partial class MainWindow : Window
    {
        public BCVM bcvm = new BCVM();

        public MainWindow()
        {
            InitializeComponent();

            var dict = new Dictionary<string, double[]>();
            /*dict.Add("x", new double[] { 0 });
            dict.Add("yv", new double[] { 0 });
            dict.Add("xe", new double[] { 0 });
            dict.Add("ex", new double[] { 0 });
            dict.Add("edx", new double[] { 0 });
            dict.Add("yx", new double[] { 0 });
            dict.Add("ydx", new double[] { 0 }); */
            bcvm.Move();
            var listSeries = new List<LineSeries>();

            listSeries.Add(new LineSeries());//x yv      
            for(int i = 0; i < bcvm.valuesDict["x"].Count; i++)
            {
                listSeries[0].Points.Add(new DataPoint(bcvm.valuesDict["x"][i], bcvm.valuesDict["yv"][i]));
            }

            listSeries.Add(new LineSeries());//xe ex
            for (int i = 0; i < bcvm.valuesDict["xe"].Count; i++)
            {
                listSeries[1].Points.Add(new DataPoint(bcvm.valuesDict["xe"][i], bcvm.valuesDict["ex"][i]));
            }

            listSeries.Add(new LineSeries());//xe edx
            for (int i = 0; i < bcvm.valuesDict["xe"].Count; i++)
            {
                listSeries[2].Points.Add(new DataPoint(bcvm.valuesDict["xe"][i], bcvm.valuesDict["edx"][i]));
            }

            listSeries.Add(new LineSeries());//x yx
            for (int i = 0; i < bcvm.valuesDict["x"].Count; i++)
            {
                listSeries[3].Points.Add(new DataPoint(bcvm.valuesDict["x"][i], bcvm.valuesDict["yx"][i]));
            }

            listSeries.Add(new LineSeries());//x yx
            for (int i = 0; i < bcvm.valuesDict["x"].Count; i++)
            {
                listSeries[4].Points.Add(new DataPoint(bcvm.valuesDict["x"][i], bcvm.valuesDict["ydx"][i]));
            }
            var z = 0;
            // Создание модели графика и добавление серии данных
            var plotModel = new PlotModel();
            foreach (var plt in listSeries)
            {
                plt.LegendKey = z.ToString();
                plotModel.Series.Add(plt);
            }
            // Установка модели графика для PlotView
            plotView.Model = plotModel;
        }
    }
}
