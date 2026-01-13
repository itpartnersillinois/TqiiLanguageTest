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
                information.classList.add('hidden');
                continueButton.classList.add('hidden');
                record.innerText = "Stop Recording";
                information.innerText = "Recording now -- press stop for playback";
            } else {
                mediaRecorder.stop();
                record.innerText = "Record";
                record.enabled = false;
                information.innerHTML = "<div class='ils-input-disclaimer'><label for='continue-checkbox'>By clicking the checkbox, you confirm you can hear your recording. If you cannot hear your recording, please re-enable microphone permissions and test your microphone again.</label><input id='continue-checkbox' onclick='enablebutton();' type='checkbox'></div>";
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
                information.classList.remove('hidden');
                console.debug('item ended');
            });
            document.body.appendChild(audioPlayer);
        }
    }

    navigator.mediaDevices.getUserMedia({ audio: true }).then(onSuccess);
}

function enablebutton() {
    document.getElementById('continue').classList.remove('hidden');
}