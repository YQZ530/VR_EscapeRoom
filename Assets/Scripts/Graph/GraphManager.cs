//using System.Collections.Generic;
using UnityEngine;
//using DCXR.Graph;
//using System.Data;
//using System.Linq;
//using XCharts.Runtime;
//using Sirenix.OdinInspector;

namespace DCXR.Graph
{
    public class GraphManager : MonoBehaviour
    {
    //    //Need Assignment
    //    public TabsHorizontalManager tabsManager;
    //    public string chartTitle = "Overal Performance";
    //    public string xAxisName = "Users";

    //    //link to UI document
    //    [OnCollectionChanged("OnFilterChange")]
    //    public List<GraphFilter> filters;
        
    //    public GraphFilter defaultFilter;
    //    public GraphType graphType;//link to UI document
    //    List<GroupFeature> featureList; //link to UI document


    //    //Get reference internally
    //    DiscreteDataLoader dataLoader;
    //    List<Material> graphMaterials;
    //    public void Start()
    //    {
    //        dataLoader = this.GetComponent<DiscreteDataLoader>();
    //        if (dataLoader == null) Debug.LogError("Did not assign dataLoader");

    //        //Initializatoin
    //        graphMaterials = new List<Material>();
    //        for (int i = 0; i < 10; i++)
    //        {
    //            graphMaterials.Add(Resources.Load("GraphMaterials/Line" + i.ToString(), typeof(Material)) as Material);
    //        }
    //        defaultFilter = new GraphFilter();
    //        filters = new List<GraphFilter>();
    //        filters.Add(defaultFilter);

    //        featureList = new List<GroupFeature>();

    //        //correspond to first UI
    //        featureList.Add(GroupFeature.GameplayDur);
    //    }
    //    public void OnGroupFeatureSelectionCallback(UnityEngine.UIElements.ToggleButtonGroupState state)
    //    {
    //        featureList = new List<GroupFeature>();
    //        featureList.Clear();
    //        for (int i = 0; i < (int)GroupFeature.All; i += 1)
    //        {
    //            if (state[i])
    //            {
    //                featureList.Add((GroupFeature)i);
    //                Debug.Log($"addd {(GroupFeature)i}");
    //            }
    //        }


    //        //if(featureType ==0) featureX = (GroupFeature)value; 
    //        //else featureY = (GroupFeature)value;
    //        //Debug.Log($"selecting {featureX} and {featureY}");
    //    }

    //    public void OnGraphTypeSelectionCallback(int value)
    //    {
    //        graphType = (GraphType)value;
    //        Debug.Log(graphType);
    //    }

    //    public void PlotGraph()
    //    {
    //        //1. update view for datatable
    //        OnFilterChange();

    //        //2. gather columns infor from the updated view
    //        List<DataColumn> columns = new List<DataColumn>();
    //        foreach (GroupFeature feature in featureList)
    //        {
    //            DataColumn columnX = dataLoader.GetDataView(feature);
    //            columns.Add(columnX);
    //        }

    //        //3. create a tab and add graph to that tab
    //        GameObject tab = tabsManager.AddTab(graphType.ToString());


    //        switch (graphType)
    //        {
    //            case GraphType.LineGraph:
    //                CreateLineChart(tab, columns);
    //                break;
    //            case GraphType.Bargraph:
    //                CreateBarChart(tab, columns);
    //                break;
    //            case GraphType.Boxplot:
    //                CreateBoxPlot(tab, columns);
    //                break;
    //            case GraphType.Histogram:
    //                CreateHistogram(tab, columns);
    //                break;
    //            case GraphType.Piechart:
    //                Debug.LogWarning("TODO");
    //                break;
    //        }

    //    }

    //    //public void AddGroupFilter(int groupID, int totNumCluster)
    //    //{
    //    //    if (groupID == totNumCluster)
    //    //    {
    //    //        filters.Add(new GraphFilter(GroupFeature.GroupID, Comparator.LessThan, (float)groupID));
    //    //    }
    //    //    else
    //    //    {
    //    //        filters.Add(new GraphFilter(GroupFeature.GroupID, Comparator.Equal, (float)groupID));
    //    //    }
    //    //}

    //   // [ShowInInspector]
    //    public void TestFilter()
    //    {
    //        //if(filters.Count == 0)
    //        // {
    //        //     filters.Add(new GraphFilter(GroupFeature.RatingA, Comparator.GreaterThan, 2.0f));
    //        //     filters.Add(new GraphFilter(GroupFeature.GameplayDur, Comparator.LessThan, 45f));
    //        //     OnFilterChange();
    //        // }


    //        // featureList = new List<GroupFeature>();
    //        // featureList.Add(GroupFeature.RatingA);
    //        // featureList.Add(GroupFeature.GameplayDur);
    //        // PlotGraph();
            
    //    }

       

    //    void OnFilterChange()
    //    {
    //        dataLoader.FilterTable(filters);
    //        Debug.Log("OnFilterChange()");
    //    }

    //    void CreateLineChart(GameObject tab, List<DataColumn> columns)
    //    {
    //        var chart = tab.transform.GetChild(0).gameObject.AddComponent<LineChart>();
    //        chart.Init();

