using DCXR.GameLogic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


	public class PlayerRecorder : MonoBehaviour, IGameplayRecorder
	{
		public int playerID = 1;
		public UnityAction<List<PlayerData>, int, float> SaveGameplayRecord { set; get; } = null;
		List<PlayerData> gameplayFrames;
		public GameObject LController;
		public GameObject RController;
		GameObject PlayerCamera;
		void Start()
		{
			
			gameplayFrames = new List<PlayerData>();
			if(LController == null || RController == null)
            {
				Debug.LogError("did not assign controllers");
            }

			PlayerCamera = this.transform.GetComponentInChildren<Camera>().gameObject;
			if(PlayerCamera == null)
            {
				Debug.LogError("Cannot find player Camera");
			}
		}

		private void OnEnable()
		{
           
            GameManager.onGameStart += StartRecord;
			GameManager.onGameEnd += OnGameEnd;
		}

		private void OnDisable()
		{
			GameManager.onGameStart -= StartRecord;
			GameManager.onGameEnd -= OnGameEnd;
		}


	
        public void StartRecord()
        {
			if(gameplayFrames != null) gameplayFrames.Clear();

			gameplayFrames = new List<PlayerData>();

			InvokeRepeating("SavePlayerData", 0f, 0.02f);
		}
		

		void SavePlayerData()
		{
			PlayerData frame = new PlayerData()
			{
				ModelPos = this.transform.position,
				ModelRot = this.transform.rotation,

				HeadPos = PlayerCamera.transform.position,
				HeadRot = PlayerCamera.transform.rotation,

				LHandPos = this.LController.transform.position,
				RHandPos = this.RController.transform.position,
				LHandRot = this.LController.transform.rotation,
				RHandRot = this.RController.transform.rotation,


				LHandLocPos = this.LController.transform.localPosition,
				RHandLocPos = this.RController.transform.localPosition,
				LHandLocRot = this.LController.transform.localRotation,
				RHandLocRot = this.RController.transform.localRotation,
			};

			gameplayFrames.Add(frame);

		}

		void OnGameEnd()
		{
			CancelInvoke();
			float camOffset = this.GetComponent<Unity.XR.CoreUtils.XROrigin>().CameraYOffset;
			SaveGameplayRecord?.Invoke(gameplayFrames, playerID, camOffset);
		}

	}
	