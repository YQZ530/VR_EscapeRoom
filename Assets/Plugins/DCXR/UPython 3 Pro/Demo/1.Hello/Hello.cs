using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TwoCatsCode.UPython3Pro.Demo
{
    public class Hello : MonoBehaviour
    {
        public InputField yourName;

        public Text resultText;
        
        /// <summary>
        /// Button event
        /// </summary>
        public void Call()
        {
            UPython.Instance.CallPreset($"hello.py '{yourName.text}'", Result);
        }

        private void Result(string result)
        {
            resultText.text = result;
        }
    }
}

