import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { IntlService } from '@progress/kendo-angular-intl';
import { ReportSource, ReportPartnerSourceSearch } from "./../report-partner-sources.service";
import { ReportPartnerSourcesService } from '../report-partner-sources.service';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PartnerSourceSimple } from 'src/app/partners/partner-simple';
import { PartnerSourcePaged, PartnerSourceService } from 'src/app/partner-sources/partner-source.service';

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

  // Pie
  public pieData: any[] = [];

  constructor(private intlService: IntlService, private reportPartnerSource: ReportPartnerSourcesService) { 
    this.labelContent = this.labelContent.bind(this);
  }

  ngOnInit() {
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
      this.pieData.push({category: this.reportResults[i].name || 'Chưa xác định', value:this.reportResults[i].countPartner , percentage: Math.round(this.reportResults[i].countPartner / this.reportResults[i].totalPartner * 100) })
     };
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
