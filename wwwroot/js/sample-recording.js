window.addEventListener("DOMContentLoaded", (event) => {
    if (navigator.mediaDevices.getUserMedia) {
        console.log('getUserMedia supported.');

        let chunks = [];

        let onSuccess = function (stream) {
            const mediaRecorder = new MediaRecorder(stream);
            const information = document.getElementById('information');
            const timerElement = document.getElementById('countdown');

            mediaRecorder.start();

            mediaRecorder.ondataavailable = (e) => {
                chunks.push(e.data);
            };

            document.getElementById('endrecording').addEventListener('click', event => {
                mediaRecorder.stop();
            });

            mediaRecorder.onstop = (e) => {
                // const blob = new Blob(chunks, { type: "audio/ogg; codecs=opus" });
                console.debug('item ended');
                information.classList.remove('hidden');
            }

            const myInterval = setInterval(function () {
                let timer = timerElement.innerText.split(':');
                let minutes = 0;
                let seconds = 0;
                if (timer.length == 1) {
                    if (timer[0] < 20) {
                        timerElement.classList.add('background');
                        timerElement.classList.remove('hidden');
                    }
                    minutes = Math.floor(timer[0] / 60);
                    seconds = timer[0] % 60;
                } else {
                    minutes = timer[0];
                    seconds = timer[1];
                }
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