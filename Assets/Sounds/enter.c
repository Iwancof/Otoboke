#include<stdio.h>
#include<stdlib.h>
#include<string.h>
#include<math.h>

typedef unsigned char BYTE;
typedef unsigned short WORD;
typedef unsigned int DWORD;

typedef struct {
    char RIFF[4];
    DWORD size; // これ以降のファイルサイズ（ファイルサイズ-8）
    char WAVE[4];

    char fmtchunk[4];
    DWORD chunksize;
    WORD fmtID;
    WORD numofchannel;
    DWORD samplingRate; // kHz
    DWORD dataSpeed;    // Byte/sec
    WORD blockSize;     // Byte/sample*channel
    WORD bitperSample;  // bit/sample

    char data[4];
    DWORD dataSize;
} WAVEHEADER;

int main(void) {
    char filename[] = "enter.wav";

    WAVEHEADER *wavHead = (WAVEHEADER*)malloc(sizeof(WAVEHEADER));

    DWORD samplingRate = 44100;
    DWORD bitsize = 16;
    DWORD msec = 100;

    strncpy(wavHead->RIFF, "RIFF", 4);
    wavHead->size = (int)(msec / 1000.0 * samplingRate * bitsize / 8 + sizeof(WAVEHEADER) - sizeof(wavHead->RIFF) - sizeof(wavHead->size));
    strncpy(wavHead->WAVE, "WAVE", 4);
    strncpy(wavHead->fmtchunk, "fmt ", 4);
    wavHead->chunksize = (int)&wavHead->data - (int)&wavHead->fmtID;
    wavHead->fmtID = 1; // リニアPCM
    wavHead->numofchannel = 1;
    wavHead->samplingRate = samplingRate;
    wavHead->dataSpeed = samplingRate * (int)(bitsize/8);
    wavHead->blockSize = bitsize * wavHead->numofchannel;
    wavHead->bitperSample = bitsize;
    strncpy(wavHead->data, "data", 4);
    wavHead->dataSize = (int)(msec / 1000.0 * samplingRate * bitsize / 8);

    FILE *output = fopen(filename, "wb");
    if(output == NULL) {
        fprintf(stderr, "Error: ファイルをオープンできませんでした。%s\n", filename);
        exit(1);
    }


    // ヘッダ書き出し
    fwrite(wavHead, sizeof(WAVEHEADER), 1, output);

    DWORD numOfSample = (int)(msec / 1000.0 * samplingRate);
    DWORD hz = 1046;
    const float omega = 2 * M_PI * hz;
    float s;
    short out;
    float time = 0;

    short *wav = (short*)malloc(sizeof(short) * numOfSample);

    // 音声データ書き出し
    for(DWORD i = 0; i < numOfSample; i++) {
        time = (float)i / samplingRate;
        s = 0xFFFF * 0.25 * sin(omega * time);
        out = (short)s;
        wav[i] = out;
    }

    fwrite(wav, sizeof(short) * numOfSample, 1, output);


    fclose(output);
    free(wav);
    free(wavHead);

    exit(0);
}
