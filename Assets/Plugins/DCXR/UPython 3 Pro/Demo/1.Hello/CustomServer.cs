using UnityEngine;
using UnityEngine.UI;

namespace TwoCatsCode.UPython3Pro.Demo
{
    public class CustomServer : MonoBehaviour
    {
        public InputField yourName;

        public Text resultText;
        
        /// <summary>
        /// Button event
        /// </summary>
        public void Call()
        {
            UPython.Instance.CallCommand($"{yourName.text}", Result);
        }

        private void Result(string result)
        {
            resultText.text = result;
        }
    }
}