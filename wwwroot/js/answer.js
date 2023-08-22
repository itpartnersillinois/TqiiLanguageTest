const timerElement = document.getElementById('countdown');
const testUserId = document.getElementById('id');
const answerText = document.getElementById('answertext');
const params = new URLSearchParams(window.location.search);
const displayAfterRecording = document.getElementById('display-after-recording');

window.addEventListener("DOMContentLoaded", (event) => {
    testUserId.value = params.get('id');

    let timer = timerElement.innerText;
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
            document.forms[0].submit();
        })
    });
});