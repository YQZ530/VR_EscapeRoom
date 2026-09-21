import sys

sum = 0

for i in range(1, len(sys.argv), 1):
    sum += float(sys.argv[i])

print(sum)