    //        chart.RemoveData();

    //        int serieIndex = 0;
    //        List<double> xColumnData = dataLoader.ConvertColumnToList<double>("UserID");
    //        foreach (var column in columns)
    //        {
    //            Serie serie = chart.AddSerie<Line>(column.ColumnName);

    //            List<double> yColumnData = dataLoader.ConvertColumnToList<double>(column.ColumnName);
    //            GraphSetting.AddDataToChart(chart, serieIndex, xColumnData, yColumnData);
    //            serieIndex++;
    //        }

    //        GraphSetting.SetLineGraph(chart, chartTitle, xAxisName);
    //        Debug.Log("CreateLineChart");
    //    }

    //    void CreateBarChart(GameObject tab, List<DataColumn> columns)
    //    {
    //        var chart = tab.transform.GetChild(0).gameObject.AddComponent<BarChart>();
    //        chart.Init();

    //        //clear default data
    //        chart.RemoveData();

    //        int serieIndex = 0;
    //        List<double> xColumnData = dataLoader.ConvertColumnToList<double>("UserID");

    //        foreach (var column in columns)
    //        {
    //            Serie serie = chart.AddSerie<Bar>(column.ColumnName);
    //            //serie.colorBy = SerieColorBy.Data;
    //            //print("construction for"+column.ColumnName);
    //            List<double> yColumnData = dataLoader.ConvertColumnToList<double>(column.ColumnName);
    //            GraphSetting.AddDataToChart(chart, serieIndex, xColumnData, yColumnData);
    //            serieIndex++;
    //        }

    //        GraphSetting.SetBarGraph(chart, chartTitle, xAxisName, xColumnData);

    //    }



    //    void CreateBoxPlot(GameObject tab, List<DataColumn> columns)
    //    {
    //        var chart = tab.transform.GetChild(0).gameObject.AddComponent<CandlestickChart>();
    //        chart.Init();
    //        chart.SetSize(1800, 900);

    //        //not removing default series, just empty the data
    //        chart.ClearData();

    //        for (int serieIndex = 0; serieIndex < columns.Count; serieIndex++)
    //        {
    //            var column = columns[serieIndex];
    //            //Serie serie = chart.AddSerie<Candlestick>(column.ColumnName);
    //            chart.AddXAxisData(column.ColumnName);
    //        }

    //        for (int serieIndex = 0; serieIndex < columns.Count; serieIndex++)
    //        {
    //            var column = columns[serieIndex];
    //            List<double> yColumnData = dataLoader.ConvertColumnToList<double>(column.ColumnName);
    //            List<double> quartiles = GetQuartiles(yColumnData);

    //            var serieData = chart.AddData(0, serieIndex, quartiles[0], quartiles[2], yColumnData.Min(), yColumnData.Max());

    //            // serieData.itemStyle.color = GraphSetting.GetRandomColor();
    //        }

    //    }

    //    public int numBin = 5;
    //    void CreateHistogram(GameObject tab, List<DataColumn> columns)
    //    {
    //        var chart = tab.transform.GetChild(0).gameObject.AddComponent<BarChart>();
    //        chart.Init();

    //        //clear default data
    //        chart.RemoveData();

    //        for (int serieIndex = 0; serieIndex < columns.Count; serieIndex++)
    //        {
    //            var column = columns[serieIndex];
    //            Serie serie = chart.AddSerie<Bar>(column.ColumnName);
    //            // serie.barWidth = 1;
    //            List<double> yColumnData = dataLoader.ConvertColumnToList<double>(column.ColumnName);

    //            int binWidth = 0;
    //            Dictionary<double, int> histo = GraphSetting.CreateHistogram(yColumnData, numBin, out binWidth);
    //            int bidx = 0;
    //            foreach (KeyValuePair<double, int> kvp in histo)
    //            {

    //                string xaxisname = $"[{kvp.Key},{kvp.Key + binWidth}]";
    //                chart.AddXAxisData(xaxisname);
    //                chart.AddData(serieIndex, bidx, kvp.Value);
    //                bidx++;
    //            }
    //        }

    //        GraphSetting.SetHistogram(chart);
    //    }



    //    static List<double> GetQuartiles(List<double> values)
    //    {
    //        values.Sort();
    //        double q1 = GetPercentile(values, 25);
    //        double q2 = GetPercentile(values, 50);
    //        double q3 = GetPercentile(values, 75);
    //        return new List<double>() { q1, q2, q3 };
    //    }

    //    // Helper method to calculate percentiles
    //    static double GetPercentile(List<double> values, double percentile)
    //    {
    //        if (values.Count == 0)
    //            return 0;
    //        double position = (values.Count - 1) * percentile / 100.0;
    //        int lowerIndex = (int)System.Math.Floor(position);
    //        int upperIndex = (int)System.Math.Ceiling(position);
    //        if (lowerIndex == upperIndex)
    //            return values[lowerIndex];
    //        double lowerValue = values[lowerIndex];
    //        double upperValue = values[upperIndex];
    //        return lowerValue + (upperValue - lowerValue) * (position - lowerIndex);
    //    }
    }
}