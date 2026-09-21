//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using Unity.Mathematics;
//using XCharts.Runtime;

namespace DCXR.Graph
{
    public class GraphSetting
    {
    //    public static void SetBarGraph(BarChart chart, string chartTitle, 
    //        string xAxisName, List<double> xColumnData)
    //    {
    //        chart.SetSize(2000, 1000);

    //        var title = chart.EnsureChartComponent<Title>();
    //        title.text = chartTitle;

    //        var legend = chart.EnsureChartComponent<Legend>();
    //        legend.show = true;

    //        var yAxis = chart.EnsureChartComponent<YAxis>();
    //        yAxis.type = Axis.AxisType.Value;
    //        yAxis.offset = -81f;
    //        yAxis.SetComponentDirty();

    //        int xAixMax = xColumnData.Count;
    //        var xAxis = chart.EnsureChartComponent<XAxis>();
    //        xAxis.show = true;
    //        //xAxis.splitNumber = 10;
    //        xAxis.boundaryGap = true;
    //        xAxis.type = Axis.AxisType.Category;
    //        xAxis.interval = 1;

    //        xAxis.axisName.show = true;
    //        xAxis.axisName.name = xAxisName;
    //        xAxis.axisName.labelStyle.offset = new Vector3(0, -28.5f, 0f);
    //        xAxis.axisName.labelStyle.position = LabelStyle.Position.Middle;
    //        xAxis.axisName.labelStyle.textStyle.fontSize = 27;

    //        xAxis.minMaxType = Axis.AxisMinMaxType.Custom;
    //        xAxis.min = -1;
    //        xAxis.max = xAixMax;
    //        xAxis.SetComponentDirty();

    //        for (int i = 0; i < xColumnData.Count; i++)
    //        {
    //            chart.AddXAxisData(xColumnData[i].ToString());
    //        }

    //    }

    //    public static void SetLineGraph(LineChart chart, string chartTitle, string xAxisName)
    //    {
    //        chart.SetSize(2000, 1000);
    //        var title = chart.EnsureChartComponent<Title>();
    //        title.text = chartTitle;

    //        var legend = chart.EnsureChartComponent<Legend>();
    //        legend.show = true;

    //        var xAxis = chart.EnsureChartComponent<XAxis>();
    //        //xAxis.splitNumber = 10;
    //        xAxis.boundaryGap = true;
    //        xAxis.type = Axis.AxisType.Value;
    //        xAxis.axisName.name = xAxisName;
    //        xAxis.interval = 1;

    //        var yAxis = chart.EnsureChartComponent<YAxis>();
    //        yAxis.type = Axis.AxisType.Value;
    //    }

    //    public static void SetHistogram(BarChart chart)
    //    {
    //        var legend = chart.EnsureChartComponent<Legend>();
    //        legend.show = true;

    //        chart.SetSize(2000, 1000);
    //        var tooltip = chart.EnsureChartComponent<Tooltip>();
    //        tooltip.numericFormatter = "count=0";
    //        var xAxis = chart.EnsureChartComponent<XAxis>();
    //        xAxis.boundaryGap = true;
    //        xAxis.type = Axis.AxisType.Category;

    //        foreach(var serie in chart.series)
    //        {
    //            serie.barWidth = 1f;
    //        }
           
    //    }

    //    public static void AddDataToChart(BaseChart chart, int serieIndex, List<double> xDataList, List<double> yDataList)
    //    {
    //        for (int i = 0; i < yDataList.Count; i++)
    //        {
    //            chart.AddData(serieIndex, xDataList[i], yDataList[i]);
    //        }
    //    }


    //    public static Color32 GetRandomColor()
    //    {

    //        // Create a Random object
    //        System.Random random = new System.Random();

    //        // Generate random RGB values
    //        int r = random.Next(1); // 0 to 255
    //        int g = random.Next(1); // 0 to 255
    //        int b = random.Next(1); // 0 to 255


    //        return new Color(r, g, b);

    //    }


    //    public static Dictionary<double, int> CreateHistogram(List<double> data, int numBins, out int binWidth)
    //    {
    //        // Calculate the minimum and maximum values in the data array
    //        int minValue = Mathf.FloorToInt((float) data.Min());
    //        int maxValue = Mathf.CeilToInt((float)data.Max());

    //        // Calculate the bin width
    //        binWidth = Mathf.RoundToInt(  1.0f* (maxValue - minValue) / numBins);

    //        // Create a dictionary to store the histogram bins
    //        Dictionary<double, int> histogram = new Dictionary<double, int>();

    //        // Initialize the histogram bins with frequency zero
    //        for (int i = 0; i < numBins; i++)
    //        {
    //            int binStart = minValue + i * binWidth;
    //            int binEnd = binStart + binWidth;
    //            histogram[binStart] = 0;
    //        }

    //        // Populate the histogram bins with the frequency of values falling into each bin
    //        foreach (int value in data)
    //        {
    //            int binStart = minValue;
    //            for (int i = 0; i < numBins; i++) //for that value, find the correct bin and update the count
    //            {
    //                int binEnd = binStart + binWidth;
    //                if (value >= binStart && value < binEnd)
    //                {
    //                    histogram[binStart]++;
    //                    break;
    //                }
    //                binStart = binEnd;
    //            }
    //        }

    //        return histogram;

    //    }
    }
}



