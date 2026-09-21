import socket
import os
import _thread as thread
import time
from Util import ifNeedToCreateFolder


# settings
bufferSize = 4194304
python_cmd = 'python'
end_signal = '[]'
exit_cmd = 'exit()'
address = ('127.0.0.1', 8888)  

end_signal_len = len(end_signal.encode('utf-8'))

# socket
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(address) 

print(s)


print("Server Started!")
s.listen(10)

# thread
def client_accept():
    while True:
        print("Listening...")
        try:
            conn, addr = s.accept()
            print('[+] Connected with', addr)
            thread.start_new_thread(client_handle, (conn, addr))
        except:
            break

def recvall(conn):
    # Helper function to recv n bytes or return None if EOF is hit
    data = bytearray()
    try:
        while len(data) < bufferSize:
            packet = conn.recv(bufferSize)
            if not packet:
                break
            if len(packet) == 0:
                break
            if len(packet) == end_signal_len:
                if packet.decode('utf-8') == end_signal: # end signal
                    # print(end_signal)
                    break
            print('Packet size: ' + str(len(packet)))
            data.extend(packet)
            print('Received:    ' + str(len(data)))
    except:
        ...
    return data


def result_error(conn, addr):
    try:
        print("Result: [Error]")
        send = "[Error]".encode('utf-8')
        print("Send size: "+str(len(send)))
        conn.sendall(send)
    except:
        print(addr, "Disconnected.")

def run_preset_python_script(conn, addr, command):
    cmd = python_cmd + " " + command
    print(addr, cmd)
    
    # run a command
    try:
        f = os.popen(cmd, 'r')
        send = f.read()	
        f.close()

        if(len(send) == 0):
            result_error(conn, addr)
        else:
            print("Result: ", send)
            print("Send size: "+str(len(send)))
            conn.sendall(send.encode('utf-8'))
    except:
        result_error(conn, addr)

def file_python_to_unity(conn, addr, command):
    # file
    try:
        # cwd = os.getcwd()
        filepath = command
        print("file path: "+filepath)
        # if file exists
        if os.path.exists(filepath):
            # read file
            file = open(filepath, 'rb')
            raw = file.read()
            print("Read File: "+filepath)
            print("DATA TRANSFERRING...")
            print("Send size: "+str(len(raw)))
            conn.sendall(raw)
            print("Sent!")
            
        else:
            send = "No File: "+filepath
            print(send)
            print("Send size: "+str(len(send)))
            conn.sendall('') 
    except:
        result_error(conn, addr)

def file_unity_to_python(conn, addr, command):
    try:
        filepath = command
        print("file path: "+filepath)

        # check if the folder existed
        ifNeedToCreateFolder(filepath)
        # remove the old file if any
        if os.path.exists(filepath):
            os.remove(filepath)
            

        print("DATA TRANSFERRING...")
        raw = recvall(conn)
        print("SAVING...")
        print("Size: "+str(len(raw)))
        # append the data to file
        file = open(filepath,'wb')
        file.write(raw)
        file.close()
        print("Result: [Saved]")
        # response
        send = ("[Saved]:"+filepath).encode('utf-8')
        print("Send size: "+str(len(send)))
        conn.sendall(send)

    except:
        result_error(conn, addr)

def client_handle(conn, addr):
    while True:
        try:
            # decode the network content
            data = recvall(conn).decode('utf-8')
        except:
            # result_error(conn, addr)
            break

        if not data:
            continue

        print('data: '+data)
        print("data size: "+str(len(data)))

        # exit cmd
        if(data == exit_cmd):
            print(data)
            break

        # parse the data -> type command
        [type, command] = data.split(' ', 1)
        print('type: ' + type)
        print('command: ' + command)
        if(type == 'P'): # run preset python script
            run_preset_python_script(conn, addr, command)
        elif(type == 'PTU'): # file python to unity
            file_python_to_unity(conn, addr, command)
        elif(type == 'UTP'): # file python to unity
            file_unity_to_python(conn, addr, command)
        else:
            result_error(conn, addr)

        time.sleep(0.05) # setting: reduce the traffic load

    conn.close()
    print('[-] Disconnected with', addr)
    print()

thread.start_new_thread(client_accept, ())

while True:
    cmd = input()
    if(cmd == exit_cmd):
        break

s.close()

print("Server Stopped!")



