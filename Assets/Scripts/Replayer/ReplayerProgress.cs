using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DCXR.Replayer
{
    public class ReplayerProgress
    {
        public GameObject player;

        public int curFrame;
        public int preFrame;
        public MGameplayKeyFrame keyFrame;
        public int pid;
        public ReplayerProgress(GameObject player,int pid, MGameplayKeyFrame keyFrame)
        {
            this.pid = pid;
            this.player = player;
           
            this.keyFrame = keyFrame;
            this.curFrame = keyFrame.startKeyframe;
            this.preFrame = this.curFrame;
        }
       
        public void SetKeyFrame( MGameplayKeyFrame keyFrame)
        {
            this.keyFrame = keyFrame;
            this.curFrame = keyFrame.startKeyframe;
            this.preFrame = this.curFrame;
            //Debug.Log("set target keyframe");
        }

        public void SetGameplayKeyFrameData(MGameplayKeyFrame keyFrame)
        {
            this.curFrame = keyFrame.startKeyframe;
        }

        public void ResetToStart()
        {
            this.curFrame = keyFrame.startKeyframe;
            this.preFrame = keyFrame.startKeyframe;
        }

        public void updateCurFame(int frameskipInc)
        {
            this.preFrame = this.curFrame; 
            this.curFrame = Mathf.Min(this.curFrame + frameskipInc, this.keyFrame.endKeyframe - 1);
        }

        public void Play(PlayerData data, bool showEyeGaze =false)
        {
            if (CheckReplayIsDone()) {
                //Debug.Log($"replay for player {this.keyFrame.userid} is done, from {keyFrame.startKeyframe} to {keyFrame.endKeyframe}");
                return; 
            }
            if (data.ModelPos != null)
            {
                //this.player.GetComponent<RePlayerStatus>().PlayData(data, preFrame, curFrame, showEyeGaze);
                Debug.Log("TODO");
            }
           
            //Debug.Log($"move to {data.position}");
        }

        internal void JumpToProgress(float curProgress)
        {
            int diff = this.keyFrame.endKeyframe - this.keyFrame.startKeyframe;
            int f =  Mathf.RoundToInt(diff * curProgress);

            if (f > this.keyFrame.endKeyframe) f = this.keyFrame.endKeyframe;
            this.curFrame = f;
        }

        public bool CheckReplayIsDone()
        {
            if(this.curFrame >= this.keyFrame.endKeyframe-1)
            {
                return true;
            }
            return false;   
        }

        public float GetProgress()
        {
            int diff = this.keyFrame.endKeyframe - this.keyFrame.startKeyframe;
            return this.curFrame / (diff * 1.0f);
        }
    }
}




