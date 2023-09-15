const play = document.getElementById('play');
const question = document.getElementById('question');
const route = document.getElementById('route');
const params = new URLSearchParams(window.location.search);

window.addEventListener("DOMContentLoaded", (event) => {
    document.getElementById('manual_link').setAttribute('href', `/${route.innerText}?id=${params.get('id')}`);
    play.addEventListener("click", playSample);
});

function playSample() {
    play.disabled = true;
    let audioPlayer = new Audio();
    audioPlayer.id = "audio";
    audioPlayer.src = '/Media/' + question.innerText;
    audioPlayer.controls = false;
    audioPlayer.addEventListener("canplaythrough", (event) => {
        audioPlayer.play();
    });
    audioPlayer.addEventListener("ended", (event) => {
        console.debug('item ended');
        window.location.href = `/${route.innerText}?id=${params.get('id')}`;
    });
    document.body.appendChild(audioPlayer);
}