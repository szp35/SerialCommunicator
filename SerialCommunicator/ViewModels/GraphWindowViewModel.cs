using SerialCommunicator.Graph;
using SerialCommunicator.Graph.DataPoints;
using SerialCommunicator.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SerialCommunicator.ViewModels
{
    public class GraphWindowViewModel : BaseViewModel
    {
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> Controller { get; set; }

        /// <summary>
        /// The graph is constantly plotted forever. So to 'plot data', change this value 
        /// to affect the To-Be-Plotted Y value which will be plotted eventually.
        /// </summary>
        public double ActivePlotValue { get; set; }

        public DispatcherTimer GraphTimer { get; set; }

        public GraphWindowViewModel()
        {
            Controller = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            Controller.Range.MinimumY = 0;
            Controller.Range.MaximumY = 1024;
            Controller.Range.MaximumX = TimeSpan.FromSeconds(10);
            Controller.Range.AutoY = true;

            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Serial Values",
                Stroke = Color.FromRgb(11, 99, 205)
            });

            GraphTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };

            GraphTimer.Tick += GraphTimer_Tick;

            StartPlotting();
        }
        private Stopwatch GraphStopWatch = new Stopwatch();
        private void GraphTimer_Tick(object sender, EventArgs e)
        {
            Controller.PushData(GraphStopWatch.Elapsed, ActivePlotValue);
        }
        public void StartPlotting()
        {
            GraphStopWatch = new Stopwatch();
            GraphStopWatch.Start();
            GraphTimer.Start();
        }

        public void StopPlotting()
        {
            //PlotLoopRunning = false;
            GraphStopWatch.Stop();
            GraphTimer.Stop();
        }

        /// <summary>
        /// Plots a value onto the graph. only a Y value is required as the graph is time-based (X is used by the timer)
        /// </summary>
        /// <param name="yValue">The height of the plot. Min = 0, Max = 1024</param>
        public void PlotGraph(double yValue)
        {
            if (yValue >= 0 && yValue <= 1023)
            {
                ActivePlotValue = yValue;
            }
            else
            {
                ActivePlotValue = 1024 / yValue * (yValue / 10);
            }
        }
    }
}
