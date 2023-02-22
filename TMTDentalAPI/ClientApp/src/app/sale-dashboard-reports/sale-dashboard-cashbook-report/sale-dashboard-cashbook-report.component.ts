import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { ChartDataset, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-sale-dashboard-cashbook-report',
  templateUrl: './sale-dashboard-cashbook-report.component.html',
  styleUrls: ['./sale-dashboard-cashbook-report.component.css']
})
export class SaleDashboardCashbookReportComponent implements OnInit {
  @Input() cashBooks: any = [];
  @Input() timeUnit: any;
  @Input() thuChiReportData: any;
  barChartLabels: string[] = [];

  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      title: {
        text: 'BIỂU ĐỒ THU CHI',
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
        type: 'time',
        time: {
          tooltipFormat: 'dd/MM/yyyy',
          displayFormats: {
            'day': 'dd/MM',
            'month': 'MM/yyyy',
          },
          unit: 'day'
        },
      },
      y: {
        beginAtZero: true,
      },
    },
  };

  public barChartData: ChartDataset[] = [
    {
      label: 'Thu',
      data: [],
      backgroundColor: 'rgba(35, 149, 255, 1)',
      hoverBackgroundColor: 'rgba(35, 149, 255, 0.8)',
      hoverBorderColor: 'rgba(35, 149, 255, 1)',
    },
    {
      label: 'Chi',
      data: [],
      backgroundColor: 'rgba(40, 167, 69, 1)',
      hoverBackgroundColor: 'rgba(40, 167, 69, 0.8)',
      hoverBorderColor: 'rgba(40, 167, 69, 1)'
    }
  ];

  constructor(private router: Router) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.cashBooks && !changes.cashBooks.firstChange) {
      this.barChartLabels = this.cashBooks.map(item => {
        return new Date(item.date)
      });

      this.barChartData[0].data = this.cashBooks.map(item => {
        return item.totalThu;
      });

      this.barChartData[1].data = this.cashBooks.map(item => {
        return item.totalChi
      });

      this.barChartOptions.scales.x['time'].unit = this.timeUnit;
      this.barChartOptions.scales.x['time'].tooltipFormat = this.timeUnit == 'day' ? 'dd/MM/yyyy' : 'MM/yyyy';
    }
  }

  ngOnInit() {
  }

  redirectTo() {
    return this.router.navigateByUrl("cash-book/tab-cabo");
  }
}