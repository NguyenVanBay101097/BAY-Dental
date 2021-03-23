import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountCommonPartnerReportService, AccountCommonPartnerReportSearch, AccountCommonPartnerReportItem } from '../account-common-partner-report.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PartnerSimple, PartnerPaged } from 'src/app/partners/partner-simple';
import { IntlService } from '@progress/kendo-angular-intl';
import { PartnerService } from 'src/app/partners/partner.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { aggregateBy } from '@progress/kendo-data-query';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-account-common-customer-report-list',
  templateUrl: './account-common-customer-report-list.component.html',
  styleUrls: ['./account-common-customer-report-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class AccountCommonCustomerReportListComponent implements OnInit {

  loading = false;
  items: AccountCommonPartnerReportItem[] = [];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  dateFrom: Date;
  dateTo: Date;
  searchPartner: PartnerSimple;
  resultSelection: string;
  public total: any;

  search: string;
  searchUpdate = new Subject<string>();

  public aggregates: any[] = [
    { field: 'end', aggregate: 'sum' },
  ];

  constructor(private reportService: AccountCommonPartnerReportService, private intlService: IntlService,
    private route: ActivatedRoute,
    private partnerService: PartnerService) { }

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());


  ngOnInit() {
    var date = new Date();
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

      this.route.queryParamMap.subscribe(params => {
        this.resultSelection = params.get('result_selection');
        this.loadDataFromApi();
      });
  
  }

  searchChangeDate(value: any) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AccountCommonPartnerReportSearch();
    val.fromDate = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.toDate = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.partnerId = this.searchPartner ? this.searchPartner.id : null;
    val.search = this.search ? this.search : '';
    val.resultSelection = this.resultSelection;

    this.reportService.getSummary(val).subscribe(res => {
      this.items = res;
      this.loadItems();
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.items.slice(this.skip, this.skip + this.limit),
      total: this.items.length
    };
  }

  onExport() {
    var val = new AccountCommonPartnerReportSearch();
    val.fromDate = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : null;
    val.toDate = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : null;
    val.partnerId = this.searchPartner ? this.searchPartner.id : null;
    val.search = this.search ? this.search : '';
    val.resultSelection = this.resultSelection;

    this.reportService.ExportExcelFile(val).subscribe((res: any) => {
      const filename = this.resultSelection == 'customer'? `BaoCaoCongNoKhachHang` : 'BaoCaoCongNoNCC';
      const newBlob = new Blob([res], {
        type:
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      });

      const data = window.URL.createObjectURL(newBlob);
      const link = document.createElement('a');
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  Sum(field) : any{
    if(this.items.length == 0 ) 
    {
      var a = {};
      a[field] = {sum: 0};
      return a;
    } else {
      return aggregateBy(this.items, [ { aggregate: "sum", field: field }]);
    }
  }
}
