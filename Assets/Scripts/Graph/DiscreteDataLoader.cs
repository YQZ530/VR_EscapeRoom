
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Data.Analysis;

using System.Data;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using DCXR.Replayer;

namespace DCXR.Graph
{
    public class DiscreteDataLoader : MonoBehaviour
    {
        
        DataTable dt;
        DataTable viewDt; //for viewing after filter
        string poseFileName = "";


        void Start()
        {

             poseFileName = Application.dataPath+"/../../EscapeRoomData/FinalEyeGazeGroup.csv";
            //
             DataFrame dataframe = DataFrame.LoadCsv(poseFileName, separator: ',', header: true);
            
            dt = new DataTable();
            
            InitColumn(dt);
            foreach (DataFrameRow row in dataframe.Rows)
            {
                DataRow dataRow = dt.NewRow();
                dataRow["UserID"] = row[0];
                dataRow["GroupID"] = row[1];
                dt.Rows.Add(dataRow);
            }
            viewDt = dt;
           
            Debug.Log($"DiscreteLoader:: Load gaze group, has row count = {viewDt.Rows.Count} ");  
        }

        private void OnEnable()
        {
          //  PythonEditor.onClusterDone += UpdateDataTable_Poses;
            GameLoader.DoneLoadingGazeDiscreteEvent += UpdateDataTable_Gazes;
        }

        private void OnDisable()
        {
          //  PythonEditor.onClusterDone -= UpdateDataTable_Poses;
            GameLoader.DoneLoadingGazeDiscreteEvent -= UpdateDataTable_Gazes;
        }

        public static void UpdateDataTableFromDataFrame(ref DataTable original, DataFrame temp, string columnName)
        {
            // // Assuming both the DataTable and DataFrame have "id" and "groupid" columns
            //
            // Iterate over DataFrame rows
            for (int i = 0; i < temp.Rows.Count; i++)
            {
                string id = temp["userID"][i].ToString();
                // Find the matching row in original database by uid
                var matchingRow = original.AsEnumerable().FirstOrDefault(row => row["userID"].ToString() == id);

                if (matchingRow != null)
                {
                    // Update the groupid in the DataTable
                    double value = -1;
                    double.TryParse(temp[columnName][i].ToString(), out value);

                    matchingRow[columnName] = value;

                }
                else
                {
                    Debug.LogError($"Cannot find user ID{id} in original table");
                }
            }
        }
        public void UpdateDataTable_Gazes()
        {
            string gazeFileName = Application.dataPath+"/../../EscapeRoomData/FinalEyeGazeGroup.csv";
            DataFrame tempDF = DataFrame.LoadCsv(gazeFileName, separator: ',', header: true);

            List<string> columns = new List<string>()
             {
                "GroupID",
             };
            foreach (string columnName in columns)
            {
                UpdateDataTableFromDataFrame(ref dt, tempDF, columnName);
            }

        }

       

        public DataColumn GetDataView(GroupFeature f)
        {
            if(viewDt == null)
            {
                Debug.LogWarning("after filter view dt is null");
                return null;
            }
            return viewDt.Columns[f.ToString()];
        }

        public int GetColInfo(int userID, string colName = "GroupID")
        {
            DataRow foundRow = dt.AsEnumerable()
                .FirstOrDefault(row => Convert.ToInt32(row["userID"]) == userID);

            // If a row with userID = 1 is found, retrieve its groupID
            if (foundRow != null)
            {
                return Convert.ToInt32(foundRow[colName]);
            }
            else
            {
                Debug.LogError($"No row with userID = {userID} found.");
                return -1;
            }
        }

        [ShowInInspector]
        public List<int> GetUserList(int groupID)
        {
            List<int> userList = dt.AsEnumerable()
                .Where(row =>  (  (int)System.Convert.ChangeType(row["GroupID"], typeof(int) ) == groupID))
                .Select(row => (int)System.Convert.ChangeType(row["userID"], typeof(int))).ToList();
            //.Select(row => Convert.ToInt32(row.Field<double>("userID")); 
            return userList;
        }

        //feature > value, 
        public void FilterTable(List< GraphFilter> filterList)
        {
           
            viewDt = dt;
            if (filterList == null || filterList.Count < 1)
            {
                return;
            }
            foreach (var filter in filterList)
            {
               viewDt = FilterByComparator(viewDt, filter);
            }

            return;
        }

        static DataTable FilterByComparator(DataTable dt, GraphFilter filter)
        {
            string featureName = filter.feature.ToString();
            Comparator c = filter.comparator;
            float value = filter.value;

            EnumerableRowCollection<DataRow> rows = null;
            switch (c)
            {
                case Comparator.LessThan:
                    rows = dt.AsEnumerable().Where(row => row.Field<double>(featureName) < value);
                    break;
                case Comparator.LessOrEqual:
                    rows = dt.AsEnumerable().Where(row => row.Field<double>(featureName) <= value);
                    break;
                case Comparator.Equal:
                    rows = dt.AsEnumerable().Where(row => row.Field<double>(featureName) == value);
                    break;
                case Comparator.GreaterThan:
                    rows = dt.AsEnumerable().Where(row => row.Field<double>(featureName) > value);
                    break;
                case Comparator.GreaterThanOrEqual:
                    rows = dt.AsEnumerable().Where(row => row.Field<double>(featureName) >= value);
                    break;
                default:
                    Debug.Log("undefined comparator" + c);
                    break;
            }

            if (rows == null || rows.Count() == 0 ) return null;
            
            return rows.CopyToDataTable();

           
        }

        public List<T> ConvertColumnToList<T>(string columnName)
        {
            //DataColumn column = dt.Columns[columnName];
            return viewDt.AsEnumerable().Select(row => (T)System.Convert.ChangeType(row[columnName], typeof(T))).ToList();
        }



        static void  InitColumn(DataTable dt)
        {
            
            DataColumn column = new DataColumn();
            column.DataType = System.Type.GetType("System.Double");
            column.ColumnName = "UserID";
            column.ReadOnly = true;
            column.Unique = true;
            dt.Columns.Add(column);


            DataColumn groupIDCol = new DataColumn();
            groupIDCol.DataType = System.Type.GetType("System.Double");
            groupIDCol.ColumnName = "GroupID";
            groupIDCol.ReadOnly = false;
            groupIDCol.Unique = false;
            dt.Columns.Add(groupIDCol);

        }

    }
}
