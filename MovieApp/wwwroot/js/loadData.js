    $("#loader").hide();
    var skip = 20;
    var take = 10;
function debounce(func, delay) {
    let timer;
    return function () {
        clearTimeout(timer);
        timer = setTimeout(func, delay);
    };
}

$(window).scroll(debounce(function () {
    var tolerance = 10;

    var windowHeight = $(window).height();
    var documentHeight = $(document).height();
    var scrollTop = $(window).scrollTop();
    if (scrollTop + windowHeight >= documentHeight - tolerance) {
        loadMoreMovies();
    }
}, 300))

function loadMoreMovies() {
    var url =`/Main/${action}/${profileID}?skip=${skip}&take=${take}`;

    fetch(url)
        .then(function (response) {
            if (response.ok) {
                return response.text();
            }
            throw new Error('Network response was not ok.');
        })
        .then(function (responseText) {
            if (responseText.trim() !== '') {
                var newMovies = $(responseText).hide();
                $("#movieListContainer").append(newMovies);
                newMovies.slideDown("slow");
                skip += take;
            }
        })
        .catch(function (error) {
            console.error('Error:', error);
        })
        .finally(function () {
            $("#loader").hide();
        });
}