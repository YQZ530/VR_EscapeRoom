using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using System.Linq;
using System;

namespace DCXR.PathDrawing
{
    public struct GazePathData
    {
        public Vector3 Position;
        public float objdur;
        public GameObject chunk;
        public GameObject chunkObject;

    }

    [Serializable]
    public class PathDrawer
    {
        public VectorLine linePath;
        protected List<Vector3> path_pos;
        private Color32 lineColor;
        public bool isInitialized = false;

        public Color32 defaultLineColor;
        protected float lineHeight = 4f;
        protected float default_alpha = 1.0f; // 0.7f;
        protected float lineWidth;
        public PathDrawer(string pathName, float lineWidth, List<Vector3> walkPath, 
            Material line_material, Texture lineTex, Transform parent)
        {
            this.isInitialized = true;
           
            AddPathHeight(ref walkPath, lineHeight);
            this.path_pos = walkPath;
            this.lineWidth = lineWidth;

            this.linePath = new VectorLine(pathName, new List<Vector3>(), lineTex, lineWidth, LineType.Continuous, parent);

            SetPathColor(line_material.color);
            this.defaultLineColor = line_material.color;
            this.defaultLineColor.a = (byte)(default_alpha * 255);

           
           // this.linePath.SetWidth(lineWidth);
            this.linePath.points3.AddRange(path_pos);
            this.linePath.Draw3D();

            VectorLine.SetCamera3D();
        }
        
        public void UpdateAndRedraw(List<Vector3> walkPath) {
            AddPathHeight(ref walkPath, lineHeight);
            this.path_pos = walkPath;
            Debug.LogWarning($"~~~~~~~~~~~~~~~~~~~~~~~before {this.linePath.points3.Count}");
            this.linePath.points3.Clear();
            this.linePath.points3.AddRange(path_pos);
        
            this.linePath.Draw3D();
            Debug.LogWarning($"~~~~~~~~~~~~~~~~~~~~~~~after {this.linePath.points3.Count} and taget count{walkPath.Count}");
            VectorLine.SetCamera3D();
        }

        public void SetDefaultPathColor()
        {
            SetPathColor(defaultLineColor);
        }
        public virtual void SetPathColor(Color32 lineColor)
        {
            this.lineColor = lineColor;
            this.lineColor.a = (byte)(default_alpha * 255);
            this.linePath.color = this.lineColor;

        }
        
        public void SetLineWidth(float lineWidth)
        {
            this.linePath.lineWidth = lineWidth;
            this.linePath.Draw3D();
           
            VectorLine.SetCamera3D();
            //Debug.Log("Redraw with lineWidth " + lineWidth);
        }
        public VectorLine GetLinePath()
        {
            return this.linePath;
        }

        public static void SetPointHeight(VectorLine vectorLineObj, float addHeight)
        {

            if (vectorLineObj == null || vectorLineObj.points3 == null) return;
            for (int i = 0; i < vectorLineObj.points3.Count; i++)
            {
                var v = vectorLineObj.points3[i];
                v.y = addHeight;
                vectorLineObj.points3[i] = v;
            }
            //Debug.Log($"SetPointHeight {addHeight}");
            vectorLineObj.Draw3D();
        }
        protected static void AddPathHeight(ref List<Vector3> path_pos, float addHeight = 2.0f)
        {
            for (int i = 0; i < path_pos.Count; i++)
            {
                var v = path_pos[i];
                v.y += addHeight;
            }
        }


        public void ShowHideLinePath(bool showPath)
        {
            if (linePath == null || linePath.points3 == null)
            {
                Debug.LogWarning("Path.points is not initialized");
                return;
            }

            if (showPath)
            {
               
                this.linePath.active = true;
            }
            else
            {
                this.linePath.active = false;
            }

        }

    }
    [Serializable]
    public class EyeGazeDrawer : PathDrawer
    {
        List<float> path_dur;
       
        VectorLine pointPath;
   
        float maxPointSize;
        float minPointSize;

        
        public EyeGazeDrawer(string pathName, float lineWidth, List<Vector3> walkPath, List<Vector3> pointPath,List<float> path_dur,
            Material line_material, Texture lineTex, Texture eyeGazePointTex, float maxPointSize,
            float minPointSize, Texture2D[] eyegaze_capTextures, Transform parent) :
            base(pathName, lineWidth, walkPath, line_material, lineTex, parent)
        {
            //assignment
            this.maxPointSize = maxPointSize;
            this.minPointSize = minPointSize;
            this.path_dur = path_dur;

            //Base class has already initialized pathline obj
            VectorLine.SetEndCap("Arrow2", EndCap.Front, 0f, 0f, eyegaze_capTextures);
            this.linePath.endCap = "Arrow2";

            //point path initilization
            
            this.pointPath = new VectorLine(pathName + "_Points", pointPath, eyeGazePointTex, lineWidth, LineType.Points, parent);
           
            SetPathColor(line_material.color);
            SetPointSizeByDur();
            SetPointHeight(this.pointPath, 1.2f);
            this.pointPath.Draw3D();

            VectorLine.SetCamera3D();
        }
      
      
        public VectorLine GetPointPathObj()
        {
            return this.pointPath;
        }

