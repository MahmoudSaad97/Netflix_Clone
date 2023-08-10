// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Area Chart
var ctx = document.getElementById("myAreaChart");
var jsondata = JSON.parse(sessionStorage.getItem("paymentObject"));
var totalDays = [];
var totalRevenues = [];
for (let i = 0; i < jsondata.Days.length; i++) {
    totalDays.push(jsondata.Days[i]);
    totalRevenues.push(jsondata.Revenue[i]);
}

var myLineChart = new Chart(ctx, {
  type: 'line',
  data: {
      labels: totalDays,
    datasets: [{
      label: "Revenue",
      lineTension: 0.3,
      backgroundColor: "rgba(78, 115, 223, 0.05)",
      borderColor: "rgba(78, 115, 223, 1)",
      pointRadius: 3,
      pointBackgroundColor: "rgba(78, 115, 223, 1)",
      pointBorderColor: "rgba(78, 115, 223, 1)",
      pointHoverRadius: 3,
      pointHoverBackgroundColor: "rgba(78, 115, 223, 1)",
      pointHoverBorderColor: "rgba(78, 115, 223, 1)",
      pointHitRadius: 10,
      pointBorderWidth: 2,
      data: totalRevenues,
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
          type: 'time',
      xAxes: [{
        time: {
              unit: 'day',
        },
        gridLines: {
          display: false,
          drawBorder: false
        },
        ticks: {
          maxTicksLimit: 15
        }
      }],
      yAxes: [{
        ticks: {
          maxTicksLimit: 5,
              padding: 10,
              callback: function (value) {
                  return '$' + value;
              }
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
      backgroundColor: "rgb(255,255,255)",
      bodyFontColor: "#858796",
      titleMarginBottom: 10,
      titleFontColor: '#6e707e',
      titleFontSize: 14,
      borderColor: '#dddfeb',
      borderWidth: 1,
      xPadding: 15,
      yPadding: 15,
      displayColors: false,
      intersect: false,
      mode: 'index',
        caretPadding: 10,
        callbacks: {
            label: function (tooltipItems, data) {
                var i = tooltipItems.index;
                return data.datasets[0].data[i] + " $";
            }
        }
    }
  }
});
