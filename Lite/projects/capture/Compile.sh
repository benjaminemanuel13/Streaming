/usr/bin/gcc -fdiagnostics-color=always -g -shared /home/pi/projects/capture/transcoding.c -o /home/pi/projects/capture/stream.so -I/usr/local/ffmpeg/include -Wall -L/usr/local/ffmpeg/lib -lavcodec -lavdevice -lavfilter -lavformat -lavutil -lpostproc -lswresample -lswscale -lpthread -lm -fpic
/home/pi/projects/capture/transcoding.c