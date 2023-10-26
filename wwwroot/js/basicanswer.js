const timerElement = document.getElementById('countdown');
const testUserId = document.getElementById('id');
const answerText = document.getElementById('answertext');
const answerText1 = document.getElementById('answertext1');
const answerText2 = document.getElementById('answertext2');
const answerText3 = document.getElementById('answertext3');
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
            timerElement.classList.add('background');
        }
        else if (seconds < 10) {
            seconds = '0' + seconds;
        }

        timerElement.innerText = minutes + ':' + seconds;

        if (minutes == 0 && seconds == 0) {
            clearInterval(myInterval);
            displayAfterRecording.classList.add('hidden');
            displayAfterRecording.classList.add('hidden');
        };
    }, 1000);

    document.querySelectorAll('.buttons a').forEach(b => {
        b.addEventListener('click', event => {
            window.removeEventListener("beforeunload", beforeUnloadHandler);
            window.removeEventListener("pagehide", beforeUnloadHandler);

            if (event.target.alt != null && event.target.alt != '') {
                answerText.value = event.target.alt;
            } else {
                answerText.value = event.target.innerHTML;
            }
            if (document.getElementsByName('a1').length > 0) {
                answerText1.value = document.querySelector('input[name="a1"]:checked').value;
            }
            if (document.getElementsByName('a2').length > 0) {
                answerText2.value = document.querySelector('input[name="a2"]:checked').value;
            }
            if (document.getElementsByName('a3').length > 0) {
                answerText3.value = document.querySelector('input[name="a3"]:checked').value;
            }
            document.forms[0].submit();
        })
    });
});