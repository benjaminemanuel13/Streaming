
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>

#include <libavutil/avassert.h>
#include <libavutil/opt.h>
#include <libavutil/mathematics.h>
#include <libavutil/timestamp.h>
#include <libswscale/swscale.h>
#include <libswresample/swresample.h>

#include <libavutil/imgutils.h>
#include <libavutil/samplefmt.h>
#include <libavutil/timestamp.h>
#include <libavformat/avformat.h>

#include <libavcodec/avcodec.h>
#include <libavutil/channel_layout.h>
#include <libavutil/common.h>
#include <libavutil/frame.h>
#include <libavutil/mem.h>

#include <libavformat/avformat.h>
#include <libavformat/avio.h>

#include <libavcodec/avcodec.h>

#include <libavutil/audio_fifo.h>
#include <libavutil/avassert.h>
#include <libavutil/avstring.h>
#include <libavutil/opt.h>

#include <libswresample/swresample.h>

#include <libavutil/opt.h>

#include <libavdevice/avdevice.h>

#include "transcode.h"

/* The output bit rate in bit/s */
#define OUTPUT_BIT_RATE 96000
/* The number of output channels */
#define OUTPUT_CHANNELS 2

#define AUDIO_INBUF_SIZE 20480
#define AUDIO_REFILL_THRESH 4096

#define STREAM_DURATION   10.0
#define STREAM_FRAME_RATE 25 /* 25 images/s */
#define STREAM_PIX_FMT    AV_PIX_FMT_YUV420P /* default pix_fmt */

#define SCALE_FLAGS SWS_BICUBIC

static AVFormatContext *fmt_ctx = NULL;
static AVCodecContext *video_dec_ctx = NULL, *audio_dec_ctx;
static int width, height;
static enum AVPixelFormat pix_fmt;
static AVStream *video_stream = NULL, *audio_stream = NULL;
static const char *src_filename = NULL;
static const char *video_dst_filename = NULL;
static const char *audio_dst_filename = NULL;
static FILE *video_dst_file = NULL;
static FILE *audio_dst_file = NULL;

static uint8_t *video_dst_data[4] = {NULL};
static int      video_dst_linesize[4];
static int video_dst_bufsize;

static int video_stream_idx = -1, audio_stream_idx = -1;
static AVFrame *frame = NULL;
static AVPacket pkt;
static int video_frame_count = 0;
static int audio_frame_count = 0;

char* tempfilename;

/* Enable or disable frame reference counting. You are not supposed to support
 * both paths in your application but pick the one most appropriate to your
 * needs. Look for the use of refcount in this example to see what are the
 * differences of API usage between them. */
static int refcount = 0;
int count = 0;

bytes_callback byteC;

static int decode_packet(int *got_frame, int cached)
{
    int ret = 0;
    int decoded = pkt.size;

    *got_frame = 0;

   
        /* decode audio frame */
    ret = avcodec_decode_audio4(audio_dec_ctx, frame, got_frame, &pkt);
    if (ret < 0) {
        fprintf(stderr, "Error decoding audio frame (%s)\n", av_err2str(ret));
        return ret;
    }
    /* Some audio decoders decode only part of the packet, and have to be
        * called again with the remainder of the packet data.
        * Sample: fate-suite/lossless-audio/luckynight-partial.shn
        * Also, some decoders might over-read the packet. */
    decoded = FFMIN(ret, pkt.size);
    fprintf(stderr, "Decoded (%d) of %d\n", ret, pkt.size);
    
    if (*got_frame) {
        size_t unpadded_linesize = frame->nb_samples * av_get_bytes_per_sample(frame->format);
        printf("audio_frame%s n:%d nb_samples:%d pts:%s\n",
                cached ? "(cached)" : "",
                audio_frame_count++, frame->nb_samples,
                av_ts2timestr(frame->pts, &audio_dec_ctx->time_base));

        fprintf(stderr, "Linesize: %d\n", unpadded_linesize);
        /* Write the raw audio data samples of the first plane. This works
            * fine for packed formats (e.g. AV_SAMPLE_FMT_S16). However,
            * most audio decoders output planar audio, which uses a separate
            * plane of audio samples for each channel (e.g. AV_SAMPLE_FMT_S16P).
            * In other words, this code will write only the first audio channel
            * in these cases.
            * You should use libswresample or libavfilter to convert the frame
            * to packed data. */
        //

        audio_dst_file = fopen(tempfilename, "wb");
        
        if (!audio_dst_file) {
            fprintf(stderr, "Could not open temp file %s\n", tempfilename);
            ret = 1;
            goto end;
        }
        // 
        //fwrite(frame->extended_data[0], 1, unpadded_linesize * 2, audio_dst_file);
        uint32_t dummy[88200];

        //fwrite(frame->extended_data[0], 1, unpadded_linesize * 2, audio_dst_file);
        //fwrite(frame->extended_data[1], 1, unpadded_linesize * 2, audio_dst_file);

        fwrite(frame->data[0], 1, unpadded_linesize * 2, audio_dst_file);
        //fwrite(frame->extended_data[1], 1, unpadded_linesize * 2, audio_dst_file);
        
        fclose(audio_dst_file);

        encodebody();
    }

    if (*got_frame && refcount)
        av_frame_unref(frame);

end:
    return decoded;
}