        public override void SetPathColor(Color32 lineColor)
        {
            if (this.pointPath == null || this.linePath == null) return;
            this.pointPath.SetColor(lineColor);
            this.linePath.SetColor(lineColor);
        }


        void SetPointSizeByDur()
        {
            List<float> pointSizes = new List<float>();
            float maxDur = path_dur.Max();
            float minDur = path_dur.Min();
            for (int i = 0; i < this.pointPath.points3.Count; i++)
            {
                float dur = path_dur[i];
                //just adjust point width based on duration
                float r = (dur - minDur) / maxDur;
                float normalizedValue = minPointSize + (r * (maxPointSize - minPointSize));
                pointSizes.Add(normalizedValue);
            }
           // Debug.Log( "normmalize point size "+ Util.HelperMethod.ListToString(pointSizes));
            this.pointPath.SetWidths(pointSizes);
        }

        void SetPointSizeByDur(float minDur, float maxDur)
        {
            List<float> pointSizes = new List<float>();
          
            for (int i = 0; i < this.pointPath.points3.Count; i++)
            {
                float dur = path_dur[i];
                //just adjust point width based on duration
                float r = (dur - minDur) / maxDur;
                float normalizedValue = minPointSize + (r * (maxPointSize - minPointSize));
                pointSizes.Add(normalizedValue);
            }
            //Debug.Log("normmalize point size " + Util.HelperMethod.ListToString(pointSizes));
            this.pointPath.SetWidths(pointSizes);
        }

        public void SetTextureScale(float textureScale)
        {
            this.linePath.textureScale = textureScale;
            this.linePath.Draw3D();
            VectorLine.SetCamera3D();
            //Debug.Log("Redraw with texturescale " + textureScale);
        }

      
        internal void UpdateAndRedraw(List<Vector3> objpos, List<Vector3> pointPath ,List<float> objdur)
        {
            base.UpdateAndRedraw(objpos);

            this.path_dur = objdur;
           
            this.pointPath.points3.Clear();
            this.pointPath.points3.AddRange(pointPath);
            this.pointPath.Draw3D();
            

            Debug.Log(   "EyeGazeDrawer::UpdateAndRedraw()");
           
        }

        public void SetMinMaxPointSize( float minPointSize, float maxPointSize)
        {
            this.maxPointSize = maxPointSize;
            this.minPointSize = minPointSize;

            SetPointSizeByDur();

            this.pointPath.Draw3D();

            VectorLine.SetCamera3D();
        }
        public void SetMinMaxDur(float minDur, float maxDur)
        {
          
            SetPointSizeByDur(minDur, maxDur);

            this.pointPath.Draw3D();

            VectorLine.SetCamera3D();
        }
        public void ShowHide_SimGazePath(bool showPath, in List<bool> levelOfDetailOptions, Material lineMaterial = null)
        {
            //check for basic
            if (linePath == null || linePath.points3 == null)
            {
                Debug.LogWarning("Path.points is not initialized");
                return;
            }
            else if (levelOfDetailOptions == null || levelOfDetailOptions.Count < (int)EyeGazeOption.All)
            {
                Debug.LogWarning("levelOfDetailOptions is not initialized");
                return;
            }


            if (levelOfDetailOptions[(int)EyeGazeOption.Line]) ShowHideLinePath(showPath);

            if (levelOfDetailOptions[(int)EyeGazeOption.Points])
            {
                //if give color, then use those color
                Color32 lineColor = lineMaterial != null ? lineMaterial.color : defaultLineColor;
                SetPathColor(lineColor);
                this.pointPath.active = true;
            }
            else
            {
                this.pointPath.active = false;
            }

        
        }

        public void ShowHide_SimGazePath(bool showPath, Material lineMaterial = null)
        {
            //check for basic
            if (linePath == null || linePath.points3 == null)
            {
                Debug.LogWarning("Path.points is not initialized");
                return;
            }

            ShowHideLinePath(showPath);
            if (showPath)
            {
                //if give color, then use those color
                Color32 lineColor = lineMaterial != null ? lineMaterial.color : defaultLineColor;
                this.SetPathColor(lineColor);

                this.pointPath.active = true;
            }
            else
            {
                this.pointPath.active = false;
            }

        }
    }




}

//public void ShowText(bool isShow)
//{
//    for (int i = 0; i < objArr.Count; i++)
//    {
//        GameObject obj = objArr[i];

//        SetTextValue setter = obj.GetComponentInChildren<SetTextValue>();
//        if (setter != null)
//        {
//            string text = $"playerX:{path_dur[i]}\n";
//            setter.ShowHideCanvas(isShow, text);
//        }

//    }
//}
