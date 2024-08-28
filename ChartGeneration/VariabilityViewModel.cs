using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartGeneration
{
  public class VariabilityViewModel
  {
    public void ScatterPoint(PlotModel plotView, 
      ScatterSeries scatterSeries,
      LineSeries constantLineSeries,
      LineSeries constantLineSeries2,
      List<double> ExcelData, double min, double max)
    {
      // Create a new PlotModel
      plotView.Title = "Scatter Plot with Constant Line";

      // Example large dataset: Generating a large number of points
      var data = JitterCalculation(ExcelData);

      // Create a ScatterSeries and add data points
      scatterSeries.Title = "Data Points";
      scatterSeries. MarkerType = MarkerType.Circle;
      scatterSeries. MarkerSize = 1;
      scatterSeries. MarkerFill = OxyColors.Blue;

      // Adding data points to the series
      foreach (var point in data)
      {
        scatterSeries.Points.Add(new ScatterPoint(point.X, point.Y));
      }

      // Create a constant line (horizontal line at Y = 50)
      constantLineSeries.Title = "Constant Line";
      constantLineSeries.Color = OxyColors.Red;
      constantLineSeries.StrokeThickness = 1;
      constantLineSeries.LineStyle = LineStyle.Dash;
      constantLineSeries.RenderInLegend = false;


      constantLineSeries2.Title = "Constant Line 2";
      constantLineSeries2.Color = OxyColors.Green;
      constantLineSeries2.StrokeThickness = 1;
      constantLineSeries2.LineStyle = LineStyle.Dash;
      constantLineSeries2.RenderInLegend = false;
      

      // Add series to the plot model
      // Add two points to the constant line series
      constantLineSeries.Points.Add(new DataPoint(data.Min(p => p.X), min));
      constantLineSeries.Points.Add(new DataPoint(data.Max(p => p.X), min));

      constantLineSeries2.Points.Add(new DataPoint(data.Min(p => p.X), max));
      constantLineSeries2.Points.Add(new DataPoint(data.Max(p => p.X), max));

      //plotView.Series.Add(scatterSeries);
      //plotView.Series.Add(constantLineSeries);
      //plotView.Series.Add(constantLineSeries2);

      // Configure axes with limited decimal places
      plotView.Axes.Add(new LinearAxis
      {
        Position = AxisPosition.Bottom,
        Title = "X Axis",

        StringFormat = "0.###########"  // Format for up to 2 decimal places
      });

      plotView.Axes.Add(new LinearAxis
      {
        Position = AxisPosition.Left,
        Title = "Y Axis",
        StringFormat = "0.##"  // Format for up to 2 decimal places
      });

    }

    private static readonly Random _random = new Random();
    public static List<DataPoint> JitterCalculation(List<double> data)
    {
      var datapoint = new List<DataPoint>();
      // For demonstration: Generate a large number of points
      for (int x = 0; x < data.Count; x++)
      {
        //if (data[x] < -0.21 && data[x] > -0.44)
        //{

        var _randomNo = _random.NextDouble();

        var CX = 1 + ((_randomNo - 0.5) * 0.04);
        var CY = data[x];
        datapoint.Add(new DataPoint(CX, CY));
        //}
      }

      return datapoint;
    }
  }
}
