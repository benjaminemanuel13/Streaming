typedef void (*bytes_callback)(int8_t*, int);

//Video and Audio Capture To Callback
extern int setcallback(bytes_callback aCallback);
extern int freecallback();

extern int capture(char* filename, char* outfilename, char* tempname);
extern int stopcapture();

void encodeinit();
void encodeterm();