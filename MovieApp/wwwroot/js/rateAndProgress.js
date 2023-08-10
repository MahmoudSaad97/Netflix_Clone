video.addEventListener('loadedmetadata', function () {
    if (progress) {
        if (progress < 100) {
            const totalDuration = video.duration;
            if (!isNaN(totalDuration) && totalDuration > 0) {
                video.currentTime = Math.floor((progress / 100) * totalDuration);
            }

        } else {
            video.currentTime = 0;
        }
    }
});

if (progress) {
    if (progress < 100) {
        const totalDuration = video.duration;
        if (!isNaN(totalDuration) && totalDuration > 0) {
            video.currentTime = Math.floor((progress / 100) * totalDuration);
        }

    } else {
        video.currentTime = 0;
    }
}



if (exist !== 'True') {
    video.addEventListener('ended', function () {
        $('#ratingModal').modal('show');

    });
}

$('.rating-icon').click(function () {
    $('.rating-icon').addClass('bi-star').removeClass('bi-star-fill');

    $(this).addClass('bi-star-fill').removeClass('bi-star');
    $(this).prevAll('.rating-icon').addClass('bi-star-fill').removeClass('bi-star');

    var starValue = $(this).attr('data-value');
    ratingValue = starValue;
});

$('#ratingModal').on('hidden.bs.modal', function () {
    ratingValue = 0;
});

$('#sendrate').click(function () {
    if (ratingValue) {
        setMovieRate(ratingValue);
    }
    else {
        toastr.error('Please Set the Rate First or click Later to Skip ');
    }
});

function setMovieRate(ratingValue) {
    $.ajax({
        type: 'POST',
        url: `/Main/${method}`,
        data: {
            id: profileID,
            showid: showid,
            movieRate: ratingValue
        },
        success: function (response) {
            $('#ratingModal').modal('hide');
            toastr.success('Thank You For Rate');
        },
        error: function (error) {
            console.error('Error sending rating:', error);
        }
    });
}

const prog = document.querySelectorAll('.progress');
prog.forEach(ele => {
    ele.style.width = ele.dataset.prog + '%';
})

//function sendProgress(id, movieid, prog) {
//    $.ajax({
//        type: 'POST',
//        url: '/Main/movieProgress',
//        data: {
//            id: id,
//            mid: movieid,
//            prog: prog
//        },
//        success: function (response) {    
//            console.log('Progress sent successfully');
//        },
//        error: function (error) {
//            console.error('Error sending progress:', error);
//        }
//    });

//}

//function sendVideoProgress() {
//    const currentTime = Math.floor(video.currentTime);

//    if (!video.paused) {
//        sendProgress(profileID, showid, currentTime);
//    }
//}
//setInterval(sendVideoProgress, 3000);