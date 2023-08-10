// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Bar Chart
var ctx = document.getElementById("myBarChart");
/*var jsondata = JSON.parse(sessionStorage.getItem("TotalSeriesYear"));*/

var jsondata = JSON.parse(sessionStorage.getItem("TotalObject"));
var totalYears = [];
var totalMoviesCount = [];
var totalSeriesCount = [];
for (let i = 0; i < jsondata.Years.length; i++) {
    totalYears.push(jsondata.Years[i]);
    totalMoviesCount.push(jsondata.Movies.Count[i]);
    totalSeriesCount.push(jsondata.Series.Count[i]);
}


var myBarChart = new Chart(ctx, {
  type: 'bar',
    data: {
        labels: totalYears,
    datasets: [{
      label: "Series",
      backgroundColor: "#4e73df",
      hoverBackgroundColor: "#2e59d9",
      borderColor: "#4e73df",
        data: totalSeriesCount,
    }, {
        label: "Movies",
        backgroundColor: "rgb(238, 130, 238)",
        hoverBackgroundColor: "#2e59d9",
        borderColor: "rgb(238, 130, 238)",
        data: totalMoviesCount,
    }],
  },
  options: {
    maintainAspectRatio: false,
    layout: {
      padding: {
        left: 10,
        right: 25,
        top: 25,
        bottom: 0
      }
    },
    scales: {
      xAxes: [{
        time: {
          unit: 'year'
        },
        gridLines: {
          display: false,
          drawBorder: false
        },
        ticks: {
          maxTicksLimit: 6
        },
      }],
      yAxes: [{
        ticks: {
          maxTicksLimit: 5,
          padding: 10,
        },
        gridLines: {
          color: "rgb(234, 236, 244)",
          zeroLineColor: "rgb(234, 236, 244)",
          drawBorder: false,
          borderDash: [2],
          zeroLineBorderDash: [2]
        }
      }],
    },
    legend: {
      display: true
    },
    tooltips: {
      titleMarginBottom: 10,
      titleFontColor: '#6e707e',
      titleFontSize: 14,
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      caretPadding: 10,
    },
  }
});
