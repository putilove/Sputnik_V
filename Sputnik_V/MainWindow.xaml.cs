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
                listSeries[1].Color = OxyColor.FromRgb(247, 0, 255);
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

            listSeries.Add(new LineSeries());//x ydx
            for (int i = 0; i < bcvm.valuesDict["x"].Count; i++)
            {
                listSeries[4].Points.Add(new DataPoint(bcvm.valuesDict["x"][i], bcvm.valuesDict["ydx"][i]));
            }
            var z = 0;

            listSeries[3].LineStyle = LineStyle.Dot;

            var plotModel = new PlotModel();
            foreach (var plt in listSeries)
            {
                plt.LegendKey = z.ToString();
                plotModel.Series.Add(plt);
            }

            plotView.Model = plotModel;
        }
    }
}
