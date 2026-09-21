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
address = ('127.0.0.1', 7777)  

end_signal_len = len(end_signal.encode('utf-8'))


########### preload your data here ##########
texts = open("demo_big_file.txt").readlines()
print("------- Loaded -------")
print(texts)
print("---------------------")

########### add your logic here ##########
def command(data, conn, addr):
    if data.isdigit():
        index = eval(data)
        print("command: " + data)
        print("text: " + texts[index])
        conn.sendall(texts[index].encode('utf-8'))
    else:
        result_error(conn, addr)


# socket
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(address) 

print(s)


print("Custom Server Started!")
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
        
        # run command
        print('command: ' + data)

        command(data, conn, addr)

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



