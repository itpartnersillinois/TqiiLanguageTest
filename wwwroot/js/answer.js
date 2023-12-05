const timerElement = document.getElementById('countdown');
const testUserId = document.getElementById('id');
const answerText = document.getElementById('answertext');
const params = new URLSearchParams(window.location.search);
const displayAfterRecording = document.getElementById('display-after-recording');
const beforeUnloadHandler = (event) => {
    event.preventDefault();
    event.returnValue = "not available";
    return "not available";
};

window.addEventListener("DOMContentLoaded", (event) => {
    testUserId.value = params.get('id');
    window.addEventListener("beforeunload", beforeUnloadHandler);
    window.addEventListener("pagehide", beforeUnloadHandler);

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
            document.getElementById('countdown').classList.add('background');
        }
        else if (seconds < 10) {
            seconds = '0' + seconds;
        }

        document.getElementById('countdown').innerText = minutes + ':' + seconds;

        if (minutes == 0 && seconds == 0) {
            clearInterval(myInterval);
            window.removeEventListener("beforeunload", beforeUnloadHandler);
            window.removeEventListener("pagehide", beforeUnloadHandler);
            document.forms[0].submit();
        };
    }, 1000);

    document.querySelectorAll('.buttons a').forEach(b => {
        b.addEventListener('click', event => {
            if (event.target.alt != null && event.target.alt != '') {
                answerText.value = event.target.alt;
            } else {
                answerText.value = event.target.innerHTML;
            }
            window.removeEventListener("beforeunload", beforeUnloadHandler);
            window.removeEventListener("pagehide", beforeUnloadHandler);
            document.forms[0].submit();
        })
    });

    let audioPlayer = new Audio();
    audioPlayer.id = "audio";
    audioPlayer.src = '/ogg/beep.flac';
    audioPlayer.controls = false;
    audioPlayer.addEventListener("canplaythrough", (event) => {
        audioPlayer.play();
    });
});