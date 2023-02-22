import { Component, OnInit } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { ChartDataset, ChartOptions } from 'chart.js';
import { ReportPartnerSourcesService } from '../report-partner-sources.service';
import { ReportPartnerSourceSearch, ReportSource } from "./../report-partner-sources.service";

@Component({
  selector: 'app-report-partner-source-list',
  templateUrl: './report-partner-source-list.component.html',
  styleUrls: ['./report-partner-source-list.component.css']
})
export class ReportPartnerSourceListComponent implements OnInit {
  loading = false;
  SourceId = '';
  reportResults: ReportSource[] = [];
  gridData: GridDataResult;
  ReportSourceNames : string;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  date: Date;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  // Pie
  public pieData: any[] = [];
  public pieChartLabels: string[] = [];
  public pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom'
      }
    }
  };
  public pieChartData: ChartDataset[] = [
    { 
      label: 'Khách hàng',
      data: [],
    }
  ];
  constructor(private intlService: IntlService, private reportPartnerSource: ReportPartnerSourcesService) { 
    this.labelContent = this.labelContent.bind(this);
  }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new ReportPartnerSourceSearch();
    if (this.dateFrom) {
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'd', 'en-US');
    }
    if (this.dateTo) {
      val.dateTo = this.intlService.formatDate(this.dateTo, 'd', 'en-US');
    }
   
    this.loading = true;
    this.reportPartnerSource.getReport(val).subscribe(result => {
      this.reportResults = result;
      this.pieData = [];
      this.loadItems();     
      this.loading = false;
    }, () => {
      this.loading = false;
    });
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadItems(): void {   
    for (let i = 0; i < this.reportResults.length; i++) {
      this.pieData.push({category: this.reportResults[i].name || 'Chưa xác định', value:this.reportResults[i].countPartner , percentage: (this.reportResults[i].countPartner / this.reportResults[i].totalPartner * 100).toFixed(2)})
     };
    this.pieChartLabels = this.pieData.map(x => x.category);
    this.pieChartData[0].data = this.pieData.map(x => x.value);
  }

  public labelContent(args: LegendLabelsContentArgs): string {
    return `${args.dataItem.category}: ${args.dataItem.value} khách hàng (${args.dataItem.percentage}%)`;
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
    
  }
}
