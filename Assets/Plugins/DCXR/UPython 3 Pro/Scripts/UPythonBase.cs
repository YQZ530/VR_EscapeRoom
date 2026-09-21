using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using System.Threading;


namespace TwoCatsCode.UPython3Pro
{
    enum ConnectionState
    {
        Connected,
        Disconnected,
    }
    public abstract class UPythonBase : MonoBehaviour
    {
        public UPythonSetting setting;
        protected CancellationTokenSource taskCancelToken = new CancellationTokenSource();
        UPythonConnection mainConnection;

        ConnectionState curConnectionState;
        public virtual void Stop()
        {
            if (mainConnection != null)
            {
                mainConnection.Close();
               
                mainConnection = null;
                Debug.Log("close connection");
            }

            taskCancelToken.Cancel();
        }
        
        public virtual void ReStart()
        {
            taskCancelToken = new CancellationTokenSource();
        }


        protected async Task<string> _continueListening(string command, UnityAction<string> callback = null)
        {
            ////if last connection is still here, close it then create new one
            if (mainConnection != null)
            {
             
                await mainConnection.Close();
               
                mainConnection = null;
                if (setting.showLog) Debug.Log("[Upython]!!Close connection before oppening");
                await Task.Delay(1000);
                //return "null";
            }


            if (mainConnection == null)
            {
                //create new connection
                var tc = UPythonConnection.CreateConnection(setting, null);
                await Task.WhenAll(tc);
                mainConnection = tc.Result;
                ReStart();
            }




            if (setting.showLog) Debug.Log($"[UPython] Send: {command}");

            //send command to connection
            byte[] bs = Encoding.UTF8.GetBytes(command);
            await Task.WhenAll(mainConnection.Send(bs, taskCancelToken));

            //if connection on the other side is closed first
            if (taskCancelToken.IsCancellationRequested)
            {
                await mainConnection.Close();
                mainConnection = null;
                if (setting.showLog) Debug.Log($"[UPython] connection is cancel by remote host(python server)");
                return null;
            }

            string result = null;
            while (true)
            {
                // Waiting for receiving data from python server
                var task = mainConnection.Receive(taskCancelToken);
                await Task.WhenAll(task);

                result = Encoding.UTF8.GetString(task.Result.bytes, 0, task.Result.length);
                if (result == "Done" || mainConnection == null || taskCancelToken.IsCancellationRequested)
                {

                    if (mainConnection != null)
                    {
                        await mainConnection.Close();
                        mainConnection = null;
                    }
                    if (setting.showLog) Debug.Log($"[UPython] Listerner:: Disconnected with {setting.host}:{setting.port}");
                    break;
                }

                if (setting.showLog) Debug.Log($"[UPython] return: {result}");


                //do not close the main connection yet
              
                if (callback != null)
                    callback.Invoke(result);
            }
          


            return result;
        }


        protected async Task<string> _Command(string command, UnityAction<string> callback = null)
        {
            ////if last connection is still here, close it then create new one
            //if (mainConnection != null)
            //{
            //    await mainConnection.Close();
            //    if (setting.showLog) Debug.Log("close connection");
            //}

            //create new connection
            var tc = UPythonConnection.CreateConnection(setting, null);
            await Task.WhenAll(tc);
            mainConnection = tc.Result;


            if (setting.showLog) Debug.Log($"[UPython] Send: {command}");

            //send command to connection
            byte[] bs = Encoding.UTF8.GetBytes(command);
            await Task.WhenAll(mainConnection.Send(bs, taskCancelToken));

            // Waiting for receiving data from python server
            var task = mainConnection.Receive(taskCancelToken);
       
            await Task.WhenAll(task);

    
            var recStr = Encoding.UTF8.GetString(task.Result.bytes, 0, task.Result.length);

            if (setting.showLog) Debug.Log($"[UPython] return: {recStr}");


            
           // await mainConnection.Close();
            if (setting.showLog)
                Debug.Log($"[UPython] Disconnected with {setting.host}:{setting.port}");
            if (callback != null)
                callback.Invoke(recStr);


            return recStr;
         
        }

        /// <summary>
        /// Call Preset python script
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <param name="externalConnection"></param>
        /// <returns></returns>
        protected async Task<string> _Preset(string command, UnityAction<string> result = null,
            UPythonConnection externalConnection = null)
        {
            var typeAndCommand = $"{CmdTypes.P.ToString()} {command}";
            
            UPythonConnection c;
            if (externalConnection != null)
                c = externalConnection;
            else
            {
                var tc = UPythonConnection.CreateConnection(setting, null);
                await Task.WhenAll(tc);
                c = tc.Result;
            }

            if (setting.showLog)
                Debug.Log($"[UPython] Send: {typeAndCommand}");

            byte[] bs = Encoding.UTF8.GetBytes(typeAndCommand);
            await Task.WhenAll(c.Send(bs, taskCancelToken));
            
            var task = c.Receive(taskCancelToken);
            // Waiting for receiving data from python server
            await Task.WhenAll(task);
            
            // Close local socket
            if (externalConnection == null)
                await c.Close();

            if (setting.showLog)
                Debug.Log($"[UPython] Disconnected with {setting.host}:{setting.port}");
            
            var recStr = Encoding.UTF8.GetString(task.Result.bytes, 0, task.Result.length);
           
            if (setting.showLog)
                Debug.Log($"[UPython] return: {recStr}");

            // return the result
            if (result != null)
                result.Invoke(recStr);

            return recStr;
        }

