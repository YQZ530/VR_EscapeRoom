import os

def ifNeedToCreateFolder(filePath):
    # check if the folder existed
    if( "/" in filePath or "\\" in filePath):
        folder = filePath.rsplit('/', 1)[0]
        if(os.path.exists(folder) == False):
            # create a folder
            os.makedirs(folder)