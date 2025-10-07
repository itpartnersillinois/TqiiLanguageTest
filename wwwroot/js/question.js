const timerElement = document.getElementById('countdown');
const manual_link = document.getElementById('manual_link');
const question = document.getElementById('question');
const route = document.getElementById('route');
const params = new URLSearchParams(window.location.search);
const beforeUnloadHandler = (event) => {
    event.preventDefault();
    event.returnValue = "not available";
    return "not available";
};

window.addEventListener("DOMContentLoaded", (event) => {
    window.addEventListener("beforeunload", beforeUnloadHandler);
    window.addEventListener("pagehide", beforeUnloadHandler);
    document.getElementById('manual_link').setAttribute('href', `/${route.innerText}?id=${params.get('id')}`);
    document.getElementById('manual_link').addEventListener('click', event => {
        window.removeEventListener("beforeunload", beforeUnloadHandler);
        window.removeEventListener("pagehide", beforeUnloadHandler);
    });
    document.getElementById('continue_link').setAttribute('href', `/${route.innerText}?id=${params.get('id')}`);
    document.getElementById('continue_link').addEventListener('click', event => {
        window.removeEventListener("beforeunload", beforeUnloadHandler);
        window.removeEventListener("pagehide", beforeUnloadHandler);
    });

    if (manual_link.style.display == 'none') {
        playSample();
    }
    if (timerElement != null && timerElement.innerText && timerElement.innerText != "0") {
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
                document.getElementById('continue_link').click();
            };
        }, 1000);
    }
});

function playSample() {
    let audioPlayer = new Audio();
    audioPlayer.id = "audio";
    audioPlayer.src = '/Media/' + question.innerText;
    audioPlayer.controls = false;
    audioPlayer.setAttribute("style", "position: absolute; top: 150px; right: 10px;");
    audioPlayer.addEventListener("canplaythrough", (event) => {
        setTimeout(function () {
            var promise = audioPlayer.play();
            if (promise !== undefined) {
                promise.then(_ => {
                    // Autoplay started, do nothing
                }).catch(error => {
                    console.log('Autoplay prevented');
                    audioPlayer.controls = true;
                    let playButton = document.createElement("div");
                    playButton.innerText = "Autoplay has been disabled -- please click the 'play' button on the control to the right to start audio.";
                    playButton.style.fontWeight = 'bold';
                    let instructions = document.getElementsByClassName('instruction')[0];
                    instructions.appendChild(playButton);
                    // Autoplay was prevented.
                    // Show a "Play" button so that user can start playback.
                });
            }
        }, 2000);
    });
    audioPlayer.addEventListener("ended", (event) => {
        console.debug('item ended');
        if (document.getElementById('continue_link').style.display == 'none') {
            window.removeEventListener("beforeunload", beforeUnloadHandler);
            window.removeEventListener("pagehide", beforeUnloadHandler);
            window.location.href = `/${route.innerText}?id=${params.get('id')}`;
        }
    });
    document.body.appendChild(audioPlayer);
}