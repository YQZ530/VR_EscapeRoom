using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace DCXR.Replayer
{
    public class AdvanceReplayer : BasicReplayer
    {
        public AdvanceReplayer(GameLoader gameLoader, EventManager eventManager)
                        : base(gameLoader, eventManager)
        {
        }
        //
        // public override void AddPlayerToList(GameObject player, int pid)
        // {
        //     MGameplayKeyFrame keyFrame = gameLoader.GetSelectedUserKeyFrame(pid);
        //     ReplayerProgress r = new ReplayerProgress(player, pid, keyFrame);
        //
        //     List<MEyeGazeEvent> mEyeGazeEvents;
        //     gameLoader.GetSelectedEyegazeEvent(pid, out mEyeGazeEvents);
        //
        //     List<MChunkDetailEvent> mChunkDetailEvents = gameLoader.GetChunkDetailEvents(pid);
        //     player.GetComponent<RePlayerStatus>().LoadEventData(mEyeGazeEvents, mChunkDetailEvents);
        //     replayerList.Add(r);
        //     UpdateReplayerWithLargestFrame(r);
        // }
        //
        // public override void AddPlayerToList(GameObject player, int pid, int startChunk, int endChunk)
        // {
        //     MGameplayKeyFrame keyFrame = gameLoader.GetSelectedUserKeyFrame(pid, startChunk, endChunk);
        //     ReplayerProgress r = new ReplayerProgress(player, pid, keyFrame);
        //
        //     List<MEyeGazeEvent> mEyeGazeEvents = new List<MEyeGazeEvent>();
        //     //gameLoader.GetSelectedEyegazeEvent(pid, out mEyeGazeEvents);
        //
        //     List<MChunkDetailEvent> mChunkDetailEvents = gameLoader.GetChunkDetailEvents(pid);
        //     player.GetComponent<RePlayerStatus>().LoadEventData(mEyeGazeEvents, mChunkDetailEvents);
        //
        //     replayerList.Add(r);
        //     UpdateReplayerWithLargestFrame(r);
        // }
        //
        //
        // public override void PlayAFrame()
        // {
        //     
        //     if (CheckIfAllReplayDone())
        //     {
        //         shouldReset = true;
        //         PauseAndWindback();
        //         return;
        //     }
        //     if (shouldReset) //reset player's data if has play to the end or has jump to zero
        //     {
        //         ResetReplayer();
        //     }
        //     foreach (ReplayerProgress r in replayerList)
        //     {
        //         int pid = r.pid;
        //         r.updateCurFame(frameSkipInc);
        //         //2. load player data
        //         int curFrame = r.curFrame;
        //
        //         //load position/rot data for body & eye gaze
        //         PlayerData playerData = gameLoader.GetSelectedUserData(pid, curFrame);
        //         gameLoader.GetSelectedUserEyegazeData(pid, curFrame, ref playerData);
        //         r.Play(playerData, true); //advance replayer should load gaze data, so can show
        //
        //     }
        //
        //
        //     //curProgress UI still bound to main player in replayer list
        //     this.curProgress = Mathf.RoundToInt(replayerWithlargestFrame.GetProgress() * 100);
        // }
        //
        //
        // #region UI_Callback
        // //public override void ConstructRepalyerSlider(VisualElement replayerUI)
        // //{
        //
        // //    VisualElement playButt = replayerUI.Q<Button>("PlayButt");
        // //    playButt.RegisterCallback<ClickEvent>((evt) =>
        // //    {
        // //        this.playForward();
        //
        // //    });
        //
        // //    VisualElement pauseButt = replayerUI.Q<Button>("PauseButt");
        // //    pauseButt.RegisterCallback<ClickEvent>((e) => { this.PauseReplay(); });
        //
        // //    VisualElement speedSlider = replayerUI.Q<SliderInt>("SpeedMultiSlider");
        // //    speedSlider.dataSource = this;
        // //    speedSlider.SetBinding(nameof(SliderInt.value),
        // //        new DataBinding() { dataSourcePath = new PropertyPath(nameof(this.speedMultiplier)) });
        // //    //speedSlider.RegisterCallback<ClickEvent>((evt) =>
        // //    speedSlider.RegisterCallback<ChangeEvent<int>>((evt) =>
        // //    {
        // //        this.speedMultiplier = evt.newValue;
        // //        this.OnSpeedChanged();
        // //    });
        //
        // //    //replayer componnent
        // //    VisualElement replayerSlider = replayerUI.Q<VisualElement>("ReplayerSlider");
        //
        // //    ToggleButtonGroup progressbar = replayerSlider.Q<ToggleButtonGroup>("ProgressbarButtGroup");
        // //    progressbar.allowEmptySelection = true;
        //
        // //    SliderInt replayerSliderbar = replayerSlider.Q<SliderInt>("SliderBar");
        // //    replayerSliderbar.dataSource = this;
        //
        // //    replayerSliderbar.SetBinding(nameof(SliderInt.value),
        // //        new DataBinding() { dataSourcePath = new PropertyPath(nameof(this.curProgress)) });
        // //    replayerSliderbar.RegisterCallback<ClickEvent>((e) => this.OnCurProgrssChanged());
        //
        // //    TextField t = replayerSliderbar.Q<TextField>("unity-text-field");
        // //    t.RegisterCallback<ChangeEvent<string>>((evt) =>
        // //    {
        // //        Debug.Log("textfield change event ");
        // //        int v = int.Parse(evt.newValue);
        // //        this.curProgress = v;
        // //        this.OnCurProgrssChanged();
        // //    });
        // //}
        //
        // #endregion
    }
}