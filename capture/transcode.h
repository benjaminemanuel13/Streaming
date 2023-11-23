typedef void (*bytes_callback)(uint8_t*, int);

//Video and Audio Capture To Callback
extern __declspec(dllexport) int setcallback(bytes_callback aCallback);
extern __declspec(dllexport) int freecallback();

extern __declspec(dllexport) int capture(char* filename, char* outfilename, char* tempname);
extern __declspec(dllexport) int stopcapture();

void encodeinit();
void encodeterm();