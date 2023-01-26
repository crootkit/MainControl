# 用来解密发送的程序。
import binascii
import py7zr

# 解密压缩包
def decrpto7z():
    key = "{07;/<<=?@FBZXY[\]^_`aybz}"

    with open(b"L1033200429", "rb") as file:
        data = file.read()

    i = 0
    tmp = []
    data = data[:-10]
    for b in data[::-1]:
        tmp.append(ord(key[i % len(key)]) ^ b)
        i += 1

    data = bytearray(tmp)

    with open("decrypto.7z", "wb") as f:
        f.write(data)

# 解压压缩包
def unpack7z():
    with py7zr.SevenZipFile('decrypto.7z', password='JNsec-7A7576A96BFB33A66F8FE5EAED4BDBBA#') as z:
        z.extractall("E:\\2022JNCTF_moniter_Cshr\\服务端\\unpack")

#解密图片
def decryptoPhoto():
    key = "aqru;'[?094{AKZ|\%$@*&"

    with open(b"unpack\\SCnr.tmp", "rb") as file:
        data = file.read()
    i = 0
    tmp = []
    for b in data:
        tmp.append(ord(key[i % len(key)]) ^ b)
        i += 1

    data = bytearray(tmp)

    with open("unpack\\captu.png", "wb") as f:
        f.write(data)

decrpto7z()
unpack7z()
decryptoPhoto()