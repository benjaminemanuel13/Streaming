# Streaming

DjStreamer is an Asp.Net MVC Web Application that distributes the stream to web clients.
The two StreamingClient's are what capture the audio and send to DjStreamer.

StreamConsole contains the StreamLibrary used by the clients.

capture is the low level c++ used by the Clients. (stream.dll)

Lite is whats deployed to Linux (tested on RaspberyPi) and old version of capture.
