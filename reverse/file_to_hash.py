import binascii, hashlib

try:
    parts = input().split()
    if len(parts) != 2:
        print("Wrong input string")
        exit()
    a, b = parts[0].lower(), parts[1]
except EOFError:
    exit()

с = ['md5', 'sha1', 'sha256', 'sha512']

if a not in с:
    print("Unknown hash function")
    exit()

try:
    d = binascii.unhexlify(b)
except (binascii.Error, ValueError):
    print("Wrong input string")
    exit()

hash_obj = hashlib.new(a)
hash_obj.update(d)
print(hash_obj.hexdigest())
