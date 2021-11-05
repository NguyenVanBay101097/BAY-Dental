import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { ChartDataset, ChartOptions } from 'chart.js';
import * as pluginDataLabels from 'chartjs-plugin-datalabels';

@Component({
  selector: 'app-sale-dashboard-ap-cr-chart',
  templateUrl: './sale-dashboard-ap-cr-chart.component.html',
  styleUrls: ['./sale-dashboard-ap-cr-chart.component.css']
})
export class SaleDashboardApCrChartComponent implements OnInit {
  // Pie
  @Input() dataCustomer: any[] = [];
  @Input() dataNoTreatment: any[] = [];

  loading = false;
  limit = 20;
  skip = 0;
  total: number;
  state: string;
  pieDataCustomer: any[] = [];
  pieDataNoTreatment: any[] = [];
  pieCustomerLabels: string[] = [];

  pieCustomerOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      datalabels: {
        display: true,
        anchor: 'end',
        align: 'start',
        color: '#fff'
      },
      legend: {
        position: 'right',
      }
    }
  }

  pieCustomerDatasets: ChartDataset[] = [];

  pieCustomerPlugins = [pluginDataLabels];

  pieReceiptLabels: string[] = [];

  pieReceiptOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      datalabels: {
        display: true,
        anchor: 'end',
        align: 'start',
        color: '#fff'
      },
      legend: {
        position: 'right',
      }
    }
  }

  pieReceiptDatasets: ChartDataset[] = [];

  pieReceiptPlugins = [pluginDataLabels];

  constructor(
    private router: Router,
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadPieDataCustomer();
  }

  ngOnInit() {
    this.loadPieDataCustomer();
  }


  loadPieDataCustomer() {
    this.pieCustomerLabels = this.dataCustomer.map(x => x.displayIsNew);
    this.pieCustomerDatasets = [
      { 
        backgroundColor: ['#0066cc', '#99ccff'],
        data: this.dataCustomer.map(x => x.partnerTotal),
        hoverBackgroundColor: ['#0066cc', '#99ccff'],
        hoverBorderColor: ['#0066cc', '#99ccff'],
      }
    ];

    this.pieReceiptLabels = this.dataNoTreatment.map(x => x.name);
    this.pieReceiptDatasets = [
      { 
        backgroundColor: ['#0066cc', '#99ccff'],
        data: this.dataNoTreatment.map(x => x.countCustomerReceipt),
        hoverBackgroundColor: ['#0066cc', '#99ccff'],
        hoverBorderColor: ['#0066cc', '#99ccff'],
      }
    ];
  }

  redirectTo(value) {
    switch (value) {
      case 'partner-report-overview':
        this.router.navigateByUrl("report-account-common/partner-report-overview");
        break;
      case 'customer-receipt-overview':
        this.router.navigateByUrl("customer-receipt-reports/customer-receipt-overview");
        break;
      default:
        break;
    }
  }



}


