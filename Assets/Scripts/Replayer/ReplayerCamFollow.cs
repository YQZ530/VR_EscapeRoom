using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace DCXR.Replayer
{
    public class ReplayerCamFollow : MonoBehaviour
    {
        public enum CamMode
        {
            side,
            topdown,
            nearFirstPerson,
            VR,
        };
        public CamMode camMode;
        private float zoom = 0; //z value affected for side, y value affected for top
        public float smoothTime = 0.3F;
        public Vector3 offset, defaultOffset;
        bool isInitialized = false;
        [ReadOnly]
        public GameObject targetObj;
        private Vector3 velocity = Vector3.zero;

        #region ReplayerIsPlaying
        private bool isplaying = false;
        void SetIsPlaying(bool isplaying)
        {
            this.isplaying = isplaying;
            var replayerEvent = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICamOffset>();
            foreach (var r in replayerEvent)
            {
                r.UpdateCamOffsetEvent += UpdateOffSet;
            }
        }

        private void OnEnable()
        {
            BasicReplayer.isPlayingEvent += SetIsPlaying;
        }
        private void OnDisable()
        {
            BasicReplayer.isPlayingEvent -= SetIsPlaying;
        }
        #endregion


        void FixedUpdate()
        {
            //if replayer is player, make cam follow target obj
            if (isplaying && this.targetObj != null )
            {
                UpdateCamPosition();
            }

        }
        private void UpdateCamPosition()
        {
            Vector3 newPos = targetObj.transform.position + offset;

            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
        }


        public void SetFollowObj(GameObject obj, CamMode camMode)
        {
         
            this.camMode = camMode;

            SetFollowObj(obj);
        }
        public void SetFollowObj(GameObject obj)
        {
            this.targetObj = obj;
         
            if (this.targetObj == null) return;

            if (!isInitialized)
            {
                defaultOffset = transform.position - targetObj.transform.position;
                isInitialized = true;
            }

            if (camMode == CamMode.topdown)
            {
                defaultOffset = new Vector3(0.17f, 10f, 4f);
            }
            else if (camMode == CamMode.nearFirstPerson)
            {
                defaultOffset = new Vector3(0.17f, 3.7f, -5f);
            }

            offset = defaultOffset;
            // Debug.Log($"SetFollowObj offset set to {defaultOffset}");

        }

       

        public void UpdateOffSet()
        {
            if (targetObj == null) return;
            defaultOffset = offset = transform.position - targetObj.transform.position;
        }

       
        public void SetZoom(float zoom)
        {
            if (zoom == this.zoom) return;

            var tempOffset = offset;
            if (camMode == CamMode.side)
            {
                if (targetObj == null) return;
                while (tempOffset.z - zoom < targetObj.transform.position.z + 5)
                    zoom -= 0.5f;

                float offDist = ((offset.z - zoom) - (targetObj.transform.position.z));
                if (offDist < 0) offDist = 0;
                float offDist2 = ((tempOffset.z - zoom) - (targetObj.transform.position.z));
                offset = new Vector3(tempOffset.x * (offDist / offDist2), tempOffset.y * (offDist / offDist2), tempOffset.z - zoom);

            }
            else if (camMode == CamMode.topdown)
            {
                this.GetComponent<Camera>().orthographicSize = zoom;
            }


            else if (camMode == CamMode.nearFirstPerson)
            {
                if (targetObj == null) return;
                offset = new Vector3(tempOffset.x , tempOffset.y, zoom);
                Debug.Log("Zoomed in x" + zoom);
                UpdateCamPosition();
            }

            this.zoom = zoom;

        }
    }

}
