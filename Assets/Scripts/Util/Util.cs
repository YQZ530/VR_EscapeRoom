
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing.Printing;
using System.Text.RegularExpressions;
using System.Linq;
namespace DCXR.Util
{



    public static class TransformExtensions
    {
        public static GameObject FindParentWithComponent<T>(this Transform child) where T : Component
        {
            Transform current = child;

            while (current != null)
            {
                T component = current.GetComponent<T>();
                if (component != null)
                    return current.gameObject;

                current = current.parent;
            }

            return null; // No parent with the specified component found
        }
    }

    public class HelperMethod
    {
        public static int ExtraValueFromString(string input, string pattern)
        {
            int value = 0;
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                value = int.Parse(match.Value); // Parse the matched digits into an integer
                Console.WriteLine(value); // Output: 1

            }
            else
            {
                Console.WriteLine("No digits found in the string.");
            }
            return value;
        }

        public static List<Vector3> stringArrToVectorList(List<string> arr)
        {
            List<Vector3> outputArr = new List<Vector3>();
            arr.ForEach(elem => outputArr.Add(stringToVector(elem)));
            return outputArr;
        }
        public static Vector3 stringToVector(string str)
        {
            string[] splt = str.Split('[', ']', ',');
            return new Vector3(float.Parse(splt[1]), float.Parse(splt[2]), float.Parse(splt[3]));
        }
        public static List<int> stringToIntList(string str)
        {
            string[] splt = str.Split('[', ']', ',');
            List<int> list = new List<int>();
            for (int i = 0; i < splt.Length; i++)
            {
                if (splt[i] == "") continue;
                list.Add(int.Parse(splt[i]));
            }
            return list;
        }

        public static List<string> stringToStrList(string str)
        {
            string[] splt = str.Split('[', ']', ',');
            List<string> list = new List<string>();
            
            for (int i = 0; i < splt.Length; i++)
            {
                if (splt[i] == "") continue;
                list.Add(splt[i]);
                //Debug.Log(splt[i]);
            }
            
            return list;
        }
        public static Quaternion stringToQuaternion(string str)
        {
            string[] splt = str.Split('[', ']', ',');
            return new Quaternion(float.Parse(splt[1]), float.Parse(splt[2]), float.Parse(splt[3]), float.Parse(splt[4]));
        }

        public static List<int> stringArrToIntList(List<string> strarr)
        {
            //
            //List<int> arr = strarr.Select(s => string.IsNullOrEmpty(s) ? -1 : int.Parse(s)).ToList();

            List<int> arr = new List<int>();
            foreach (string str in strarr)
            {
                if (string.IsNullOrEmpty(str)) continue;
                //Debug.Log(str);

                arr.Add(int.Parse(str));

                //List<int> splt = str.Split( ',').Select(int.Parse)
                //                   .ToList();
            }


            return arr;
        }

        public static int stringToInt(string s, int defaultv = -1)
        {
            int v = defaultv;
            if (int.TryParse(s, out v))
            {
                return v;
            }
            else
            {
                Debug.LogWarning($"Fail to parse string {s}");
                return defaultv;
            }
        }

        public static float stringTofloat(string s, float defaultv = -1)
        {
            float v = defaultv;
            if (float.TryParse(s, out v))
            {
                return v;
            }
            else
            {
                Debug.LogWarning($"Fail to parse string {s}");
                return defaultv;
            }
        }

        public static int GetIntFromString(string s)
        {
            Match match = Regex.Match(s, @"\d+");
            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            else
            {
                return -1;
            }
        }

        public static string QuaterionToString(Quaternion v)
        {
            string s = "";
            if (v == null) return s;

            s = $"\"[{v.x},{v.y},{v.z},{v.w}]\"";
            return s;
        }

        public static string Vector3ToString(Vector3 v)
        {
            string s = "";
            if (v == null) return s;

            s = $"\"[{v.x},{v.y},{v.z}]\"";
            return s;
        }
        public static string ListToString<T>(List<T> arr)
        {
            string s = "\"[]\"";
            if (arr == null || arr.Count == 0) return s;
            s = "\"";
            for (int i = 0; i < arr.Count; i++)
            {
                if (i == 0) s += "[";
                if (i < arr.Count - 1) s += arr[i].ToString() + ",";
                else s += arr[i].ToString() + "]\"";
            }
            return s;
        }





        public static void FindParentObjNodeInGame(string areaName, string parentName, in List<Area> AreaList,
                                 out GameObject parentObj)
        {
           
            parentObj = null;

            Area area = AreaList.Find(elem => elem.gameObject.name == areaName);
            if (area == null)
            {
                Debug.LogError("Failed to find " + areaName);
                return;
            }

            string parentNode = parentName + "Node";
            Transform pt =  area.transform.Find(parentNode);
            if (pt == null)
            {
                Debug.LogError("Failed to find : " + areaName + " of  " + parentNode);
                return;
            }
            parentObj = pt.gameObject;

          
        }
        
        
        public static List<Vector3> FindGazeObjInGame(string areaName, string parentName, 
            List<string> gazeObjNamesList,  in List<Area> AreaList)
        {
           
            GameObject parentObj = null;

            Area area = AreaList.Find(elem => elem.gameObject.name == areaName);
            if (area == null)
            {
                Debug.LogError("Failed to find " + areaName);
                return null;
            }

            Transform pt =  area.transform.Find(parentName);
            if (pt == null)
            {
                Debug.LogError("Failed to find : " + areaName + " of  " + parentName);
                return null;
            }
            parentObj = pt.gameObject;

            List<Vector3> objPos = new List<Vector3>();
            foreach (string objname in gazeObjNamesList)
            {
                Transform obj = parentObj.transform.Find(objname);
                if (obj == null)
                {
                    Debug.LogError($"Failed to find: {objname} in area: {areaName} with parent: {parentName}");
                    continue;
                }
                objPos.Add(obj.transform.position);
            }

            return objPos;


        }


        //public static ChunkType TryParseChunktype(string chunkName)
        //{
        //    ChunkType chunkType;
        //    if (!Enum.TryParse<ChunkType>(chunkName, out chunkType))
        //    {
        //        Debug.LogWarning("Failed to convert string to enum. " + chunkName);
        //        return ChunkType.Tot;
        //    }
        //    return chunkType;

        //}



    }
}