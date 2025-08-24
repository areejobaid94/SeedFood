import {Injectable} from '@angular/core';
import * as RecordRTC from 'recordrtc';
import {DomSanitizer} from '@angular/platform-browser';
import * as lamejs from 'lamejs';


@Injectable()
export class RecordRTCService {
    /**
     * NOTE: if your are upload the file on server then you change your according
     * UPLOAD ON SERVER @function stopRTC write your code
     */
    SendButton = false;
    blobUrl: any;
    interval;
    recordingTimer: string;
    recordWebRTC: any;
    mediaRecordStream: any;
    options: any = {
        type: 'audio',
        mimeType: 'audio/webm'
    };
    recordingFile;

    constructor(
        private sanitizer: DomSanitizer
    ) {
    }

    /**
     * @function toggleRecord
     * check recording base on `recordingTimer`
     * getting permission on `mediaDevices` audio
     */
    toggleRecord(message) {
        if (this.recordingTimer) {
            this.stopRTC(message);
            this.SendButton = true;
        } else {
            navigator.mediaDevices.getUserMedia({audio: true}).then(stream => {
                this.startRTC(stream);
                this.SendButton = false;
            }).catch(error => {
                alert(error);
            });
        }
    }

    /**
     * @param stream
     * @name recordWebRTC set recording `stream` and `options`
     * @var blobUrl set null UI update
     * @see startCountdown()
     */
    startRTC(stream: any) {
        this.recordWebRTC = new RecordRTC.StereoAudioRecorder(stream, this.options);
        this.recordingFile;
        this.mediaRecordStream = stream;
        this.blobUrl = null;
        this.recordWebRTC.record();
        this.startCountdown();
    }

    /**
     * @function stopRTC
     * after `stop` recordWebRTC function getting blob
     * blob file making to blob url `blobUrl`
     * @name startCountdown stop counting with stream
     */
    stopRTC(message) {
        this.recordWebRTC.stop((blob) => {
            //NOTE: upload on server
            this.blobUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(blob));
            this.recordWebRTC.name += '.wav';
            this.recordingFile = new File([blob], this.recordWebRTC.name, {
                type: this.recordWebRTC.blob.type,
                lastModified: Date.now()
            });

            message.value = this.recordingFile;
            this.startCountdown(true);
        });
    }

    /**
     * @function stopRTC New
     * after click `stop` recordWebRTC function getting blob
     * blob file making to blob url `blobUrl`
     * @name startCountdown stop counting with stream
     */
    async clickStopRTC() {
        await new Promise((resolve => {
            this.recordWebRTC.stop((blob) => {

                //NOTE: upload on server
                this.blobUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(blob));
                this.recordWebRTC.name += '.mp3';
                this.recordingFile = new File([blob], this.recordWebRTC.name, {
                    type: 'audio/mp3',
                    lastModified: Date.now()
                });
                this.startCountdown(true);
                // this.convertWavToMp3(this.recordingFile);
                resolve(this.recordingFile);
            });
        }));

    }
    async convertWavToMp3(blob) {
        const arrayBuffer = await blob.arrayBuffer();
        const buffer = new Int16Array(arrayBuffer);
    
        let mp3encoder = new lamejs.Mp3Encoder(1, 44100, 128); // Mono channel, 44100 Hz, 128 kbps
        let mp3Data = [];
        let samples = buffer.length;
        let sampleBlockSize = 1152; // can be anything but make it a multiple of 576 to make encoders life easier
    
        for (let i = 0; i < samples; i += sampleBlockSize) {
          let sampleChunk = buffer.subarray(i, i + sampleBlockSize);
          let mp3buf = mp3encoder.encodeBuffer(sampleChunk);
          if (mp3buf.length > 0) {
              mp3Data.push(new Int8Array(mp3buf));
          }
        }
    
        let mp3buf = mp3encoder.flush();   // finish writing mp3
    
        if (mp3buf.length > 0) {
            mp3Data.push(new Int8Array(mp3buf));
        }
    
        let mp3Blob = new Blob(mp3Data, {type: 'audio/mp3'});
        return mp3Blob;
    }

    /**
     * @param clearTime default value `false`
     * `false` miens recording start if getting `true` then we are stop counting `clearStream`
     * Maximum Recoding time `10`Minutes @see minutes == 10
     */
    startCountdown(clearTime = false) {
        if (clearTime) {
            this.clearStream(this.mediaRecordStream);
            this.recordWebRTC = null;
            this.recordingTimer = null;
            this.mediaRecordStream = null;
            clearInterval(this.interval);
            return;
        } else {
            this.recordingTimer = `00:00`;
            clearInterval(this.interval);
        }

        this.interval = setInterval(() => {
            let timer: any = this.recordingTimer;
            timer = timer.split(':');
            let minutes = +timer[0];
            let seconds = +timer[1];

            if (minutes == 10) {
                this.recordWebRTC.stopRecording();
                clearInterval(this.interval);
                return;
            }
            ++seconds;
            if (seconds >= 59) {
                ++minutes;
                seconds = 0;
            }

            if (seconds < 10) {
                this.recordingTimer = `0${minutes}:0${seconds}`;
            } else {
                this.recordingTimer = `0${minutes}:${seconds}`;
            }
        }, 1000);
    }

    /**
     * @param stream clear stream Audio also video
     */
    clearStream(stream: any) {
        try {
            stream.getAudioTracks().forEach(track => track.stop());
            stream.getVideoTracks().forEach(track => track.stop());
        } catch (error) {
            //stream error
        }
    }

    clearRecordedData() {
        this.blobUrl = '';
    }

}
