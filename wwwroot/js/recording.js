const timerElement = document.getElementById('countdown');
const testUserId = document.getElementById('id');
const fileObject = document.getElementById('file');
const displayAfterRecording = document.getElementById('display-after-recording');
const params = new URLSearchParams(window.location.search);
const beforeUnloadHandler = (event) => {
    event.preventDefault();
    event.returnValue = "not available";
    return "not available";
};

window.addEventListener("DOMContentLoaded", (event) => {
    if (navigator.mediaDevices.getUserMedia) {
        testUserId.value = params.get('id');
        window.addEventListener("beforeunload", beforeUnloadHandler);
        window.addEventListener("pagehide", beforeUnloadHandler);
        console.log('getUserMedia supported.');

        let chunks = [];

        let onSuccess = function (stream) {
            const mediaRecorder = new MediaRecorder(stream);
            const timerElement = document.getElementById('countdown');

            mediaRecorder.start();
            let timer = timerElement.innerText;
            if (isNaN(timer)) {
                timer = 0;
            }
            if (timer < 20) {
                timerElement.classList.add('background');
            }
            let minutes = Math.floor(timer / 60);
            let seconds = timer % 60;
            if (seconds < 10) {
                seconds = '0' + seconds;
            }
            timerElement.innerText = minutes + ':' + seconds;
            displayAfterRecording.classList.remove('hidden');

            let audioPlayer = new Audio();
            audioPlayer.id = "audio";
            audioPlayer.src = '/ogg/beep.flac';
            audioPlayer.controls = false;
            audioPlayer.addEventListener("canplaythrough", (event) => {
                audioPlayer.play();
            });

            mediaRecorder.ondataavailable = (e) => {
                chunks.push(e.data);
            };

            document.getElementById('endrecording').addEventListener('click', event => {
                mediaRecorder.stop();
            });

            mediaRecorder.onstop = (e) => {
                window.removeEventListener("beforeunload", beforeUnloadHandler);
                window.removeEventListener("pagehide", beforeUnloadHandler);
                const blob = new Blob(chunks, { type: "audio/ogg; codecs=opus" });
                console.debug('item ended');
                var fd = new FormData();
                fd.append('file', blob, "answer.wav");
                fd.append('id', params.get('id'));
                fd.append('answerguid', document.getElementById('answerguid').value);
                fetch('/Recording', {
                    method: 'POST',
                    body: fd
                }).then(function (response) {
                    window.removeEventListener("beforeunload", beforeUnloadHandler);
                    window.location.href = "/Question?id=" + params.get('id');
                });
            }

            const myInterval = setInterval(function () {
                let timer = timerElement.innerText.split(':');
                let minutes = timer[0];
                let seconds = timer[1];
                seconds -= 1;
                if (seconds < 0 && minutes != 0) {
                    minutes -= 1;
                    seconds = 59;
                }
                else if (seconds == 20 && minutes == 0) {
                    timerElement.classList.add('background');
                }
                else if (seconds < 10) {
                    seconds = '0' + seconds;
                }

                timerElement.innerText = minutes + ':' + seconds;

                if (minutes == 0 && seconds == 0) {
                    clearInterval(myInterval);
                    mediaRecorder.stop();
                };
            }, 1000);
        }

        navigator.mediaDevices.getUserMedia({ audio: true }).then(onSuccess);
    }
});