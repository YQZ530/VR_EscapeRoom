using UnityEngine;
using UnityEngine.UI;

namespace TwoCatsCode.UPython3Pro.Demo
{
    public class RunPythonScript : MonoBehaviour
    {
        public InputField saveToFileText;
        
        public InputField scriptText;

        public Text resultText;
        
        /// <summary>
        /// Button event
        /// </summary>
        public void Call()
        {
            if (saveToFileText.text != "" && scriptText.text != "")
            {
                UPython.Instance.CallPythonScript(saveToFileText.text, scriptText.text, Result);
            }
        }

        private void Result(string result)
        {
            resultText.text = result;
        }
    }
}