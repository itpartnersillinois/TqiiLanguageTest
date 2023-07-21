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
        information.innerText = "Listen for playback and either continue or change speaker settings";
    });
    audioPlayer.addEventListener("ended", (event) => {
        let audio = document.getElementById("audio");
        audio.remove();
        play.disabled = false;
        continueButton.classList.remove('hidden');
        console.debug('item ended');
    });
    document.body.appendChild(audioPlayer);
}