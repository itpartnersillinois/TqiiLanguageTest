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

let globalSelectedId = 'info_1_1';

function inputKeyPress(event) {
    if (event.code == 'Backspace' || event.code == 'ArrowLeft') {
        if (this.value != '') {
            this.value = '';
        } else {
            let previousItem = this.previousElementSibling;
            if (previousItem != null) {
                previousItem.focus();
                previousItem.value = '';
            }
        }
        event.preventDefault();
    } else if (event.code == 'ArrowUp' || event.code == 'ArrowDown') {
        var letters = document.querySelector('.letters');
        if (letters.classList.contains('popup')) {
            letters.classList.remove('popup');
            letters.classList.add('popup-disabled');
        }
        document.querySelector('.letters button').focus();
    }
    else if (!(event.code.startsWith('Delete') || event.code.startsWith('Arrow') || event.code.startsWith('Tab') || event.code.startsWith('Alt') || event.code.startsWith('Numpad') || event.code.startsWith('Control') || event.code.startsWith('Shift'))) {
        addLetter(this, event.key, false, true, false);
        event.preventDefault();
    }
}

function inputFocus(event) {
    let chosenValue = document.getElementById(globalSelectedId);
    if (chosenValue != null) {
        chosenValue.classList.remove('selected');
    }
    globalSelectedId = this.id;
    this.classList.add('selected');
}

function letterButton(event) {
    let chosenValue = document.getElementById(globalSelectedId);
    var letters = document.querySelector('.letters');
    if (letters.classList.contains('popup-disabled')) {
        letters.classList.remove('popup-disabled');
        letters.classList.add('popup');
        addLetter(chosenValue, this.innerText, true, true, true);
    } else {
        chosenValue.classList.remove('selected');
        addLetter(chosenValue, this.innerText, true, false, true);
    }
}

function letterArrowPress(event) {
    if (event.code == 'ArrowLeft' || (event.code == 'Tab' && event.shiftKey)) {
        let previousItem = this.previousElementSibling;
        if (previousItem != null) {
            previousItem.focus();
        }
        event.preventDefault();
    } else if (event.code == 'ArrowRight' || event.code == 'Tab') {
        let nextItem = this.nextElementSibling;
        if (nextItem != null) {
            nextItem.focus();
        }
        event.preventDefault();
    } else if (event.code == 'ArrowUp') {
        let nextRow = this.parentElement.previousElementSibling;
        if (nextRow != null && nextRow.classList.contains('button-row')) {
            nextRow.querySelector('button').focus();
        }
        event.preventDefault();
    } else if (event.code == 'ArrowDown') {
        let nextRow = this.parentElement.nextElementSibling;
        if (nextRow != null && nextRow.classList.contains('button-row')) {
            nextRow.querySelector('button').focus();
        }
        event.preventDefault();
    } else if (event.code == 'Backspace') {
        let chosenValue = document.getElementById(globalSelectedId);
        clearLetter(chosenValue);
        event.preventDefault();
    } else if (event.code == 'Escape') {
        let chosenValue = document.getElementById(globalSelectedId);
        chosenValue.focus();
    } else {
        let chosenItem = document.querySelector('.letters button[data-key="' + event.key + '"]');
        if (chosenItem != null) {
            chosenItem.focus();
            let chosenValue = document.getElementById(globalSelectedId);
            addLetter(chosenValue, chosenItem.innerText, false, false, true);
        }
    }
}

function addLetter(input, letter, overwriteLastLetter, focusOnLetter, useStandardKeyboard) {
    let nextItem = input.nextElementSibling;
    if (nextItem == null) {
        let nextRow = input.parentElement.nextElementSibling;
        if (nextRow != null) {
            nextItem = nextRow.querySelector('input');
        }
    }
    if (nextItem == null) {
        let nextRow = input.parentElement.parentElement.nextElementSibling;
        if (nextRow != null) {
            nextItem = nextRow.querySelector('input');
        }
    }
    if (nextItem != null) {
        input.value = letter;
        if (focusOnLetter) {
            nextItem.focus();
        } else {
            globalSelectedId = nextItem.id;
            nextItem.classList.add('selected');
            input.classList.remove('selected');
        }
    } else if (useStandardKeyboard) {
        input.value = letter;
    } else if (letter == '' || letter == ' ') {
        input.value = '';
    } else if (input.value == '' || overwriteLastLetter) {
        input.value = letter;
        input.focus();
    }
}

function clearLetter(input) {
    let previousItem = input.previousElementSibling;
    if (previousItem == null) {
        let previousRow = input.parentElement.previousElementSibling;
        if (previousRow != null) {
            previousItems = previousRow.querySelectorAll('input');
            previousItem = previousItems[previousItems.length - 1];
        }
    }
    if (previousItem == null) {
        let previousRow = input.parentElement.parentElement.previousElementSibling;
        if (previousRow != null) {
            previousItems = previousRow.querySelectorAll('input');
            previousItem = previousItems[previousItems.length - 1];
        }
    }
    if (previousItem != null) {
        input.value = '';
        globalSelectedId = previousItem.id;
        previousItem.classList.add('selected');
        input.classList.remove('selected');
    } else {
        input.value = '';
    }
}

window.addEventListener("DOMContentLoaded", (event) => {
    window.addEventListener("beforeunload", beforeUnloadHandler);
    window.addEventListener("pagehide", beforeUnloadHandler);
    testUserId.value = params.get('id');
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
        };
    }, 1000);

    document.getElementById('continue').addEventListener('click', event => {
        window.removeEventListener("beforeunload", beforeUnloadHandler);
        window.removeEventListener("pagehide", beforeUnloadHandler);
        displayAfterRecording.classList.add('hidden');
        document.querySelectorAll('.interactive span.question').forEach(s => {
            s.appendChild(document.createTextNode('['));
            s.querySelectorAll('input').forEach(i => {
                if (s.id != 'answer') {
                    s.appendChild(document.createTextNode(i.value));
                }
            });
            s.appendChild(document.createTextNode(']'));
            s.querySelectorAll('select').forEach(i => {
                s.innerHTML = '[' + i.value + ']';
            });
        });

        answerText.value = displayAfterRecording.innerText;
        document.forms[0].submit();
    });
    document.querySelectorAll('.interactive input').forEach(i => {
        i.addEventListener('focus', inputFocus);
        i.addEventListener('keydown', inputKeyPress);
    });
    document.querySelectorAll('.letters button').forEach(i => {
        i.addEventListener('click', letterButton);
        i.addEventListener('keydown', letterArrowPress);
    });
    var letters = document.querySelector('.letters');
    if (!letters.classList.contains('popup-disabled') && !letters.classList.contains('popup')) {
        document.querySelector('.letters button').focus();
    }
});