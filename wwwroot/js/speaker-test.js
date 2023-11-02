const information = document.getElementById('information');
const continueButton = document.getElementById('continue');

window.addEventListener("DOMContentLoaded", (event) => {
    playSample();
});

function playSample() {
    let audioPlayer = new Audio();
    audioPlayer.id = "audio";
    audioPlayer.src = '/ogg/sample.wav';
    audioPlayer.controls = false;
    audioPlayer.addEventListener("canplaythrough", (event) => {
        audioPlayer.play();
        information.innerText = "Listen for playback and either continue or change speaker settings";
    });
    audioPlayer.addEventListener("ended", (event) => {
        let audio = document.getElementById("audio");
        audio.remove();
        continueButton.classList.remove('hidden');
        console.debug('item ended');
    });
    document.body.appendChild(audioPlayer);
}