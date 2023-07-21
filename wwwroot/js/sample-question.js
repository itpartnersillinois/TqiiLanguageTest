const play = document.getElementById('play');
play.addEventListener("click", playSample);
const information = document.getElementById('information');
const continueButton = document.getElementById('continue');

function playSample() {
    play.disabled = true;
    let audioPlayer = new Audio();
    audioPlayer.id = "audio";
    audioPlayer.src = '/ogg/sample-clip.ogg';
    audioPlayer.controls = false;
    audioPlayer.addEventListener("canplaythrough", (event) => {
        audioPlayer.play();
    });
    audioPlayer.addEventListener("ended", (event) => {
        console.debug('item ended');
        window.location.href = '/SampleAnswer';
    });
    document.body.appendChild(audioPlayer);
}