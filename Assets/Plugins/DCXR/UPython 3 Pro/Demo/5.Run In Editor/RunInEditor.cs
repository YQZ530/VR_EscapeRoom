using UnityEngine;
using UnityEngine.UI;

namespace TwoCatsCode.UPython3Pro.Demo
{
    public class RunInEditor : MonoBehaviour
    {
        public Text resultText;
        
        public string saveToFileText = "myScripts/RunInEditor.py";
        
        [TextArea(5,100)]
        public string scriptText ="print('Hello, Two Cats Code')\n" +
                                  "print('Hello, UPython 3 Pro')\n";
        
        /// <summary>
        /// Button event
        /// </summary>
        public void Call()
        {
            if (saveToFileText != "" && scriptText != "")
            {
                UPython.Instance.CallPythonScript(saveToFileText, scriptText, Result);
            }
        }

        private void Result(string result)
        {
            resultText.text = result;
        }
    }
}