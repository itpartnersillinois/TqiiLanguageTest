if (navigator.mediaDevices.getUserMedia) {
    console.log('getUserMedia supported.');

    let chunks = [];

    let onSuccess = function (stream) {
        const mediaRecorder = new MediaRecorder(stream);
        mediaRecorder.stop();

        const information = document.getElementById('information');
        const continueButton = document.getElementById('continue');
        const record = document.getElementById('record');
        record.addEventListener("click", recording);

        function recording() {
            if (mediaRecorder.state == "inactive") {
                mediaRecorder.start();
                continueButton.classList.add('hidden');
                record.innerText = "Stop Recording";
                information.innerText = "Recording now -- press stop for playback";
            } else {
                mediaRecorder.stop();
                record.innerText = "Record";
                record.enabled = false;
                information.innerText = "Listen for playback and either continue or re-record";
            }
        }

        mediaRecorder.ondataavailable = (e) => {
            chunks.push(e.data);
        };

        mediaRecorder.onstop = (e) => {
            const blob = new Blob(chunks, { type: "audio/ogg; codecs=opus" });
            let audioPlayer = new Audio();
            audioPlayer.id = "audio";
            audioPlayer.src = window.URL.createObjectURL(blob);
            audioPlayer.controls = false;
            audioPlayer.addEventListener("canplaythrough", (event) => {
                audioPlayer.play();
            });
            audioPlayer.addEventListener("ended", (event) => {
                record.enabled = true;
                chunks = [];
                continueButton.classList.remove('hidden');
                console.debug('item ended');
            });
            document.body.appendChild(audioPlayer);
        }
    }

    navigator.mediaDevices.getUserMedia({ audio: true }).then(onSuccess);
}