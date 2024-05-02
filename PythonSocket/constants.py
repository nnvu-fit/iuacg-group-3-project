import os

HOST = "0.0.0.0"
PORT = 65432  # port to listen on (non-privileged ports are > 1023)

# define path split character based on OS
PATH_SPLIT = "\\" if os.name == "nt" else "/"