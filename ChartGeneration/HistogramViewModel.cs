using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartGeneration
{
  public class HistogramViewModel
  {
    public static double m_x1;
    public static double m_x2;
    public static double m_x3;
    public static double m_x4;
    public static double m_x5;
    public static double m_x6;

    public static double Average;
    public static double FindMedian(List<double> numbers)
    {
      // Sort the list
      var sortedNumbers = numbers.OrderBy(n => n).ToList();

      int count = sortedNumbers.Count;
      if (count == 0)
      {
        throw new InvalidOperationException("Cannot find the median of an empty list.");
      }

      // Calculate the median
      if (count % 2 == 0)
      {
        // Even number of elements: average the two middle values
        double mid1 = sortedNumbers[(count / 2) - 1];
        double mid2 = sortedNumbers[count / 2];
        return (mid1 + mid2) / 2.0;
      }
      else
      {
        // Odd number of elements: return the middle value
        return sortedNumbers[count / 2];
      }
    }

    public static double DynamicLimitRatio = 0.25;
    public static double UpperBound;
    public static double LowerBound;
    public static List<Data> FindXSpectrum(List<double> numbers, double median)
    {
      int AverageCount = 5;

      //UpperBound = median * (1 + DynamicLimitRatio);
      //LowerBound = median * (1 - DynamicLimitRatio);

      // Notes: UpperBound is the minimum value as majority is negative value, vice versa.
      if (UpperBound + LowerBound > 0)
      {
        #region Find the Positive Spectrum 
        Average = (LowerBound - UpperBound) / AverageCount;

        m_x1 = UpperBound;
        m_x2 = UpperBound + Average * 1;
        m_x3 = UpperBound + Average * 2;
        m_x4 = UpperBound + Average * 3;
        m_x5 = UpperBound + Average * 4;
        m_x6 = UpperBound + Average * 5;
        #endregion
      }
      else
      {
        #region Finding the Negative Spectrum
        Average = (UpperBound - LowerBound) / AverageCount;

        m_x1 = LowerBound;
        m_x2 = LowerBound + Average * 1;
        m_x3 = LowerBound + Average * 2;
        m_x4 = LowerBound + Average * 3;
        m_x5 = LowerBound + Average * 4;
        m_x6 = LowerBound + Average * 5;
        #endregion
      }


      var SortedData = numbers
         .GroupBy(n => GetRange(n))
         .OrderBy(g => g.Key)
         .Select(g => new Data { VariableValue = g.Key, VariableQty = g.Count() })
         .ToList();


      foreach (var group in SortedData)
      {
        Console.WriteLine($"Range {group.VariableValue}: {group.VariableQty} numbers, TotalCount : {TotalCount}");
      }

      return SortedData;
    }

    public static int TotalCount;
    static double GetRange(double value)
    {
      double scaledown = Average / 10;

      if (UpperBound + LowerBound > 0)
      {
        #region Positive Spectrum
        if (value >= m_x1 && value <= m_x2)
        {
          for (double x = m_x1; x <= m_x2; x += scaledown)
          {
            if (value >= m_x1 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }

        if (value >= m_x2 && value <= m_x3)
        {
          for (double x = m_x2; x <= m_x3; x += scaledown)
          {
            if (value >= m_x2 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }

        if (value >= m_x3 && value <= m_x4)
        {
          for (double x = m_x3; x <= m_x4; x += scaledown)
          {
            if (value >= m_x3 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }
        if (value >= m_x4 && value <= m_x5)
        {
          for (double x = m_x4; x <= m_x5; x += scaledown)
          {
            if (value >= m_x4 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }
        if (value >= m_x5 && value <= m_x6)
        {
          for (double x = m_x5; x <= m_x6; x += scaledown)
          {
            if (value >= m_x5 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }
        #endregion
      }

      else
      {
        #region Negative Spectrum
        if (value >= m_x2 && value <= m_x1)
        {
          for (double x = m_x2; x <= m_x1; x -= scaledown)
          {
            if (value >= m_x2 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }
        if (value >= m_x3 && value <= m_x2)
        {
          for (double x = m_x3; x <= m_x2; x -= scaledown)
          {
            if (value >= m_x3 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }

        if (value >= m_x4 && value <= m_x3)
        {
          for (double x = m_x4; x <= m_x3; x -= scaledown)
          {
            if (value >= m_x4 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }

        if (value >= m_x5 && value <= m_x4)
        {
          for (double x = m_x5; x <= m_x4; x -= scaledown)
          {
            if (value >= m_x5 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }
        if (value >= m_x6 && value <= m_x5)
        {
          for (double x = m_x6; x <= m_x5; x -= scaledown)
          {
            if (value >= m_x6 && value <= x)
            {
              TotalCount++;
              return x;
            }
          }
        }
        #endregion
      }

      return 0; // For numbers outside defined ranges
    }
  }
}
