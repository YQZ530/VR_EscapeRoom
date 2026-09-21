using System;

using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace DCXR.Replayer
{
    public enum ReplayMode
    {
        stop, //replay stopped
        forward, //play forward
    };

    
    public class BasicReplayer
    {
        public static event Action<bool> isPlayingEvent;

        internal ReplayMode currentMode;
        
        internal int frameSkipInc = 1;
        internal GameLoader gameLoader;
        internal EventManager eventManager;
        internal List<ReplayerProgress> replayerList;

        public int curProgress = 2;
        public int speedMultiplier = 2;
        internal bool shouldReset;


        internal ReplayerProgress replayerWithlargestFrame;
        public BasicReplayer(GameLoader gameLoader, EventManager eventManager)
        {
            this.gameLoader = gameLoader;
            replayerList = new List<ReplayerProgress>();
            this.eventManager = eventManager;
            shouldReset = false;
        }

        public void ReInitializedPlayerList()
        {
            if (replayerList != null) {
                PauseAndWindback(); //reset player before reinitialize
                replayerList.Clear(); 
            }
            replayerList = new List<ReplayerProgress>();
            replayerWithlargestFrame = null;
        }

        public virtual void AddPlayerToList(GameObject player, int pid)
        {
            MGameplayKeyFrame keyFrame = gameLoader.GetSelectedUserKeyFrame(pid);
            ReplayerProgress r = new ReplayerProgress(player, pid, keyFrame);
            replayerList.Add(r);

            UpdateReplayerWithLargestFrame(r);
        }

        protected void UpdateReplayerWithLargestFrame(ReplayerProgress r)
        {
            if (replayerWithlargestFrame == null)
            {
                replayerWithlargestFrame = r;
            }
            else
            {
                if (r.keyFrame.endKeyframe > replayerWithlargestFrame.keyFrame.endKeyframe)
                    replayerWithlargestFrame = r;
            }
        }
        public virtual void AddPlayerToList(GameObject player, int pid, int startChunk, int endChunk)
        {
            MGameplayKeyFrame keyFrame = gameLoader.GetSelectedUserKeyFrame(pid, startChunk, endChunk);
            ReplayerProgress r = new ReplayerProgress(player, pid, keyFrame);
            replayerList.Add(r);
            UpdateReplayerWithLargestFrame(r);
        }

        //Play, pause forward, adjust speed etc
        #region GeneralControl
        internal void OnCurProgrssChanged()
        {
            PauseReplay();
            float f = this.curProgress / 100f;
            for (int i = 0; i < replayerList.Count; i++)
            {
                replayerList[i].JumpToProgress(f);
            }
            if(f == 0f)
            {
                PauseAndWindback();
                shouldReset = true;
            }
         //   Debug.Log($"OnCurProgrssChanged = {curProgress},");
        }
       void OnJumpToProgress()
        {
            PauseReplay();
            OnCurProgrssChanged();
            playForward();
        }

        internal void OnSpeedChanged()
        {
            frameSkipInc = Mathf.RoundToInt(1 * speedMultiplier);
            Debug.Log($"Speed Adjusted to x {speedMultiplier}, frameIncrement ={frameSkipInc}");
            //Debug.Log($"Speed Adjusted to x {speedMultiplier}, dt ={dt}");
        }
       

        //[HorizontalGroup("regular-replay"), Button(SdfIconType.Pause, ""), HideIf("currentMode", ReplayMode.stop)]
        public void PauseReplay()
        {
            currentMode = ReplayMode.stop;
            isPlayingEvent?.Invoke(false);
           // Debug.Log("T3:Stop");
        }


        //[HorizontalGroup("regular-replay"), ShowIf("currentMode", ReplayMode.stop), Button(SdfIconType.Play, "")]
        public void playForward()
        {
            Debug.Log("playForward");
            currentMode = ReplayMode.forward;
            isPlayingEvent?.Invoke(true);
        }

    

        //reset player progress to frame 0 & reset player status
        public  void PauseAndWindback()
        {
            PauseReplay();
            if (replayerList == null || replayerList.Count == 0) return;
            for (int i = 0; i < replayerList.Count; i++)
            {
                replayerList[i].ResetToStart();
            }
            this.curProgress = Mathf.RoundToInt(replayerWithlargestFrame.GetProgress() * 100);
           
            Debug.Log("reset players back to original position");
        }

        protected void ResetReplayer()
        {
            if (replayerList == null || replayerList.Count == 0) return;
            for (int i = 0; i < replayerList.Count; i++)
            {
               // replayerList[i].player.GetComponent<RePlayerStatus>().ResetForReplay();
            }
            shouldReset = false;
            Debug.Log("TODO");
        }
        #endregion

        internal bool CheckIfAllReplayDone()
        {
            bool isDone = true;
            if (replayerList == null || replayerList.Count == 0) return isDone;
            for (int i = 0; i < replayerList.Count; i++)
            {
                if (!replayerList[i].CheckReplayIsDone())
                {
                    isDone = false;
                }

            }
            if (isDone) Debug.Log("all player's replay is done");
          
            return isDone;
        }
        public virtual  void  PlayAFrame()
        {
            if (CheckIfAllReplayDone())
            {
                PauseAndWindback();
                shouldReset = true;
                return;
            }
            if (shouldReset) //reset player's data if has play to the end or has jump to zero
            {

                ResetReplayer();
            }



            // frame
            for (int i = 0; i < replayerList.Count; i++)
            {
                replayerList[i].updateCurFame(frameSkipInc);
                int curFrame = replayerList[i].curFrame;
                
                PlayerData data = gameLoader.GetSelectedUserData(replayerList[i].pid, curFrame);
                replayerList[i].Play(data, false); //basic replayer does not have eye gaze data so 
            }


            this.curProgress = Mathf.RoundToInt(replayerWithlargestFrame.GetProgress() * 100);
        }

  

        /// <summary>
        /// 
        /// </summary>

        #region level-oriented
    
      
        public int GetTotPlayer()
        {
            return gameLoader.totPlayer;
        }


        #endregion


       

        public void DestoryAllPlayers()
        {
            if (replayerList == null || replayerList.Count < 0) return;
            foreach (ReplayerProgress r in replayerList)
            {
                GameObject.Destroy(r.player);

            }
        }


     

       

        #region UI
        VisualElement replayerUI;
        public virtual void ConstructRepalyerSlider(VisualElement replayerUI)
        {
            this.replayerUI = replayerUI;
            ToggleButtonGroup PlayPauseTGroup = replayerUI.Q<ToggleButtonGroup>("PlayPauseTGroup");
            PlayPauseTGroup.allowEmptySelection = true;

            VisualElement playButt = replayerUI.Q<Button>("PlayButt");
            playButt.RegisterCallback<ClickEvent>((evt) =>
            {
                this.playForward();
               
            });

            VisualElement pauseButt = replayerUI.Q<Button>("PauseButt");
            pauseButt.RegisterCallback<ClickEvent>((e) => { this.PauseReplay(); });

            VisualElement speedSlider = replayerUI.Q<SliderInt>("SpeedMultiSlider");
            speedSlider.dataSource = this;
            speedSlider.SetBinding(nameof(SliderInt.value),
                new DataBinding() { dataSourcePath = new PropertyPath(nameof(this.speedMultiplier)) });
            //speedSlider.RegisterCallback<ClickEvent>((e) => this.OnSpeedChanged());
            speedSlider.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                this.speedMultiplier = evt.newValue;
                this.OnSpeedChanged();
            });


            //replayer componnent
            VisualElement replayerSlider = replayerUI.Q<VisualElement>("ReplayerSlider");

            ToggleButtonGroup progressbar = replayerSlider.Q<ToggleButtonGroup>("ProgressbarButtGroup");
            progressbar.allowEmptySelection = true;

         

            SliderInt replayerSliderbar = replayerSlider.Q<SliderInt>("SliderBar");
            replayerSliderbar.dataSource = this;

            replayerSliderbar.SetBinding(nameof(SliderInt.value),
                new DataBinding() { dataSourcePath = new PropertyPath(nameof(this.curProgress)) });

            replayerSliderbar.RegisterCallback<ChangeEvent<int>>((evt) => {
                this.curProgress = evt.newValue; //update slider to move
               
              //  Debug.Log("replayerSliderbar.change");
            });

            TextField t = replayerSliderbar.Q<TextField>("unity-text-field");
            t.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                //Debug.Log("textfield change event ");
                int v = int.Parse(evt.newValue);
                this.curProgress = v;
                this.OnCurProgrssChanged();
             
            });
        }

        


        

        #region User_anchored_Replay
        public void ReplayGameplayEvent(List<MGameEvent> events)
        {
            //if (!CheckIfAllReplayDone())
            //{
            //    stop current replay
            //    PauseReplay();
            //}
            //LoadGameEvent(events);
            //this.playForward();
        }

        #endregion



        #endregion
    }


    
}


