// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Pie Chart
var ctx = document.getElementById("myPieChart");
var jsondata = JSON.parse(localStorage.getItem('jsondata'));
var totalUsers = parseInt(sessionStorage.getItem("TotalUserCount"))
    , totalMovies = parseInt(sessionStorage.getItem("TotalMoviesCount"))
    , totalSeries = parseInt(sessionStorage.getItem("TotalSeriesCount"));

var myPieChart = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: ["Movies", "Series", "Users"],
      datasets: [{
      data: [totalMovies, totalSeries, totalUsers],
      backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
      hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
      hoverBorderColor: "rgba(234, 236, 244, 1)",
    }],
  },
  options: {
    maintainAspectRatio: false,
      tooltips: {
      enabled: true,
      mode: 'single',
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      caretPadding: 10,
      callbacks: {
          label: function (tooltipItems, data) {
              var i = tooltipItems.index;
              return data.labels[i] + ": " + data.datasets[0].data[i] + " %";
          }
      } 
    },
    legend: {
      display: false
    },
    cutoutPercentage: 80,
  },
});
