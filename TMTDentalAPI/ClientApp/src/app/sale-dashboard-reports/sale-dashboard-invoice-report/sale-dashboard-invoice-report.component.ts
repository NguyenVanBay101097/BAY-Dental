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
  @Input() timeUnit: any;
  @Input() revenueActualReportData: any;
  @Input() saleRevenueCashBookData: any[] = [];
  barChartLabels: any[] = [];

  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      title: {
        text: 'BIỂU ĐỒ TIỀN ĐIỀU TRỊ - DOANH THU - THỰC THU',
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
      label: 'Tiền điều trị',
      data: [],
      backgroundColor: 'rgba(235, 59, 91, 1)',
      hoverBackgroundColor: 'rgba(235, 59, 91, 0.8)',
      hoverBorderColor: 'rgba(235, 59, 91, 1)'
    },
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
    if (changes.saleRevenueCashBookData && !changes.saleRevenueCashBookData.firstChange) {
      this.barChartData[0].data = this.saleRevenueCashBookData.map(x => x.totalSaleAmount);
      this.barChartData[1].data = this.saleRevenueCashBookData.map(x => x.totalRevenueAmount);
      this.barChartData[2].data = this.saleRevenueCashBookData.map(x => x.totalLiquidityAmount);
      this.barChartLabels = this.saleRevenueCashBookData.map(x => new Date(x.date));
      this.barChartOptions.scales.x['time'].unit = this.timeUnit;
      this.barChartOptions.scales.x['time'].tooltipFormat = this.timeUnit == 'day' ? 'dd/MM/yyyy' : 'MM/yyyy';
    }
  }
}
