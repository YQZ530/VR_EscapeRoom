using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;


namespace TwoCatsCode.UPython3Pro
{
    /// <summary>
    /// The Core.
    /// </summary>
    [AddComponentMenu(Utils.ComponentMenuPath + "/UPython")]
    public class UPython : UPythonBase
    {
        // Singleton
        public static UPython Instance
        { 
            get
            {
                if (_Instance == null)
                    _Instance = FindObjectOfType<UPython>();
                return _Instance; 
            }
        }
        protected static UPython _Instance;
        

        private void Awake()
        {
            _Instance = this;
        }
        
        /// <summary>
        /// Call Preset python script
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task<string> CallPreset(string command, UnityAction<string> result = null)
        {
            var t = _Preset(command, result);
            await Task.WhenAll(t);
            return t.Result;
        }


        /// <summary>
        /// Bytes Python to Unity
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="result"></param>
        public async Task<UPythonConnection.ReceiveData> CallPUT(string filePath, UnityAction<UPythonConnection.ReceiveData> result = null)
        {
            var t = _PTU(filePath, result);
            await Task.WhenAll(t);
            return t.Result;
        }


        /// <summary>
        /// Bytes Unity to Python
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sendingBytes"></param>
        /// <param name="result"></param>
        public async Task<string> CallUTP(string filePath, byte[] sendingBytes, UnityAction<string> result = null)
        {
            var t = _UTP(filePath, sendingBytes, result);
            await Task.WhenAll(t);
            return t.Result;
        }


        /// <summary>
        /// Run raw python script
        /// </summary>
        /// <param name="saveToFilePath"></param>
        /// <param name="script"></param>
        /// <param name="result"></param>
        public async Task<string> CallPythonScript(string saveToFilePath, string script, UnityAction<string> result = null)
        {
            var t = _PythonScript(saveToFilePath, script, result);
            await Task.WhenAll(t);
            return t.Result;
        }

        public async void CallCommand(string fileName, UnityAction<string> result = null)
        {
            // var t = _Command(fileName, result);
            var t = _continueListening(fileName, result);
            await Task.WhenAll(t);

          
            //return t.Result;
        }
    } 
}

