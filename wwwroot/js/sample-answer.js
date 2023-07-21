const timerElement = document.getElementById('countdown');

setInterval(function () {
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
        document.getElementById('countdown').classList.add('background');
    }
    else if (seconds < 10) {
        seconds = '0' + seconds;
    }

    document.getElementById('countdown').innerText = minutes + ':' + seconds;

    if (minutes == 0 && seconds == 0) {
        document.location.href = '/SampleRecording';
    };
}, 1000);

document.querySelectorAll('.buttons a').forEach(b => {
    b.addEventListener('click', event => {
        console.log(event.target.innerText);
        document.location.href = '/SampleRecording';
    })
});