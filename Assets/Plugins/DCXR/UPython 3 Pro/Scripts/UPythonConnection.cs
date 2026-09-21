using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace TwoCatsCode.UPython3Pro
{
    public class UPythonConnection: IDisposable
    {
        protected UPythonSetting setting;

        public Socket socket;

        public class ReceiveData
        {
            public byte[] bytes = null;
            public int length = 0;
        }
        
        private UPythonConnection(UPythonSetting setting)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SendTimeout = 3600000;//int.MaxValue;
            socket.ReceiveTimeout = 3600000;//int.MaxValue;
            socket.SendBufferSize = setting.dataBuffer;
            socket.ReceiveBufferSize = setting.dataBuffer;
            this.setting = setting;
        }

        /// <summary>
        /// Create a connected connection.
        /// Supports async and callback event.
        /// Create a persistent connection, need to manually close it.
        /// Return the connection that has been connected to the python server.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task<UPythonConnection> CreateConnection(UPythonSetting setting, UnityAction<UPythonConnection> result = null)
        {
            // Create a local socket
            UPythonConnection conn = new UPythonConnection(setting);

            // Connect
            conn.socket.Connect(setting.host, setting.port);
            
            if(setting.showLog)
                Debug.Log($"[UPython] Connecting to {setting.host}:{setting.port} ...");
            while (!conn.socket.Connected)
                await Task.Yield();
            
            if (setting.showLog)
                Debug.Log("[UPython] Connected!");

            if(result !=null)
                result.Invoke(conn);
            return conn;
        }

        public async Task<int> Send(byte[] bytes,
                CancellationTokenSource taskCancelToken, UnityAction<int> result = null)
        {
            int count = 0 ;
            var task = new Task(() =>
            {
                try
                {
                    count = socket.Send(bytes);
                }
                catch (Exception e)
                {
                    if (setting.showLog)
                        Debug.Log(e);
                }

            }, taskCancelToken.Token);
            task.Start();
            await Task.WhenAll(task);
            
            if(setting.showLog)
                Debug.Log($"[UPython] Sending data size: {count}.");

            // Setting: Reduce traffic load
            await Task.Delay(setting.sendIntervalInMilliSeconds); 
                
            // End signal
            task = new Task(() =>
            {
                try
                {
                    socket.Send(Encoding.UTF8.GetBytes(setting.end_signal));
                }
                catch (Exception e)
                {
                    taskCancelToken.Cancel();
                    if (setting.showLog)
                        Debug.Log(e);
                }
            
            }, taskCancelToken.Token);
            task.Start();
            await Task.WhenAll(task);

            if (result != null)
            {
                result.Invoke(count);
            }
           
            return count;
        }

      

        public async Task<ReceiveData> Receive(CancellationTokenSource taskCancelToken, UnityAction<ReceiveData> result = null)
        {
            ReceiveData receiveData = new ReceiveData();
            receiveData.bytes = new byte[setting.dataBuffer];

            var task = new Task(() =>
            {
                try
                {
                    receiveData.length = socket.Receive(receiveData.bytes, setting.dataBuffer, 0);
                }
                catch (Exception e)
                {
                    taskCancelToken.Cancel();
                    if (setting.showLog)
                        Debug.Log(e);
                }

            }, taskCancelToken.Token);
            task.Start();

            // Waiting for receiving data from python server
            await Task.WhenAll(task);
            
            if(setting.showLog)
                Debug.Log($"[UPython] Received data size: {receiveData.length}.");
            
            if(result != null)
                result.Invoke(receiveData);

            return receiveData;
        }
        

        public async Task Close()
        {
            if(setting.showLog)
                Debug.Log($"[UPython] Sending disconnect event {setting.host}:{setting.port} ...");
            await Send(Encoding.UTF8.GetBytes(setting.exit_cmd), new CancellationTokenSource());
            socket.Close();
            
        }

        public void Dispose()
        {
            Close();
        }
    }
}