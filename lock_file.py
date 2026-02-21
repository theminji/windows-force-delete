## Locks test.txt file to test the Force Delete

import time

f = open("test.txt", "r")

print("File is now locked. Press Ctrl+C to release.")
try:
    while True:
        time.sleep(1)
except KeyboardInterrupt:
    f.close()
    print("Released.")