static int open_codec_context(int *stream_idx,
                              AVCodecContext **dec_ctx, AVFormatContext *fmt_ctx, enum AVMediaType type)
{
    int ret, stream_index;
    AVStream *st;
    AVCodec *dec = NULL;
    AVDictionary *opts = NULL;

    ret = av_find_best_stream(fmt_ctx, type, -1, -1, NULL, 0);
    if (ret < 0) {
        fprintf(stderr, "Could not find %s stream in input file '%s'\n",
                av_get_media_type_string(type), src_filename);
        return ret;
    } else {
        stream_index = ret;
        st = fmt_ctx->streams[stream_index];

        /* find decoder for the stream */
        dec = avcodec_find_decoder(st->codecpar->codec_id);
        if (!dec) {
            fprintf(stderr, "Failed to find %s codec\n",
                    av_get_media_type_string(type));
            return AVERROR(EINVAL);
        }

        /* Allocate a codec context for the decoder */
        *dec_ctx = avcodec_alloc_context3(dec);
        if (!*dec_ctx) {
            fprintf(stderr, "Failed to allocate the %s codec context\n",
                    av_get_media_type_string(type));
            return AVERROR(ENOMEM);
        }

        /* Copy codec parameters from input stream to output codec context */
        if ((ret = avcodec_parameters_to_context(*dec_ctx, st->codecpar)) < 0) {
            fprintf(stderr, "Failed to copy %s codec parameters to decoder context\n",
                    av_get_media_type_string(type));
            return ret;
        }

        /* Init the decoders, with or without reference counting */
        av_dict_set(&opts, "refcounted_frames", refcount ? "1" : "0", 0);
        if ((ret = avcodec_open2(*dec_ctx, dec, &opts)) < 0) {
            fprintf(stderr, "Failed to open %s codec\n",
                    av_get_media_type_string(type));
            return ret;
        }
        *stream_idx = stream_index;
    }

    return 0;
}

static int get_format_from_sample_fmt(const char **fmt,
                                      enum AVSampleFormat sample_fmt)
{
    int i;
    struct sample_fmt_entry {
        enum AVSampleFormat sample_fmt; const char *fmt_be, *fmt_le;
    } sample_fmt_entries[] = {
        { AV_SAMPLE_FMT_U8,  "u8",    "u8"    },
        { AV_SAMPLE_FMT_S16, "s16be", "s16le" },
        { AV_SAMPLE_FMT_S32, "s32be", "s32le" },
        { AV_SAMPLE_FMT_FLT, "f32be", "f32le" },
        { AV_SAMPLE_FMT_DBL, "f64be", "f64le" },
    };
    *fmt = NULL;

    for (i = 0; i < FF_ARRAY_ELEMS(sample_fmt_entries); i++) {
        struct sample_fmt_entry *entry = &sample_fmt_entries[i];
        if (sample_fmt == entry->sample_fmt) {
            *fmt = AV_NE(entry->fmt_be, entry->fmt_le);
            return 0;
        }
    }

    fprintf(stderr,
            "sample format %s is not supported as output format\n",
            av_get_sample_fmt_name(sample_fmt));
    return -1;
}




/* check that a given sample format is supported by the encoder */
static int check_sample_fmt(const AVCodec *codec, enum AVSampleFormat sample_fmt)
{
    const enum AVSampleFormat *p = codec->sample_fmts;

    while (*p != AV_SAMPLE_FMT_NONE) {
        if (*p == sample_fmt)
            return 1;
        p++;
    }
    return 0;
}

/* just pick the highest supported samplerate */
static int select_sample_rate(const AVCodec *codec)
{
    const int *p;
    int best_samplerate = 0;

    if (!codec->supported_samplerates)
        return 44100;

    p = codec->supported_samplerates;
    while (*p) {
        if (!best_samplerate || abs(44100 - *p) < abs(44100 - best_samplerate))
            best_samplerate = *p;
        p++;
    }
    return best_samplerate;
}

