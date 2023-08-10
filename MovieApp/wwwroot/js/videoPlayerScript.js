const video = document.getElementById("video");
const controls = document.getElementById("controls");
const play = document.getElementById("play");
const PlayPuse = document.getElementById("Play-Puse");
const volume = document.getElementById("volume");
const skipStart = document.getElementById("skip-start");
const skipEnd = document.getElementById("skip-end");
const back10 = document.getElementById("back-10");
const skip10 = document.getElementById("skip-10");
const subtitle = document.getElementById("subtitle");
const settings = document.getElementById("settings");
const similar = document.getElementById("similar");
const fullscreen = document.getElementById('fullscreen')
const similarItems = document.getElementById("similaritems");
const progressBar = document.querySelector('.progress-bar');
const current = document.querySelector('.current-time');
const end = document.querySelector('.end-time');
const volumeBar = document.querySelector('input[type="range"]');
const speedOptions = document.querySelector('.speed-options');
const videoplayer = document.querySelector('.vidoplayer');
const timeline = document.querySelector('.video-timeline');
const displayedData = document.querySelector('.displayed-data');
const waitloader = document.getElementById('waiting');

similar.addEventListener('click', () => {
    similarItems.classList.toggle("movelist");
})

play.addEventListener('click', () => {
    playPause();
    displayedData.classList.add('show');
});

PlayPuse.addEventListener('click', () => {
    playPause();
})

video.addEventListener('click', () => {
    playPause();
});

video.addEventListener('timeupdate', e => {
    let { currentTime, duration } = e.target;
    let percent = (currentTime / duration) * 100;
    progressBar.style.width = `${percent}%`
    current.innerText = formatTime(currentTime);
    end.innerText = formatTime(duration - currentTime);
});

video.addEventListener('waiting', () => {
    waitloader.style.display = 'flex';
});

video.addEventListener('playing', () => {
    waitloader.style.display = ' none';
})

volume.addEventListener('click', () => {
    if (!volume.classList.contains('bi-volume-up-fill')) {
        video.volume = 0.5;
        volume.classList.replace("bi-volume-mute-fill", "bi-volume-up-fill");
    } else {
        video.volume = 0;
        volume.classList.replace("bi-volume-up-fill", "bi-volume-mute-fill");
    }
    volumeBar.value = video.volume;
});

back10.addEventListener('click', () => {
    video.currentTime -= 1;
});

skip10.addEventListener('click', () => {
    video.currentTime += 1;
});

fullscreen.addEventListener('click', () => {
    FullScreenFun();
});

settings.addEventListener('click', () => {
    speedOptions.classList.toggle('hide');
});

speedOptions.querySelectorAll('li').forEach(o => {
    o.addEventListener('click', e => {
        video.playbackRate = o.dataset.speed;
        speedOptions.querySelector('.active').classList.remove("active");
        o.classList.add("active");
    })
});



document.addEventListener('keydown', (e) => {
    if (e.code === 'Space') {
        e.preventDefault();
        playPause();
        if (!speedOptions.classList.contains('hide')) {
            speedOptions.classList.add('hide');
        }

        if (similarItems.classList.contains('movelist')) {
            similarItems.classList.remove("movelist");
        }

        videoplayer.classList.add("show-controls");
        clearTimeout(timer);
        hideControls();
    }

    if (e.code === 'ArrowUp') {
        if (volume.classList.contains("bi-volume-mute-fill")) {
            volume.classList.replace("bi-volume-mute-fill", "bi-volume-up-fill");
        }
        if (video.volume < 1) {
            const newVolume = Math.min(1, video.volume + 0.05);
            video.volume = newVolume;
        }
        volumeBar.value = video.volume;
    }

    if (e.code === 'ArrowDown') {
        if (video.volume > 0) {
            const newVolume = Math.max(0, video.volume - 0.05); // Apply a minimum value of 0
            video.volume = newVolume;
            if (video.volume == 0) {
                if (volume.classList.contains("bi-volume-up-fill"))
                volume.classList.replace("bi-volume-up-fill", "bi-volume-mute-fill");
            }
        }
        volumeBar.value = video.volume;
    }

    if (e.code === 'KeyM') {
        video.muted = !video.muted;
        if (video.muted) {
            if (volume.classList.contains("bi-volume-up-fill"))
                volume.classList.replace("bi-volume-up-fill", "bi-volume-mute-fill");
        } else {
            if (volume.classList.contains("bi-volume-mute-fill"))
                volume.classList.replace("bi-volume-mute-fill", "bi-volume-up-fill");
        }
    }

});

