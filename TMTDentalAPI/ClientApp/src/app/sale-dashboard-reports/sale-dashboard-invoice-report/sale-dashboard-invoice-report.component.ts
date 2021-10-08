import { AfterViewInit, Component, ElementRef, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-sale-dashboard-invoice-report',
  templateUrl: './sale-dashboard-invoice-report.component.html',
  styleUrls: ['./sale-dashboard-invoice-report.component.css']
})
export class SaleDashboardInvoiceReportComponent implements OnInit, OnChanges {
  @Input() revenues: any[];
  @Input() timeUnit: any;
  @Input() cashBooks: any;
  @Input() revenueActualReportData: any;

  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    title: {
      text: 'BIỂU ĐỒ DOANH THU - THỰC THU',
      display: true,
      fontSize: 16,
    },
    legend: {
      position: 'bottom'
    },
    tooltips: {
      mode: 'index',
      callbacks: {
        label: function (tooltipItems, data) {
          return data.datasets[tooltipItems.datasetIndex].label + ': ' + tooltipItems.yLabel.toLocaleString();
        }
      }
    },
    // We use these empty structures as placeholders for dynamic theming.
    scales: {
      xAxes: [{
        offset: true,
        distribution: 'linear',
        type: 'time',
        time: {
          tooltipFormat: 'DD/MM/YYYY',
          displayFormats: {
            'day': 'DD/MM',
            'month': 'MM/YYYY',
          },
          unit: 'day'
        }
      }],
      yAxes: [{
        ticks: {
          beginAtZero: true,
        },
      }],
    },
  };

  public barChartData: ChartDataSets[] = [
    {
      label: 'Doanh thu',
      data: [],
      backgroundColor: 'rgba(35, 149, 255, 1)',
      hoverBackgroundColor: 'rgba(35, 149, 255, 0.8)',
      hoverBorderColor: 'rgba(35, 149, 255, 1)'
    },
    {
      label: 'Thực thu',
      data: [],
      backgroundColor: 'rgba(40, 167, 69, 1)',
      hoverBackgroundColor: 'rgba(40, 167, 69, 0.8)',
      hoverBorderColor: 'rgba(40, 167, 69, 1)'
    }
  ];

  @ViewChild(BaseChartDirective, { static: true }) private chart;

  constructor(private intlService: IntlService) { }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.revenues && !changes.revenues.firstChange && changes.cashBooks && !changes.cashBooks.firstChange) {
      //sort asc
      var revenueData = this.revenues.sort(function(a, b) { return new Date(a.invoiceDate).getTime() - new Date(b.invoiceDate).getTime() })
      .map(item => {
        return {
          x: new Date(item.invoiceDate),
          y: item.priceSubTotal
        }
      });

      this.cashBooks.forEach(item => {
        var d = new Date(item.date);
        if (revenueData.filter(r => r.x.getTime() == d.getTime()).length == 0) {
          var index = revenueData.findIndex(r => r.x.getTime() > d.getTime());
          if (index != -1) {
            revenueData.splice(index, 0, {
              x: d,
              y: 0
            });
          } else {
            revenueData.push({
              x: d,
              y: 0
            });
          }
        }
      });

      this.barChartData[0].data = revenueData;

      var cashBookData = this.cashBooks.sort(function(a, b) { return new Date(a.date).getTime() - new Date(b.date).getTime() })
      .map(item => {
        return {
          x: new Date(item.date),
          y: item.totalThu
        }
      });

      this.revenues.forEach(item => {
        var d = new Date(item.invoiceDate);
        if (cashBookData.filter(r => r.x.getTime() == d.getTime()).length == 0) {
          var index = cashBookData.findIndex(r => r.x.getTime() > d.getTime());
          if (index != -1) {
            cashBookData.splice(index, 0, {
              x: d,
              y: 0
            });
          } else {
            cashBookData.push({
              x: d,
              y: 0
            });
          }
        }
      });

      this.barChartData[1].data = cashBookData;

      this.barChartOptions.scales.xAxes[0].time.unit = this.timeUnit;
      this.barChartOptions.scales.xAxes[0].time.tooltipFormat = this.timeUnit == 'day' ? 'DD/MM/YYYY' : 'MM/YYYY';
      setTimeout(() => {
        this.chart.refresh();
      }, 10);
    }
  }
}