FILE *f;

/* select layout with the highest channel count */
static int select_channel_layout(const AVCodec *codec)
{
    const uint64_t *p;
    uint64_t best_ch_layout = 0;
    int best_nb_channels   = 0;

    if (!codec->channel_layouts)
        return AV_CH_LAYOUT_STEREO;

    p = codec->channel_layouts;
    while (*p) {
        int nb_channels = av_get_channel_layout_nb_channels(*p);

        if (nb_channels > best_nb_channels) {
            best_ch_layout    = *p;
            best_nb_channels = nb_channels;
        }
        p++;
    }
    return best_ch_layout;
}

static void encode(AVCodecContext *ctx, AVFrame *frame, AVPacket *pkt,
                   FILE *output)
{
    int ret;

    /* send the frame for encoding */
    ret = avcodec_send_frame(ctx, frame);
    if (ret < 0) {
        fprintf(stderr, "Error sending the frame to the encoder\n");
        exit(1);
    }

    /* read all the available output packets (in general there may be any
     * number of them */
    while (ret >= 0) {
        ret = avcodec_receive_packet(ctx, pkt);
        if (ret == AVERROR(EAGAIN) || ret == AVERROR_EOF)
            return;
        else if (ret < 0) {
            fprintf(stderr, "Error encoding audio frame\n");
            exit(1);
        }

        fwrite(pkt->data, 1, pkt->size, f);
        byteC(pkt->data, pkt->size);

        av_packet_unref(pkt);
    }
}

typedef union
{
    int32_t i;
    float f;
} u;


struct SwrContext *swr_ctx;

int ret = 0, got_frame;

AVFrame *encframe;
AVPacket *encpkt;

int16_t *samples1;
int16_t *samples2;
//int16_t *samples3;
//int16_t *samples4;
AVCodecContext *c= NULL;
int i, j, k;

extern __declspec(dllexport) int setcallback(bytes_callback aCallback)
{
    byteC = aCallback;
}

extern __declspec(dllexport) int freecallback()
{
    free(byteC);

    return -1;
}

int encodefile(char* infilename, char* filename)
{
    const AVCodec *codec;
    AVCodecContext *c= NULL;
    AVFrame *encframe;
    AVPacket *encpkt;
    int i, j, k, ret;
    FILE *f;
    int16_t *samples1;
    int16_t *samples2;

    float t, tincr;

    /* find the MP3 encoder */
    codec = avcodec_find_encoder(AV_CODEC_ID_MP3);
    if (!codec) {
        fprintf(stderr, "Codec not found\n");
        exit(1);
    }

    c = avcodec_alloc_context3(codec);
    if (!c) {
        fprintf(stderr, "Could not allocate audio codec context\n");
        exit(1);
    }

    /* put sample parameters */
    c->bit_rate = 1411200;

    /* check that the encoder supports s16 pcm input */
    c->sample_fmt = AV_SAMPLE_FMT_S16P;
    if (!check_sample_fmt(codec, c->sample_fmt)) {
        fprintf(stderr, "Encoder does not support sample format %s",
                av_get_sample_fmt_name(c->sample_fmt));
        exit(1);
    }

    /* select other audio parameters supported by the encoder */
    c->sample_rate    = select_sample_rate(codec);
    c->channel_layout = select_channel_layout(codec);
    c->channels       = av_get_channel_layout_nb_channels(c->channel_layout);

    /* open it */
    if (avcodec_open2(c, codec, NULL) < 0) {
        fprintf(stderr, "Could not open codec\n");
        exit(1);
    }

    //f = fopen(filename, "wb");
    //if (!f) {
    //    fprintf(stderr, "Could not open %s\n", filename);
    //    exit(1);
    //}

    /* packet for holding encoded output */
    encpkt = av_packet_alloc();
    if (!encpkt) {
        fprintf(stderr, "could not allocate the packet\n");
        exit(1);
    }

    /* frame containing input raw audio */
    encframe = av_frame_alloc();
    if (!encframe) {
        fprintf(stderr, "Could not allocate audio frame\n");
        exit(1);
    }

    encframe->nb_samples     = c->frame_size;
    encframe->format         = c->sample_fmt;
    encframe->channel_layout = c->channel_layout;

    /* allocate the data buffers */
    ret = av_frame_get_buffer(encframe, 0);
    if (ret < 0) {
        fprintf(stderr, "Could not allocate audio data buffers\n");
        exit(1);
    } 

    FILE* fin = fopen(infilename, "rb");

    float inbuf[1152 * 4];
    int32_t outbuf[1152 * 4];
    float *data;
    size_t   data_size;

    data = inbuf;

    data_size = fread(inbuf, 1, 1152 * 4, fin);

    while (data_size > 0) {
        
        int pos = 0;
        for(int a= 0; a < 1; a++)
        {
            ret = av_frame_make_writable(encframe);
            if (ret < 0)
                exit(1);

            samples1 = (int16_t*)encframe->data[0];
            samples2 = (int16_t*)encframe->data[1];
            
            for (j = 0; j < c->frame_size; j++){
                float val = inbuf[j];
                int16_t newval = (*((int16_t*)&val)/2);

                //Left
                samples1[j] = newval;
                
                //Right
                samples2[j] = newval;
                
                pos++;
            }
                
            encode(c, encframe, encpkt, f);
        }
        data_size = fread(inbuf, 1, 1152 * 4, fin);
    }

    /* flush the encoder */
    encode(c, NULL, encpkt, f);

    fclose(f);

    av_frame_free(&frame);
    av_packet_free(&pkt);
    avcodec_free_context(&c);

    return 0;
}

