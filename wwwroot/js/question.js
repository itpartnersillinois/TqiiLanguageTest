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
});

function playSample() {
    let audioPlayer = new Audio();
    audioPlayer.id = "audio";
    audioPlayer.src = '/Media/' + question.innerText;
    audioPlayer.controls = false;
    audioPlayer.addEventListener("canplaythrough", (event) => {
        setTimeout(function () { audioPlayer.play(); }, 2000);
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