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
            timerElement.classList.add('background');
        }
        else if (seconds < 10) {
            seconds = '0' + seconds;
        }

        timerElement.innerText = minutes + ':' + seconds;

        if (minutes == 0 && seconds == 0) {
            clearInterval(myInterval);
            displayAfterRecording.classList.add('hidden');
        };
    }, 1000);

    document.getElementById('continue').addEventListener('click', event => {
        displayAfterRecording.classList.add('hidden');
        document.querySelectorAll('.interactive span').forEach(s => {
            s.querySelectorAll('input').forEach(i => {
                s.appendChild(document.createTextNode(i.value));
            });
            s.querySelectorAll('select').forEach(i => {
                s.innerHTML = i.value;
            });
        });

        answerText.value = displayAfterRecording.innerText;
        document.forms[0].submit();
    });
    document.querySelectorAll('.interactive input').forEach(i => {
        i.addEventListener('keydown', keyPress)
    });
});

function keyPress(event) {
    if (event.code.startsWith('Key')) {
        this.value = event.key;
        let nextItem = this.nextElementSibling;
        if (nextItem != null) {
            nextItem.focus();
        }
        event.preventDefault();
    } else if (event.code == 'Backspace' || event.code == 'ArrowLeft') {
        this.value = '';
        let previousItem = this.previousElementSibling;
        if (previousItem != null) {
            previousItem.focus();
            previousItem.value = '';
        }
        event.preventDefault();
    }
}