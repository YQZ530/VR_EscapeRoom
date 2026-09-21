
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using DCXR.Util;


public enum EyeGazeOption
{
    Line = 0,
    Points,
    Text,
    Heatmap,
    All
}

namespace DCXR.PathDrawing
{


    public class DrawWalkPath : MonoBehaviour
    {


        public Texture eyeGazePointTex;
        [OnValueChanged("OnPointSizeChange")]
        public float maxPointSize = 5;
        [OnValueChanged("OnPointSizeChange")]
        public float minPointSize = 1;
        [OnValueChanged("OnPointHeightChange")]
        public float pathHeight = 1.5f;

        [OnValueChanged("OnTextureScaleChange")]
        public float eyegaze_textureScale = 1;
        [OnValueChanged("OnLineWidthChange")]
        public float lineWidth = 5f;

        public Texture lineTex;
        public Texture2D e_lineTex;
        public Texture2D e_frontTex;
        public Texture2D e_backTex;
        public Transform PathHolder;
        public EyeGazeDrawer simEyeGazePathDrawer;

        public Material line_material;

        int wlineIndex = 0;
        public GameObject chunkParent;
        List<MEyeGazeEvent> mEyeGazeEvents;
        
        List<Area> areaList;
        private bool isInit = false;
        #region callbacksWhenFieldChange
        public void OnLineWidthChange()
        {
            if (this.simEyeGazePathDrawer != null)
            {
                this.simEyeGazePathDrawer.SetLineWidth(lineWidth);
            }
        }

        private void OnTextureScaleChange()
        {
            if (this.simEyeGazePathDrawer != null)
            {
                this.simEyeGazePathDrawer.SetTextureScale(eyegaze_textureScale);
            }
        }

        private void OnPointHeightChange()
        {

            if (simEyeGazePathDrawer != null)
            {
                PathDrawer.SetPointHeight(simEyeGazePathDrawer.GetLinePath(), pathHeight);
                PathDrawer.SetPointHeight(simEyeGazePathDrawer.GetPointPathObj(), pathHeight);
            }


        }
        //call when max/minPointsize is changed by UI
        public void OnPointSizeChange()
        {

            if (this.simEyeGazePathDrawer != null)
            {
                this.simEyeGazePathDrawer.SetMinMaxPointSize(minPointSize, maxPointSize);

            }
        }
        #endregion
        private void Start()
        {
            if (eyeGazePointTex == null)
            {
                Debug.LogError("did not assign this");
            }
            isInit = false;
        }



        #region eyegaze

        public List<MEyeGazeEvent> GetEyeGazeEvents()
        {
            return this.mEyeGazeEvents;
        }



        public void CreateEyegazeDrawer(int userID, List<Vector3> objpos, List<Vector3> pointPath, List<float> objdur)
        {
            this.wlineIndex = userID;
            LoadLineMaterial();

            this.simEyeGazePathDrawer = new EyeGazeDrawer($"{gameObject.name}_SimEyeGaze", lineWidth,
                objpos, pointPath, objdur, line_material, null, eyeGazePointTex, maxPointSize, minPointSize,
                new Texture2D[] { e_lineTex, e_frontTex, e_backTex }, PathHolder);
            this.simEyeGazePathDrawer.SetTextureScale(eyegaze_textureScale);

          
           
            Debug.Log($"Done initialized eyegaze path drawer for {gameObject.name}");

        }
        
