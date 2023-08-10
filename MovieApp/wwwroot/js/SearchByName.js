let searchResultsDiv = document.getElementById('searchResults');
let searchInput = document.getElementById('searchName');
let t = type ? type : "a";
function filterS(searchTerm) {
    var url = '/Main/searched?t='+ t+ '&searchName=' + searchTerm;

    fetch(url)
        .then(function (response) {
            if (response.ok) {
                return response.json();
            }
            throw new Error('Network response was not ok.');
        })
        .then(function (data) {
            var filteredMovies = data.movies;
            var filteredSeries = data.series;

            displayResults(filteredMovies, filteredSeries);
        })
        .catch(function (error) {
            console.error('Error:', error);
        });
}

function displayResults(movies, series) {
    searchResultsDiv.innerHTML = `<h3 class="search-results">${searchInput.value} Results</h3>`;
    if ((movies.length === 0 && series.length === 0) || searchInput.value.length === 0) {
        searchResultsDiv.classList.add('d-none');
    } else {
        searchResultsDiv.classList.remove('d-none');

        movies.forEach(function (movie) {
            var movieDiv = document.createElement('div');
            movieDiv.innerHTML = `
        <img class="col-4" src="/images/movies/${movie.Poster}" alt="Movie Poster" />
        <p class="col-7 text-start">${movie.MovieName}</p>
    `;
            movieDiv.classList.add('row');
            movieDiv.classList.add('gap-1');
            //Click Event
            movieDiv.addEventListener('click', function () {
                if (playMovieUrl.split("/").length > 3) {
                    window.location.href = playMovieUrl + `?mid=${movie.MovieID}`;

                } else {
                    window.location.href = playMovieUrl + '/' + profileID + `?mid=${movie.MovieID}`;
                }
            });

            searchResultsDiv.appendChild(movieDiv);
        });

        series.forEach(function (serie) {
            var seriesDiv = document.createElement('div');
            seriesDiv.innerHTML = `
                <img class="col-4" src="/images/series/${serie.Poster}" alt="Series Poster" />
                <p class="col-7 text-start">${serie.SeriesName}</p>
                `;

            seriesDiv.classList.add('row');
            seriesDiv.classList.add('gap-1');
            seriesDiv.addEventListener('click', function () {
                if (playSeriesUrl.split("/").length > 3) {
                    window.location.href = playSeriesUrl + `?sid=${serie.SeriesID}`;

                } else {
                    window.location.href = playSeriesUrl + '/' + profileID + `?sid=${serie.SeriesID}`;
                }
            });
            searchResultsDiv.appendChild(seriesDiv);
        });
    }
}

searchInput.addEventListener('keyup', function (event) {
    var searchTerm = event.target.value;
    filterS(searchTerm);
});


document.addEventListener('click', () => {
    searchResultsDiv.classList.add('d-none');
});

function ManageList(id, movieID, url, e) {
    var listIcon = e.querySelector('.bi');
    var buttonText = e.querySelector('span');

    var isAdded = listIcon.classList.contains('bi-trash');

    $.ajax({
        url: url,
        method: 'POST',
        data: { id: id, movieID: movieID },
        success: function () {
            listIcon.classList.toggle('bi-trash', !isAdded);
            listIcon.classList.toggle('bi-patch-plus', isAdded);

            if (isAdded) {
                buttonText.textContent = 'Add To My List';
            } else {
                buttonText.textContent = 'Remove From List';
            }
        },
        error: function () {
        }
    });
}
function ManageListSeries(id, movieID, url, e) {
    var listIcon = e.querySelector('.bi');
    var buttonText = e.querySelector('span');

    var isAdded = listIcon.classList.contains('bi-trash');

    $.ajax({
        url: url,
        method: 'POST',
        data: { id: id, sid: movieID },
        success: function () {
            listIcon.classList.toggle('bi-trash', !isAdded);
            listIcon.classList.toggle('bi-patch-plus', isAdded);

            if (isAdded) {
                buttonText.textContent = 'Add To My List';
            } else {
                buttonText.textContent = 'Remove From My List';
            }
        },
        error: function () {
        }
    });
}


