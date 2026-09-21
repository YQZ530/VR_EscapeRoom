using UnityEngine;

namespace TwoCatsCode.UPython3Pro
{
    [CreateAssetMenu(menuName = Utils.ComponentMenuPath + "/UPython Setting", fileName = "UPython Setting")]
    public class UPythonSetting : ScriptableObject
    {
        public string host = "127.0.0.1";
        public int port = 8888;
        public int dataBuffer = 4194304;
        public string end_signal = "[]";
        public string exit_cmd = "exit()";
        public int sendIntervalInMilliSeconds = 50;
        public bool showLog = true;
    }
}