arg4 = "AB4937230984FECC012312"
key = "009913aefcdb"

for i in range(len(arg4)):
    print((ord(arg4[i])+1)^ord(key[i%len(key)]),end=', ')