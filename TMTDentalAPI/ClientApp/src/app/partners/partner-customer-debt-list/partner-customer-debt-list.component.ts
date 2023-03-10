import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AmountCustomerDebtFilter, CustomerDebtFilter, CustomerDebtReportService } from 'src/app/core/services/customer-debt-report.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PartnerCustomerDebtPaymentDialogComponent } from '../partner-customer-debt-payment-dialog/partner-customer-debt-payment-dialog.component';
import { AuthService } from './../../auth/auth.service';

@Component({
  selector: 'app-partner-customer-debt-list',
  templateUrl: './partner-customer-debt-list.component.html',
  styleUrls: ['./partner-customer-debt-list.component.css']
})
export class PartnerCustomerDebtListComponent implements OnInit {
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  partnerId: string;
  search: string;
  limit = 20;
  offset = 0;
  pagerSettings: any;
  edit = false;
  dateFrom: Date;
  dateTo: Date;
  loading = false;
  debtStatistics: any;

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(private intlService: IntlService,
    private modalService: NgbModal,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private customerDebtReportService: CustomerDebtReportService,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }


  ngOnInit() {
    this.partnerId = this.route.parent.parent.snapshot.paramMap.get('id');
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.offset = 0;
        this.loadDataFromApi();
      });


    this.loadDataFromApi();
    this.loadAmountDebtTotal();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new CustomerDebtFilter();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.partnerId = this.partnerId;
    paged.search = this.search || '';
    paged.companyId = this.authService.userInfo.companyId;
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.customerDebtReportService.getReports(paged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;

      this.loading = false;
    }, err => {
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.limit = event.take;
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.offset = 0;
    this.loadDataFromApi();
  }

  loadAmountDebtTotal() {
    var val = new AmountCustomerDebtFilter();
    val.partnerId = this.partnerId;
    val.companyId = this.authService.userInfo.companyId;
    this.customerDebtReportService.getAmountDebtTotal(val).subscribe((rs: any) => {
      this.debtStatistics = rs;
    });
  }


  createItem() {
    if(this.debtStatistics.balanceTotal <= 0) {
      return this.notifyService.notify('error', 'B???n kh??ng c?? c??ng n??? ????? thanh to??n');
    }

    const modalRef = this.modalService.open(PartnerCustomerDebtPaymentDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thu c??ng n??? kh??ch h??ng';
    modalRef.componentInstance.type = 'thu';
    modalRef.componentInstance.partnerId = this.partnerId;
    modalRef.componentInstance.accountType = "customer_debt";
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Thanh to??n th??nh c??ng');
      this.loadDataFromApi();
      this.loadAmountDebtTotal();
    }, er => { })
  }



  getForm(item) {
    if(item.type == "debit"){
      this.router.navigate(['/sale-orders', item.id]);
    }else{
      this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { id: item.id } });
    }
    
  }

  exportExcelFile() {
    var paged = new CustomerDebtFilter();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.partnerId = this.partnerId;
    paged.search = this.search || '';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.customerDebtReportService.exportExcelFile(paged).subscribe((res) => {
      let filename = "S??? c??ng n??? kh??ch h??ng";

      let newBlob = new Blob([res], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

}
