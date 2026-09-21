# importing the required module 
import matplotlib.pyplot as plt 
import sys
import os
from Util import ifNeedToCreateFolder

# saving file
savedFile = sys.argv[1]

# x axis values 
x = sys.argv[2].split(',')
# corresponding y axis values 
y = sys.argv[3].split(',')

for i in range(0, len(x)):
    x[i] = float(x[i])
    y[i] = float(y[i])
    

# plotting the points 
plt.plot(x, y) 

# naming the x axis 
plt.xlabel('x - axis') 
# naming the y axis 
plt.ylabel('y - axis') 

# giving a title to my graph 
plt.title('My first graph!') 

# function to show the plot or save it to a file
# plt.show() 

# check if the folder existed
ifNeedToCreateFolder(savedFile)

# save fig
plt.savefig(savedFile)
plt.close()

# b.seek(0)
print("[Plot]:"+savedFile)
# print(b)