document.addEventListener("fullscreenchange", () => {
    if (document.fullscreenElement) {

        fullscreen.classList.replace("bi-arrows-angle-expand", "bi-arrows-angle-contract");
        displayedData.classList.remove('show');

    }
    else {
        fullscreen.classList.replace("bi-arrows-angle-contract", "bi-arrows-angle-expand");
        if (!displayedData.classList.contains('show'))
            displayedData.classList.add('show');
    }
})

document.addEventListener('click', (e) => {
    if (e.target.className !== "bi bi-gear-fill" && !speedOptions.classList.contains('hide')) {
        speedOptions.classList.add('hide');
    }

    if (e.target.className !== "bi bi-card-list" && similarItems.classList.contains('movelist') && !e.target.closest(".similarMovies")) {
        similarItems.classList.remove("movelist");
    }
});

document.addEventListener('dblclick', () => {
    FullScreenFun();
});

timeline.addEventListener('click', (e) => {
    timelinewidth = timeline.clientWidth;
    video.currentTime = (e.offsetX / timelinewidth) * video.duration;
});

timeline.addEventListener('mousedown', () => {
    timeline.addEventListener('mousemove', brogressbardrag);
});

timeline.addEventListener('mousemove', (e) => {
    let time = timeline.querySelector('span');
    let offsetX = e.offsetX;
    time.style.left = `${offsetX}px`
    let timelinewidth = timeline.clientWidth;
    percent = (e.offsetX / timelinewidth) * video.duration;
    time.innerText = formatTime(percent);
})

videoplayer.addEventListener('mouseup', () => {
    timeline.removeEventListener('mousemove', brogressbardrag);
});

videoplayer.addEventListener('mousemove', () => {
    videoplayer.classList.add("show-controls");
    clearTimeout(timer);
    hideControls();
});

volumeBar.addEventListener('input', e => {
    video.volume = e.target.value;
    if (video.volume == 0) {
        volume.classList.replace("bi-volume-up-fill", "bi-volume-mute-fill");
    } else {
        volume.classList.replace("bi-volume-mute-fill", "bi-volume-up-fill");
    }
});

function playPause() {

    if (video.paused) {
        video.play();
        PlayPuse.classList.remove('bi-play-fill');
        PlayPuse.classList.add('bi-pause-fill');
    } else {
        video.pause();
        PlayPuse.classList.remove('bi-pause-fill');
        PlayPuse.classList.add('bi-play-fill');
    }

    if (!play.classList.contains('hide')) {
        play.classList.add('hide');
    }
    controls.classList.remove('hide');
}

function FullScreenFun() {
    if (document.fullscreenElement) {
        fullscreen.classList.replace("bi-arrows-angle-contract", "bi-arrows-angle-expand");
        if (!displayedData.classList.contains('show')) displayedData.classList.add('show');
        return document.exitFullscreen();
    }
    fullscreen.classList.replace("bi-arrows-angle-expand", "bi-arrows-angle-contract");
    videoplayer.requestFullscreen();
    displayedData.classList.remove('show');
}

function formatTime(time) {
    let seconds = Math.floor(time % 60);
    let minutes = Math.floor(time / 60) % 60;
    let hours = Math.floor(time / 3600);

    seconds = seconds < 10 ? `0${seconds}` : seconds;
    minutes = minutes < 10 ? `0${minutes}` : minutes;
    hours = hours < 10 ? `0${hours}` : hours;

    if (hours == 0)
        return `${minutes}:${seconds}`;

    return `${hours}:${minutes}:${seconds}`;
}

function brogressbardrag(e) {
    let timelinewidth = timeline.clientWidth;
    progressBar.style.width = `${e.offsetX}px`
    video.currentTime = (e.offsetX / timelinewidth) * video.duration;
}
let timer;
function hideControls() {
    if (video.paused) return;
    timer = setTimeout(() => {
        videoplayer.classList.remove("show-controls")
    }, 2000);
}

//check Mobile Screens
function IsMobile() {
    let regex = /android|iphone|kindle|ipad/i;
    return regex.test(navigator.userAgent);
}

