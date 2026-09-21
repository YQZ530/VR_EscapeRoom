using System;
using UnityEngine;
using UnityEngine.UI;

namespace TwoCatsCode.UPython3Pro.Demo
{
    public class SendBigFileToPython : MonoBehaviour
    {
        public InputField commandText;

        public Text resultText;

        public RawImage figure;
        

        /// <summary>
        /// Button event
        /// </summary>
        /// 
        public void Call()
        {
            if (commandText.text != "" && figure.texture != null)
            {
                // Convert image to bytes
                byte[] bytes = ((Texture2D)(figure.texture)).EncodeToJPG();
                UPython.Instance.CallUTP(commandText.text, bytes, Result);
            }
        }

        private void Result(string result)
        {
            resultText.text = result;
            
        }
        
    }
}