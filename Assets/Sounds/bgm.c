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
    char filename[] = "bgm.wav";

    WAVEHEADER *wavHead = (WAVEHEADER*)malloc(sizeof(WAVEHEADER));

    DWORD samplingRate = 44100;
    DWORD bitsize = 16;
    DWORD msec = 400;
    DWORD kaisuu = 16;

    strncpy(wavHead->RIFF, "RIFF", 4);
    wavHead->size = (int)(msec*kaisuu / 1000.0 * samplingRate * bitsize / 8 + sizeof(WAVEHEADER) - sizeof(wavHead->RIFF) - sizeof(wavHead->size));
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
    wavHead->dataSize = (int)(msec*kaisuu / 1000.0 * samplingRate * bitsize / 8);

    FILE *output = fopen(filename, "wb");
    if(output == NULL) {
        fprintf(stderr, "Error: ファイルをオープンできませんでした。%s\n", filename);
        exit(1);
    }


    // ヘッダ書き出し
    fwrite(wavHead, sizeof(WAVEHEADER), 1, output);

    DWORD numOfSample = (int)(msec*kaisuu / 1000.0 * samplingRate);
    DWORD hz1 = 442;
    DWORD hz2= 884;
    DWORD hz = (int)((hz1 + hz2) / 2);
    const float omega = 2 * M_PI * hz;
    float cycle = 0.6;  // 音程が上がる/下がる周期
    float s;
    short out;

    short *wav = (short*)malloc(sizeof(short) * numOfSample);

    int flag = 0;
    float time = 0;
    int elapsedTime = 1;

    // 音声データ書き出し
    for(DWORD i = 0; i < numOfSample; i++) {
        time = (float)i / samplingRate;
        float x = 2 * (hz2 - hz1)/2 * M_PI; // 2 * pi * hz
        //float y = 2 * 1/cycle/*hz*/ * M_PI; // 2 * pi * hz, 音の上下する周期
        float y = 2 * 1/(msec/1000.0) * M_PI;
        s = 0xFFFF * 0.10 * sin(omega * time + x/y * sin(y * time));    // FM変調
        out = (short)s;
        wav[i] = out;
    }

    fwrite(wav, sizeof(short) * numOfSample, 1, output);


    fclose(output);
    free(wav);
    free(wavHead);

    exit(0);
}