void demuxinit(char* filename, char* outfilename)
{
    int ret = 0, got_frame;

    src_filename = filename;
    audio_dst_filename = outfilename;

    avdevice_register_all();

    AVInputFormat* fmt;
    fmt = av_find_input_format("dshow");

    /* open input file, and allocate format context */
    if (avformat_open_input(&fmt_ctx, src_filename, fmt, NULL) < 0) {
        fprintf(stderr, "Could not open source file %s\n", src_filename);
        exit(1);
    }

    /* retrieve stream information */
    if (avformat_find_stream_info(fmt_ctx, NULL) < 0) {
        fprintf(stderr, "Could not find stream information\n");
        exit(1);
    }

    if (open_codec_context(&audio_stream_idx, &audio_dec_ctx, fmt_ctx, AVMEDIA_TYPE_AUDIO) >= 0) {
        audio_stream = fmt_ctx->streams[audio_stream_idx];
    }

    /* dump input information to stderr */
    av_dump_format(fmt_ctx, 0, src_filename, 0);

    if (!audio_stream) {
        fprintf(stderr, "Could not find audio stream in the input, aborting\n");
        ret = 1;
        goto end;
    }

    frame = av_frame_alloc();
    if (!frame) {
        fprintf(stderr, "Could not allocate frame\n");
        ret = AVERROR(ENOMEM);
        goto end;
    }

    /* initialize packet, set data to NULL, let the demuxer fill it */
    av_init_packet(&pkt);
    pkt.data = NULL;
    pkt.size = 0;

    if (audio_stream)
        printf("Demuxing audio from file '%s' into '%s'\n", src_filename, audio_dst_filename);
end:
}

void encodeinit(char* filename)
{
    const AVCodec *codec;
    int i, j, k, ret;
    float t, tincr;

    /* find the MP3 encoder */
    codec = avcodec_find_encoder(AV_CODEC_ID_MP3);
    if (!codec) {
        fprintf(stderr, "Codec not found\n");
        exit(1);
    }

    c = avcodec_alloc_context3(codec);
    if (!c) {
        fprintf(stderr, "Could not allocate audio codec context\n");
        exit(1);
    }

    /* put sample parameters */
    c->bit_rate = 1411200;
    //c->bit_rate = 68000;

    /* check that the encoder supports s16 pcm input */
    c->sample_fmt = AV_SAMPLE_FMT_S16P;
    if (!check_sample_fmt(codec, c->sample_fmt)) {
        fprintf(stderr, "Encoder does not support sample format %s",
                av_get_sample_fmt_name(c->sample_fmt));
        exit(1);
    }

    /* select other audio parameters supported by the encoder */
    c->sample_rate    = select_sample_rate(codec);
    c->channel_layout = select_channel_layout(codec);
    c->channels       = av_get_channel_layout_nb_channels(c->channel_layout);

    /* open it */
    if (avcodec_open2(c, codec, NULL) < 0) {
        fprintf(stderr, "Could not open codec\n");
        exit(1);
    }

    f = fopen(filename, "wb");
    if (!f) {
        fprintf(stderr, "Could not open %s\n", filename);
        exit(1);
    }

    /* packet for holding encoded output */
    encpkt = av_packet_alloc();
    if (!encpkt) {
        fprintf(stderr, "could not allocate the packet\n");
        exit(1);
    }

    /* frame containing input raw audio */
    encframe = av_frame_alloc();
    if (!encframe) {
        fprintf(stderr, "Could not allocate audio frame\n");
        exit(1);
    }

    encframe->nb_samples     = c->frame_size;
    encframe->format         = c->sample_fmt;
    encframe->channel_layout = c->channel_layout;

    /* allocate the data buffers */
    ret = av_frame_get_buffer(encframe, 0);
    if (ret < 0) {
        fprintf(stderr, "Could not allocate audio data buffers\n");
        exit(1);
    } 

    ret = av_frame_make_writable(encframe);
        if (ret < 0)
        exit(1);
end:
}