        /// <summary>
        /// Bytes Python to Unity
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="resultBytes"></param>
        /// <param name="externalConnection"></param>
        /// <returns></returns>
        protected async Task<UPythonConnection.ReceiveData> _PTU(string filePath, UnityAction<UPythonConnection.ReceiveData> resultBytes = null,
            UPythonConnection externalConnection = null)
        {
            var typeAndFilePath = $"{CmdTypes.PTU.ToString()} {filePath}";
            
            UPythonConnection c;
            if (externalConnection != null)
                c = externalConnection;
            else
            {
                var tc = UPythonConnection.CreateConnection(setting, null);
                await Task.WhenAll(tc);
                c = tc.Result;
            }
            
            
            if (setting.showLog)
                Debug.Log($"[UPython] Send: {typeAndFilePath}");


            byte[] bs = Encoding.UTF8.GetBytes(typeAndFilePath);
            await Task.WhenAll(c.Send(bs, taskCancelToken));
            
            var task = c.Receive(taskCancelToken);
            // Waiting for receiving data from python server
            await Task.WhenAll(task);
            
            // Close local socket
            if (externalConnection == null)
                await c.Close();
            
            if (setting.showLog)
            {
                Debug.Log("[UPython2] DATA TRANSFERRING COMPLETED!");
                Debug.Log($"[UPython2] Disconnected with {setting.host}:{setting.port}");
            }

            if (resultBytes != null)
                resultBytes.Invoke(task.Result);

            return task.Result;
        }

        /// <summary>
        /// Bytes Unity to Python
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sendingBytes"></param>
        /// <param name="result"></param>
        /// <param name="externalConnection"></param>
        /// <returns></returns>
        protected async Task<string> _UTP(string filePath, byte[] sendingBytes, UnityAction<string> result = null,
            UPythonConnection externalConnection = null)
        {
            var typeAndFilePath = $"{CmdTypes.UTP.ToString()} {filePath}";
            
            UPythonConnection c;
            if (externalConnection != null)
                c = externalConnection;
            else
            {
                var tc = UPythonConnection.CreateConnection(setting, null);
                await Task.WhenAll(tc);
                c = tc.Result;
            }
            

            if (setting.showLog)
                Debug.Log($"[UPython] Send: {typeAndFilePath}");
            
            // Request a file space
            byte[] bs = Encoding.UTF8.GetBytes(typeAndFilePath);
            await Task.WhenAll(c.Send(bs, taskCancelToken));

            if (setting.showLog)
            {
                Debug.Log("[UPython] File space is ready.");
            }

            // Sending bytes
            await Task.WhenAll(c.Send(sendingBytes, taskCancelToken));

            if (setting.showLog)
            {
                Debug.Log("[UPython] DATA TRANSFERRING COMPLETED!");
            }
            
            // Response from python
            var recTask = c.Receive(taskCancelToken);
            await Task.WhenAll(recTask);
            
            // Close local socket
            if (externalConnection == null)
                await c.Close();
            
            var recStr = Encoding.UTF8.GetString(recTask.Result.bytes, 0, recTask.Result.length);

            if (setting.showLog)
                Debug.Log($"[UPython] return: {recStr}");

            // return the result
            if (result != null)
                result.Invoke(recStr);

            return recStr;
        }

        /// <summary>
        /// Run raw python script
        /// DOC: This is a good example of learning how to finish a list of tasks using one socket connection for the calling order-sensitive tasks.
        /// Create an external connection, connect to the python server, run the python tasks with the external connection, then close the connection.
        /// </summary>
        /// <param name="saveToFilePath"></param>
        /// <param name="script"></param>
        /// <param name="result"></param>
        /// <param name="externalConnection"></param>
        /// <returns></returns>
        protected async Task<string> _PythonScript(string saveToFilePath, string script, 
            UnityAction<string> result = null,
            UPythonConnection externalConnection = null)
        {
            UPythonConnection c;
            if (externalConnection != null)
                c = externalConnection;
            else
            {
                var tc = UPythonConnection.CreateConnection(setting, null);
                await Task.WhenAll(tc);
                c = tc.Result;
            }
            
            // Save the script to python side
            var t = _UTP(saveToFilePath, Encoding.UTF8.GetBytes(script), null, c);
            await Task.WhenAll(t);
            
            // Run the script as a preset
            t = _Preset(saveToFilePath, null, c);
            await Task.WhenAll(t);
            
            // Close local socket
            if (externalConnection == null)
                await c.Close();
            
            // Get result
            if(result != null)
                result.Invoke(t.Result);
            
            return t.Result;
        }

     


        protected void OnEnable()
        {
            ReStart();
        }

        protected void OnDisable()
        {
            
            Stop();
        }

    }
}