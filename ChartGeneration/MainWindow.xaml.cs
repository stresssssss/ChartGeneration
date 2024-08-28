using LiveCharts;
using LiveCharts.Wpf;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using OfficeOpenXml;
using System.Windows.Media;
using System.IO;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ChartGeneration
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public ChartValues<double> ChartValues { get; set; }
    public VariabilityViewModel m_variabilityViewModel = new VariabilityViewModel();
    public event PropertyChangedEventHandler PropertyChanged;
    public List<double> dataList { get; set; }
    private Func<double, string> _xFormatter;
    public Func<double, string> XFormatter
    {
      get { return _xFormatter; }
      set
      {
        _xFormatter = value;
        OnPropertyChanged(nameof(XFormatter));
      }
    }
    private Func<double, string> _yFormatter;
    public Func<double, string> YFormatter
    {
      get { return _yFormatter; }
      set
      {
        _yFormatter = value;
        OnPropertyChanged(nameof(_yFormatter));
      }
    }
    public static List<List<Criteria>> WaferList { get; set; }
    private int _selectedindex;

    private ObservableCollection<string> _comboboxitem;
    public ObservableCollection<string> ComboBoxItems 
    {
      get { return _comboboxitem; }
      set
      {
        _comboboxitem = value;
        OnPropertyChanged(nameof(ComboBoxItems));
      }
    }


    public int SelectedIndex
    {
      get 
      {
        return _selectedindex;
          }
      set
      {
        _selectedindex = value;
        OnPropertyChanged(nameof(SelectedIndex));
      }
      
    }

    private int _selectedindexratio;
    public ObservableCollection<string> RatioList { get; set; }

    public int SelectedIndexRatio
    {
      get
      {
        return _selectedindexratio;
      }
      set
      {
        _selectedindexratio = value;
        OnPropertyChanged(nameof(SelectedIndexRatio));
      }
    }

    private int _selectedindexwafer;
    public ObservableCollection<string> WaferTypeList { get; set; }

    public int SelectedIndexWafer
    {
      get
      {
        return _selectedindexwafer;
      }
      set
      {
        _selectedindexwafer = value;
        OnPropertyChanged(nameof(SelectedIndexWafer));
      }
    }


    public MainWindow()
    {
      InitializeComponent();
      Instantiate();
      ReadCSV(14);
      //ReadExcel();
      DataContext = this; // Set the DataContext for data binding

      // Handle the Loaded event to ensure layout is complete
      this.SizeChanged += MainWindow_SizeChanged;
      this.Loaded += MainWindow_Loaded;
    }
   
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      LoadHistogramChart();
      ScatterGraphInstantiation();
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      LoadHistogramChart();
      ScatterGraphInstantiation();
    }

    private void Instantiate()
    {
      WaferList = new List<List<Criteria>>();

      #region Wafer Type 1
      var ItemList = new List<Criteria>();
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "IL_ALL_MAG_C1_S21",
          CriteriaColIndex = 14,
        });
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "AT_HI_C1_S21_6.5_G",
          CriteriaColIndex = 48,
        });
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "RL11_MID_MAG_C1_S11",
          CriteriaColIndex = 44,
        });
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "RL22_MID_MAG_C1_S22",
          CriteriaColIndex = 46,
        });
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "AT_LOW_C1_S21_4.5_G",
          CriteriaColIndex = 47,
        });

      WaferList.Add(ItemList);
      #endregion

      #region Wafer Type 1
      ItemList = new List<Criteria>();
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "IL_ALL_MAG_C1_S21",
          CriteriaColIndex = 14,
        });
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "FSL_24_C1_S21_4.5_G_5.5_G",
          CriteriaColIndex = 25,
        });
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "AT_RIGHT_MAG_C1_S21",
          CriteriaColIndex = 51,
        });
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "AT_LEFT_MAG_C1_S21",
          CriteriaColIndex = 49,
        });
      ItemList.Add(
        new Criteria
        {
          CriteriaName = "RL22_OOB_RIGHT_MAG_C1_S22",
          CriteriaColIndex = 64,
        });

      WaferList.Add(ItemList);
      #endregion


      #region Instantiate UI Variable
      WaferTypeList = new ObservableCollection<string>();

      for (int x = 0; x < WaferList.Count; x++)
      {
        WaferTypeList.Add($"Wafer Type: {x}");
      }

      ComboBoxItems = new ObservableCollection<string>();

      for (int x = 0; x < WaferList[SelectedIndexWafer].Count; x++)
      {
        ComboBoxItems.Add(WaferList[SelectedIndexWafer][x].CriteriaName);
      }

      RatioList = new ObservableCollection<string>();
      RatioList.Add("0.02");
      RatioList.Add("0.05");
      RatioList.Add("0.10");
      RatioList.Add("0.15");
      RatioList.Add("0.20");
      RatioList.Add("0.25");
      RatioList.Add("0.3");
      RatioList.Add("0.35");
      RatioList.Add("0.4");
      #endregion

    }

    private void ReadExcel(int c_criteria)
    {
      int R_Criteria = 45;

      string filePath = @"D:\2RR3-7350_4988-P014A_PR-EG4090-09_FULL-1PASS_20240728_051641_IP192.168.0.2_O7S5AQL.CSV";

      // List to store the Data objects
      dataList = new List<double>();

      // Load the Excel file
      ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
      FileInfo fileInfo = new FileInfo(filePath);
      using (ExcelPackage package = new ExcelPackage(fileInfo))
      {
        // Get the first worksheet
        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

        // Read the first column
        int row = 1;
        while (worksheet.Cells[c_criteria, R_Criteria].Value != null)
        {
          // Try to parse the value as a double
          if (double.TryParse(worksheet.Cells[c_criteria, 1].Text, out double value))
          {
            // Add the value to the list as a Data object
            dataList.Add(value);
          }
          else
          {
            Console.WriteLine($"Invalid number at row {c_criteria}");
          }
          c_criteria++;
        }

        data = new List<double>();
        data = dataList.ToList();
      }
    }

    //private void ReadCSV(int c_criteria)
    //{
    //  int R_CriteriaMax = 48;
    //  int R_CriteriaMin = 49;
    //  int R_Criteria = 50;
    //  string filePath = @"D:\2VS9-9069VA_4632-F043A_PR-UF2000-26_FULL-1PASS_20240819_052347_IP192.168.24.49_O8J5AXO.CSV";

    //  // List to store the Data objects
    //  dataList = new List<double>();

    //  var lines = File.ReadAllLines(filePath);

    //  for (int i = R_Criteria; i < lines.Length; i++)
    //  {
    //    var line = lines[i];
    //    var strvalues = line.Split(',');

    //    // Ensure the start column index is within bounds
    //    if (c_criteria < strvalues.Length)
    //    {
    //      var doublevalue = Convert.ToDouble(strvalues[c_criteria]);
    //      dataList.Add(doublevalue);
    //    }
    //  }


    //  #region Finding the max/min 
    //  var _MaxLine = lines[R_CriteriaMax];
    //  var _Maxvalue = _MaxLine.Split(',');

    //  WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMax = Convert.ToDouble(_Maxvalue[c_criteria]);

    //  var _MinLine = lines[R_CriteriaMin];
    //  var _Minvalue = _MinLine.Split(',');

    //  WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMin = Convert.ToDouble(_Minvalue[c_criteria]);

    //  data = new List<double>();
    //  data = dataList.ToList();
    //  #endregion

    //}

    private void ReadCSV(int c_criteria)
    {
      int R_CriteriaMax = 50;
      int R_CriteriaMin = 51;
      int R_Criteria = 52;
      string filePath = @"D:\2VS9-9069VA_4632-F043A_PR-UF2000-26_FULL-1PASS_20240819_052347_IP192.168.24.49_O8J5AXO.CSV";

      // List to store the Data objects
      dataList = new List<double>();

      var lines = File.ReadAllLines(filePath);

      for (int i = R_Criteria; i < lines.Length; i++)
      {
        var line = lines[i];
        var strvalues = line.Split(',');

        // Ensure the start column index is within bounds
        if (c_criteria < strvalues.Length)
        {
          var doublevalue = Convert.ToDouble(strvalues[c_criteria]);
          dataList.Add(doublevalue);
        }
      }


      #region Finding the max/min 
      var _MaxLine = lines[R_CriteriaMax];
      var _Maxvalue = _MaxLine.Split(',');

      WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMax = Convert.ToDouble(_Maxvalue[c_criteria]);
      HistogramViewModel.LowerBound = WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMax;
      var _MinLine = lines[R_CriteriaMin];
      var _Minvalue = _MinLine.Split(',');

      WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMin = Convert.ToDouble(_Minvalue[c_criteria]);
      HistogramViewModel.UpperBound = WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMin;
      data = new List<double>();
      data = dataList.ToList();
      #endregion

    }

    private SeriesCollection _seriesCollection;
    public SeriesCollection SeriesCollection
    {
        get { return _seriesCollection; }
        set
        {
            _seriesCollection = value;
        OnPropertyChanged(nameof(SeriesCollection));
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
      if(propertyName == "SelectedIndex")
      {
        ReadCSV(WaferList[SelectedIndexWafer][SelectedIndex].CriteriaColIndex);
        LoadHistogramChart();
        ScatterGraphInstantiation();
      }
      if (propertyName == "SelectedIndexRatio")
      {
        HistogramViewModel.DynamicLimitRatio = Convert.ToDouble(RatioList[SelectedIndexRatio]);
        ReadCSV(WaferList[SelectedIndexWafer][SelectedIndex].CriteriaColIndex);
        LoadHistogramChart();
        ScatterGraphInstantiation();
      }

      if(propertyName == "SelectedIndexWafer")
      {
        Instantiate();
        HistogramViewModel.DynamicLimitRatio = Convert.ToDouble(RatioList[SelectedIndexRatio]);
        ReadCSV(WaferList[SelectedIndexWafer][SelectedIndex].CriteriaColIndex);
        LoadHistogramChart();
        ScatterGraphInstantiation();
      }
      
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private void LoadHistogramChart()
    {
      // Convert data to ChartValues
      ChartValues = new ChartValues<double>();
      var DuplicateList = dataList.ToList();
      var Median = HistogramViewModel.FindMedian(DuplicateList);
      var ChartData = HistogramViewModel.FindXSpectrum(DuplicateList, Median);
      var SpectrumX = new List<double>();
      SpectrumX = ChartData.Select(n => n.VariableValue).ToList();
      //foreach (var item in SpectrumX)
      //{
      //  ChartValues.Add(item.VariableQty);
      //}

      ChartValues = new ChartValues<double>(ChartData.Select(d => d.VariableQty));

      SeriesCollection = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Histogram Data",
                Values = ChartValues
            }
        };

      XFormatter = value =>
      {
        int index = (int)value; // Convert the double value to an integer index
        return SpectrumX[index].ToString();
      };

      //DataContext = this;
      // Set up formatters (if needed)
      //XFormatter = value => value.ToString(CultureInfo.InvariantCulture);
      //YFormatter = value => value.ToString(CultureInfo.InvariantCulture);
    }


    public static List<double> data { get; set; }

    private void ScatterGraphInstantiation()
    {
      var m_PlotView = new PlotModel();
      var mdata = VariabilityViewModel.JitterCalculation(data);

      var scatterSeries = new OxyPlot.Series.ScatterSeries
      {
        Title = "Data Points",
        MarkerType = MarkerType.Circle,
        MarkerSize = 1,
        MarkerFill = OxyColors.Blue
      };

      var constantLineSeries = new OxyPlot.Series.LineSeries
      {
        Title = "Constant Line",
        Color = OxyColors.Red,
        StrokeThickness = 1,
        LineStyle = LineStyle.Dash,
        RenderInLegend = false
      };

      var constantLineSeries2 = new OxyPlot.Series.LineSeries
      {
        Title = "Constant Line 2",
        Color = OxyColors.Green,
        StrokeThickness = 1,
        LineStyle = LineStyle.Dash,
        RenderInLegend = false
      };

      constantLineSeries.Points.Add(new DataPoint(mdata.Min(p => p.X), WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMin));
      constantLineSeries.Points.Add(new DataPoint(mdata.Max(p => p.X), WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMin));

      constantLineSeries2.Points.Add(new DataPoint(mdata.Min(p => p.X), WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMax));
      constantLineSeries2.Points.Add(new DataPoint(mdata.Max(p => p.X), WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMax));

      m_PlotView.Series.Add(scatterSeries);
      m_PlotView.Series.Add(constantLineSeries);
      m_PlotView.Series.Add(constantLineSeries2);

      // Configure axes with limited decimal places
      m_PlotView.Axes.Add(new LinearAxis
      {
        Position = OxyPlot.Axes.AxisPosition.Bottom,
        Title = "X Axis",

        StringFormat = "0.###########"  // Format for up to 2 decimal places
      });

      m_PlotView.Axes.Add(new LinearAxis
      {
        Position = OxyPlot.Axes.AxisPosition.Left,
        Title = "Y Axis",
        StringFormat = "0.##"  // Format for up to 2 decimal places
      });

      m_variabilityViewModel.ScatterPoint(m_PlotView,
        scatterSeries,
        constantLineSeries,
        constantLineSeries2,
        data,
        WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMin,
        WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMax);

      plotView.Model = m_PlotView;

    }

    //private void ScatterPoint()
    //{
    //  // Create a new PlotModel
    //  var plotModel = new PlotModel { Title = "Scatter Plot with Constant Line" };

    //  // Example large dataset: Generating a large number of points
    //  var data = JitterCalculation();

    //  // Create a ScatterSeries and add data points
    //  var scatterSeries = new OxyPlot.Series.ScatterSeries
    //  {
    //    Title = "Data Points",
    //    MarkerType = MarkerType.Circle,
    //    MarkerSize = 1,
    //    MarkerFill = OxyColors.Blue
    //  };

    //  // Adding data points to the series
    //  foreach (var point in data)
    //  {
    //    scatterSeries.Points.Add(new ScatterPoint(point.X, point.Y));
    //  }

    //  // Create a constant line (horizontal line at Y = 50)
    //  var constantLineSeries = new OxyPlot.Series.LineSeries
    //  {
    //    Title = "Constant Line",
    //    Color = OxyColors.Red,
    //    StrokeThickness = 1,
    //    LineStyle = LineStyle.Dash,
    //     RenderInLegend = false
    //  };

    //  var constantLineSeries2 = new OxyPlot.Series.LineSeries
    //  {
    //    Title = "Constant Line 2",
    //    Color = OxyColors.Green,
    //    StrokeThickness = 1,
    //    LineStyle = LineStyle.Dash,
    //    RenderInLegend = false
    //  };

    //  // Add series to the plot model
    //  // Add two points to the constant line series
    //  constantLineSeries.Points.Add(new DataPoint(data.Min(p => p.X), WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMin));
    //  constantLineSeries.Points.Add(new DataPoint(data.Max(p => p.X), WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMin));

    //  constantLineSeries2.Points.Add(new DataPoint(data.Min(p => p.X), WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMax));
    //  constantLineSeries2.Points.Add(new DataPoint(data.Max(p => p.X), WaferList[SelectedIndexWafer][SelectedIndex].CriteriaMax));

    //  plotModel.Series.Add(scatterSeries);
    //  plotModel.Series.Add(constantLineSeries);
    //  plotModel.Series.Add(constantLineSeries2);

    //  // Configure axes with limited decimal places
    //  plotModel.Axes.Add(new LinearAxis
    //  {
    //    Position = OxyPlot.Axes.AxisPosition.Bottom,
    //    Title = "X Axis",

    //    StringFormat = "0.###########"  // Format for up to 2 decimal places
    //  });

    //  plotModel.Axes.Add(new LinearAxis
    //  {
    //    Position = OxyPlot.Axes.AxisPosition.Left,
    //    Title = "Y Axis",
    //    StringFormat = "0.##"  // Format for up to 2 decimal places
    //  });


    //  // Set the PlotModel to the PlotView
    //  plotView.Model = plotModel;
    //}
    //private static readonly Random _random = new Random();
    //private List<DataPoint> JitterCalculation()
    //{
    //  var datapoint = new List<DataPoint>();
    //  // For demonstration: Generate a large number of points
    //  for (int x = 0; x < data.Count; x++)
    //  {
    //    //if (data[x] < -0.21 && data[x] > -0.44)
    //    //{

    //    var _randomNo = _random.NextDouble();

    //    var CX = 1 + ((_randomNo - 0.5) * 0.04);
    //    var CY = data[x];
    //    datapoint.Add(new DataPoint(CX, CY));
    //    //}
    //  }

    //  return datapoint;
    //}
  }
}