int running = 1;

void demuxbody()
{
    /* read frames from the file */
    while (av_read_frame(fmt_ctx, &pkt) >= 0  && running == 1) {
        AVPacket orig_pkt = pkt;
        do {
            ret = decode_packet(&got_frame, 0);
            if (ret < 0)
                break;
            pkt.data += ret;
            pkt.size -= ret;
        } while (pkt.size > 0);
        av_packet_unref(&orig_pkt);
    }

    /* flush cached frames */
    pkt.data = NULL;
    pkt.size = 0;
    do {
        decode_packet(&got_frame, 1);
    } while (got_frame);
}

void encodebody()
{
    FILE* fin = fopen(tempfilename, "rb");

    int32_t inbuf[1152 * 4];
    size_t data_size;

    data_size = fread(inbuf, 1, 1152 * 4, fin);

    int pos = 0;

    while (data_size > 0) {
        
        int pos = 0;
        
        samples1 = (int16_t*)encframe->data[0];
        samples2 = (int16_t*)encframe->data[1];
        
        for (j = 0; j < c->frame_size; j+=2){
            //l
            int32_t val1 = inbuf[pos];
            //r
            int32_t val2 = inbuf[pos + 1];
            
            //int16_t newval = (*((int16_t*)&val));
            //int16_t newval2 = (*((int16_t*)&val2));
            
            samples1[j] = val1;
            samples2[j] = val2;
            pos+=2;
        }
            
        encode(c, encframe, encpkt, f);
        
        data_size = fread(inbuf, 1, 1152 * 4, fin);
    }

    //encode(c, NULL, encpkt, f);

    fclose(fin);

    //encodeterm();
}

void encodeterm()
{
    /* flush the encoder */
    encode(c, NULL, encpkt, f);

    fclose(f);

    av_frame_free(&encframe);
    av_packet_free(&encpkt);
    avcodec_free_context(&c);
}

void demuxterm()
{
    if (audio_stream) {
        enum AVSampleFormat sfmt = audio_dec_ctx->sample_fmt;
        int n_channels = audio_dec_ctx->channels;
        const char *fmt;

        if (av_sample_fmt_is_planar(sfmt)) {
            const char *packed = av_get_sample_fmt_name(sfmt);
            printf("Warning: the sample format the decoder produced is planar "
                   "(%s). This example will output the first channel only.\n",
                   packed ? packed : "?");
            sfmt = av_get_packed_sample_fmt(sfmt);
            n_channels = 1;
        }

        if ((ret = get_format_from_sample_fmt(&fmt, sfmt)) < 0)
            goto end;

        printf("Play the output audio file with the command:\n"
               "ffplay -f %s -ac %d -ar %d %s\n",
               fmt, n_channels, audio_dec_ctx->sample_rate,
               audio_dst_filename);
    }

end:
    avcodec_free_context(&audio_dec_ctx);
    avformat_close_input(&fmt_ctx);
    if (audio_dst_file)
        fclose(audio_dst_file);
    av_frame_free(&frame);
}

extern __declspec(dllexport) int capture(char* filename, char* outfilename, char* tempname)
{
    tempfilename = tempname;
    demuxinit(filename, outfilename);
    encodeinit(outfilename);

    demuxbody();

    demuxterm();
    encodeterm();
}

extern __declspec(dllexport) int stopcapture()
{
    running = 0;
}

int main(int argc, char **argv)
{
   char* filename = argv[1];
   char* outfilename = argv[2];
   tempfilename = argv[3];

   capture(filename, outfilename, tempfilename);

    //This works (*=>pcm)
    //demuxfile(filename, outfilename);

    //decodefile(filename, outfilename);
    
    //Half works (squelsion)
    //encodefile(filename, outfilename);

    //muxfile(filename, outfilename);
    //transcodefile(filename, outfilename);

    //test();
}