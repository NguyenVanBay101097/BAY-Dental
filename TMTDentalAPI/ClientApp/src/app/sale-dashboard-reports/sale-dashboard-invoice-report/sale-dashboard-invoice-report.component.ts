import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ChartDataset, ChartOptions } from 'chart.js';
import 'chartjs-adapter-date-fns';
import { vi } from 'date-fns/locale';

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
  barChartLabels: string[] = [];

  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      title: {
        text: 'BIỂU ĐỒ DOANH THU - THỰC THU',
        display: true,
        font: {
          size: 16
        }
      },
      legend: {
        position: 'bottom'
      },
      tooltip: {
        mode: 'index'
      }
    },
    scales: {
      x: {
        adapters: {
          date: {
            locale: vi
          }
        },
        offset: true,
        type: 'time',
        time: {
          tooltipFormat: 'dd/MM/yyyy',
          displayFormats: {
            'day': 'dd/MM',
            'month': 'MM/yyyy',
          },
          unit: 'day'
        }
      },
      y: {
        beginAtZero: true,
      },
    },
  };

  public barChartData: ChartDataset[] = [
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

  constructor() { }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.revenues && !changes.revenues.firstChange && changes.cashBooks && !changes.cashBooks.firstChange) {
      //sort asc
      var revenueData = this.revenues.sort(function (a, b) { return new Date(a.invoiceDate).getTime() - new Date(b.invoiceDate).getTime() })
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

      this.barChartData[0].data = revenueData.map(x => x.y);

      var cashBookData = this.cashBooks.sort(function (a, b) { return new Date(a.date).getTime() - new Date(b.date).getTime() })
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

      this.barChartData[1].data = cashBookData.map(x => x.y);
      this.barChartLabels = cashBookData.map(x => x.x);

      this.barChartOptions.scales.x['time'].unit = this.timeUnit;
      this.barChartOptions.scales.x['time'].tooltipFormat = this.timeUnit == 'day' ? 'dd/MM/yyyy' : 'MM/yyyy';
    }
  }
}
