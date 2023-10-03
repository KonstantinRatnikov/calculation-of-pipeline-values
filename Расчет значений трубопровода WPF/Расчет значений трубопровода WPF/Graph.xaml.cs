using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows;


namespace Расчет_значений_трубопровода_WPF
{
    /// <summary>
    /// Логика взаимодействия для Graph.xaml
    /// </summary>
    public partial class Graph : Window
    {
        PlotModel plotModel;

        /// <summary>
        /// Вызов окна Graph и построение графика по точкам
        /// </summary>
        /// <param name="name"> название графика</param>
        /// <param name="points"> массив с точками ,по которым строится график</param>
        public Graph(string name, Point[] points)
        {
            InitializeComponent();

            plotModel = new PlotModel { Title = name };

            BuildGraph(points, "Длина участка трубопровода, м", "Напряжения, Па");
        }
        /// <summary>
        /// Вызов окна Graph и построение графика по полиному от 0 до l
        /// </summary>
        /// <param name="name">название графика</param>
        /// <param name="polinom">коэффициенты полинома</param>
        /// <param name="l"> длина участка</param>
        public Graph(string name, double[] polinom, double l)
        {
            InitializeComponent();

            plotModel = new PlotModel { Title = name };

            BuildGraph(polinom, l, "Длина участка трубопровода, м", "Прогиб, м");
        }
        /// <summary>
        /// Построение графика по полиному от 0 до l
        /// </summary>
        /// <param name="polinom">коэффициенты полиномаparam>
        /// <param name="l">длина участка</param>
        void BuildGraph(double[] polinom, double l, string nameX, string nameY)
        {
            var seriesP = new LineSeries();
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = nameX,
                AxisTitleDistance = 10
            };
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = nameY,
                AxisTitleDistance = 10
            };

            for (double i = 0; i < l; i += 0.1)
            {
                var y = polinom[0] * i * i * i * i + polinom[1] * i * i * i + polinom[2] * i * i + polinom[3] * i;
                seriesP.Points.Add(new DataPoint(i, y));
            }

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);
            plotModel.Series.Add(seriesP);
            plotView.Model = plotModel;
        }
         /// <summary>
         /// Построение графика по точкам
         /// </summary>
         /// <param name="points"> массив точек</param>
        void BuildGraph(Point[] points,string nameX, string nameY)
        {
            var series = new LineSeries();
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = nameX,
                AxisTitleDistance = 10
            };
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = nameY,
                AxisTitleDistance = 10
            };
            series.MarkerType = OxyPlot.MarkerType.Circle;
            for (int i = 0; i < points.Length; i++)
            {
                series.Points.Add(new DataPoint(points[i].X, points[i].Y));
            }
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);
            plotModel.Series.Add(series);
            plotView.Model = plotModel;
        }
    }
}
