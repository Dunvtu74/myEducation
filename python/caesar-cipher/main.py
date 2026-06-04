import sys

def encrypt(text, shift):
    result = []
    for ch in text:
        if ch.isalpha():
            base = ord('A') if ch.isupper() else ord('a')
            result.append(chr((ord(ch) - base + shift) % 26 + base))
        else:
            result.append(ch)
    return "".join(result)

def decrypt(text, shift):
    return encrypt(text, -shift)

if __name__ == "__main__":
    msg = " ".join(sys.argv[1:]) if len(sys.argv) > 1 else "Hello World"
    shift = 3

    encoded = encrypt(msg, shift)
    decoded = decrypt(encoded, shift)

    print(f"исходный:    {msg}")
    print(f"зашифрован:  {encoded}")
    print(f"расшифрован: {decoded}")
