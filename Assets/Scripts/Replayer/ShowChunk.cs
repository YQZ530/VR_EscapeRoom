using System;

using TMPro;
using UnityEngine;

namespace DCXR.Replayer
{
    [Serializable]
    public struct ChunkAggregateData{
        public float GameplayDur;
        public float NumCoinsCollected;
        public float NumBombHit;
        public float NumObstaclesHit;
        public float NumTreasureCollected;

    }

    public class ShowChunk : MonoBehaviour
    {
        Canvas canvas;

        public TextMeshProUGUI textMeshPro;
        public int chunkID = 0;
        public ChunkAggregateData chunkData;
        void Start()
        {
            GameObject canvasObj = this.transform.Find("ChunkCanvas").gameObject;
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.enabled = false;

            textMeshPro = canvasObj.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = $"Chunk {chunkID}:\nAvg number coins collection;Avg box collection\nAvg number bomb hit;\n  Avg number obsticle hit \n ";
            textMeshPro.enableAutoSizing = false;
            textMeshPro.fontSize = 8;
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.alignment = TextAlignmentOptions.MidlineJustified;
        }

        // private void OnEnable()
        // {
        //     GameLoader.DoneLoadingEvent += LoadChunkAggregateData;
        // }
        // private void OnDisable()
        // {
        //     GameLoader.DoneLoadingEvent -= LoadChunkAggregateData;
        // }

        void LoadChunkAggregateData()
        {
            GameObject main =  GameObject.FindGameObjectWithTag("GameManager");
            if (main == null) { Debug.LogError("cannnot find obj with tag GameManager"); return; }
           // main.GetComponent<GameLoader>().GetChunkDetailInfo(chunkID, ref chunkData);
           // Debug.Log($"DONE LoadChunkAggregateData {chunkID}");
           Debug.Log("TODO");
        }

        public void OnShowChunkInfo()
        {
            canvas.enabled = true;
           
            textMeshPro.text = $"Chunk {chunkID}:\nAvg number coins={chunkData.NumCoinsCollected}\n" +
                $"Avg treasure box ={chunkData.NumTreasureCollected}\n" +
                $"Avg number bomb hit={chunkData.NumBombHit};\nAvg number obsticle hit={chunkData.NumObstaclesHit}\n";
        }


        public void OnHideChunkInfo()
        {
            canvas.enabled = false;
        }
    }
}