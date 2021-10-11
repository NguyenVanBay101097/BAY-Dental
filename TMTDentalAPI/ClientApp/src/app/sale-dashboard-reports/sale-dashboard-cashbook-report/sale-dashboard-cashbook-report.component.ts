import { Component, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { IntlService, load } from '@progress/kendo-angular-intl';
import { CashBookReportFilter, CashBookReportItem, CashBookService } from 'src/app/cash-book/cash-book.service';
import { ChartDataSets, ChartOptions, ChartType } from 'chart.js';
import { BaseChartDirective, Label, SingleDataSet } from 'ng2-charts';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-sale-dashboard-cashbook-report',
  templateUrl: './sale-dashboard-cashbook-report.component.html',
  styleUrls: ['./sale-dashboard-cashbook-report.component.css']
})
export class SaleDashboardCashbookReportComponent implements OnInit {
  @Input() cashBooks: any;
  @Input() timeUnit: any;
  @Input() thuChiReportData: any;

  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    title: {
      text: 'BIỂU ĐỒ THU CHI',
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
        },
        // ticks: {
        //   maxTicksLimit: this.maxTicks
        // }
      }],
      yAxes: [{
        ticks: {
          beginAtZero: true,
        }
      }],
    },
  };

  public barChartData: ChartDataSets[] = [
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

  @ViewChild(BaseChartDirective, { static: true }) private chart;


  constructor(private router: Router) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.cashBooks && !changes.cashBooks.firstChange) {
      
      this.barChartData[0].data = this.cashBooks.map(item => {
        return {
          x: new Date(item.date),
          y: item.totalThu
        }
      });

      this.barChartData[1].data = this.cashBooks.map(item => {
        return {
          x: new Date(item.date),
          y: item.totalChi
        }
      });

      this.barChartOptions.scales.xAxes[0].time.unit = this.timeUnit;
      this.barChartOptions.scales.xAxes[0].time.tooltipFormat = this.timeUnit == 'day' ? 'DD/MM/YYYY' : 'MM/YYYY';
      setTimeout(() => {
        this.chart.refresh();
      }, 10);
    }
  }

  ngOnInit() {
  }

  redirectTo() {
    return this.router.navigateByUrl("cash-book/tab-cabo");
  }
}