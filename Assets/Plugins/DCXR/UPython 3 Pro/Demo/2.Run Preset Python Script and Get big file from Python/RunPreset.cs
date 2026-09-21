using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TwoCatsCode.UPython3Pro.Demo
{
    public class RunPreset : MonoBehaviour
    {
        public InputField commandText;

        public Text resultText;

        public RawImage figure;

        private Texture2D texture2D;
        
        /// <summary>
        /// Button event
        /// </summary>
        public void Call()
        {
            if(commandText.text != "")
                UPython.Instance.CallPreset(commandText.text, Result);
        }

        private void Result(string result)
        {
            resultText.text = result;

            // Get the figure
            if (result.StartsWith("[Plot]"))
            {
                var fileName = result.Split(':')[1].Replace("\n","");
                Debug.Log($"Retrieving the figure {fileName}");
                UPython.Instance.CallPUT(fileName, ResultBytes);
            }
        }

        private void ResultBytes(UPythonConnection.ReceiveData data)
        {
            Debug.Log("Get bytes.");
            if (texture2D == null)
                texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(data.bytes);
            figure.texture = texture2D;
        }
        

        public void Preset1()
        {
            commandText.text = $"sum.py {Random.Range(0f,100f)} {Random.Range(0f,100f)} {Random.Range(0f,100f)}";
        }

        public void Preset2()
        {
            commandText.text = $"plot.py 'figs/plt.png' '0,1,2,3,4' " +
                               $"'{Random.Range(0f,100f)},{Random.Range(0f,100f)},{Random.Range(0f,100f)},{Random.Range(0f,100f)},{Random.Range(0f,100f)}'";
        }
    }
}