        public void UpdateEyeGazeEventAndRedraw(int userID, in List<MEyeGazeEvent> eyeGazeEvents)
        {
            this.mEyeGazeEvents = eyeGazeEvents;
            Debug.Log($"[DrawWalkPath::UpdateEyeGazeEventAndRedraw] {this.gameObject.name}::");//{ToString_GazeEvent(this.mEyeGazeEvents)}
            List<Vector3> objpos; List<float> objdur;
            areaList = chunkParent.GetComponentsInChildren<Area>().ToList();
            ObtainPosFromGazeData(in mEyeGazeEvents, in areaList, out objpos, out objdur);

            List<Vector3> pointPath = RandomizeXZ(objpos, 0.2f);
            if (!this.isInit)
            {
                CreateEyegazeDrawer(userID, objpos,pointPath, objdur);
                isInit = true;
            }
            else
            {
                this.simEyeGazePathDrawer.UpdateAndRedraw(objpos,pointPath, objdur);
            }

        }
        
        public static List<Vector3> RandomizeXZ(List<Vector3> positions, float range = 0.5f)
        {
            List<Vector3> randomized = new List<Vector3>(positions.Count);

            foreach (var pos in positions)
            {
                float offsetX = Random.Range(-range, range);
                float offsetY = Random.Range(-range, range);
                float offsetZ = Random.Range(-range, range);

                randomized.Add(new Vector3(
                    pos.x + offsetX,
                    pos.y + offsetY,          // Keep Y the same
                    pos.z + offsetZ
                ));
            }

            return randomized;
        }
        static void ObtainPosFromGazeData(in List<MEyeGazeEvent> mEyeGazeEvents, in List<Area> areaList,
                        out List<Vector3> objpos, out List<float> objdur)
        {
            objpos = new List<Vector3>();
            objdur = new List<float>();


            foreach (MEyeGazeEvent e in mEyeGazeEvents)
            {

               
                GameObject tarObj = null;
                HelperMethod.FindParentObjNodeInGame(e.AreaName, e.ParentName, areaList, out tarObj);
               
                if (tarObj != null)
                {
                    Vector3 objPos = tarObj.transform.position;
                    //if (tarObj.name.Equals("Bar"))
                    //{
                    //    objPos = new Vector3(objPos.x, objPos.y + 3f, objPos.z);
                    //}
                    if (Vector3.Distance(objPos , Vector3.zero) < 1f)
                    {
                        Debug.LogError($"area name {e.AreaName} parentname {e.ParentName} is zero");
                    }
                    
                    //Debug.Log($"area name {e.AreaName} parentname {e.ParentName} objPOs={objPos}");

                    objpos.Add(objPos);
                    objdur.Add(e.dur);
                 
                }
                
            }
        }

        public List<int> GetAllGazeDuration()
        {
            if (this.mEyeGazeEvents == null)
            {
                Debug.LogError("update gaze list is null");
                return null;
            }
            return this.mEyeGazeEvents.Select(elem => elem.dur).ToList();
        }
        string ToString_GazeEvent(List<MEyeGazeEvent> c_events)
        {
            string s = "[";
            foreach (MEyeGazeEvent e in c_events)
            {
                s += $"{e.AreaName}_{e.ParentName},";
            }
            s.Remove(s.Length - 1); //remove last comma
            s += "] ";
            return s;

        }



       

        public void ShowHideSimEyeGazePath(bool showPath, Material lineMaterial = null)
        {
            simEyeGazePathDrawer.ShowHide_SimGazePath(showPath, lineMaterial);

        }

        public void ShowHideSimEyeGazePath(bool showPath, in List<bool> levelOfDetailOptions, Material lineMaterial = null)
        {
            simEyeGazePathDrawer.ShowHide_SimGazePath(showPath, in levelOfDetailOptions, lineMaterial);
            OnPointSizeChange();
            OnLineWidthChange(); 
        }


        #endregion


        #region DrawPath

        void LoadLineMaterial()
        {
            string materialName = $"Line{this.wlineIndex}";
            //Debug.Log("try to get " + materialName + " in "+ Application.dataPath + "Materials/" + materialName);
            line_material = Resources.Load("Materials/" + materialName, typeof(Material)) as Material;
            if (line_material == null)
            {
                this.wlineIndex = 0;
                LoadLineMaterial();
            }

        }





        #endregion
    }
      
}

