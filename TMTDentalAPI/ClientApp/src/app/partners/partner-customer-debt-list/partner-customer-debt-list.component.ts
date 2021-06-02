import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SaleOrderPaymentMethodFilter, SaleOrderPaymentService } from 'src/app/core/services/sale-order-payment.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PartnerCustomerDebtPaymentDialogComponent } from '../partner-customer-debt-payment-dialog/partner-customer-debt-payment-dialog.component';
import { PartnerService } from '../partner.service';

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
  edit = false;
  dateFrom: Date;
  dateTo: Date;
  loading = false;
  amountDebtTotal = 0;
  amountDebtPaidTotal = 0;
  amountDebtBalanceTotal = 0;


  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(private intlService: IntlService,
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private router: Router,
    private route: ActivatedRoute,
    private notifyService: NotifyService,
    private saleOrderPaymentService: SaleOrderPaymentService) { }


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
    this.loadAmountDebtPaidTotal();
    this.loadAmountDebtBalanceTotal();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new SaleOrderPaymentMethodFilter();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.partnerId = this.partnerId;
    paged.journalType = 'debt';
    paged.search = this.search || '';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.saleOrderPaymentService.getHistoryPaymentMethodPaged(paged).pipe(
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
    this.loadDataFromApi();
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.offset = 0;
    this.loadDataFromApi();
  }

  loadAmountDebtTotal() {
    this.partnerService.getAmountDebtTotal(this.partnerId).subscribe(rs => {
      this.amountDebtTotal = rs;
    });
  }

  loadAmountDebtPaidTotal() {
    this.partnerService.getAmountDebtPaidTotal(this.partnerId).subscribe(rs => {
      this.amountDebtPaidTotal = rs;
    });
  }

  loadAmountDebtBalanceTotal() {
    this.partnerService.getAmountDebtBalance(this.partnerId).subscribe(rs => {
      this.amountDebtBalanceTotal = rs;
    });
  }


  createItem() {
    const modalRef = this.modalService.open(PartnerCustomerDebtPaymentDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thu công nợ khách hàng';
    modalRef.componentInstance.type = 'thu';
    modalRef.componentInstance.partnerId = this.partnerId;
    modalRef.componentInstance.accountType = "customer_debt";
    modalRef.result.then(() => {
      this.notifyService.notify('success', 'Thanh toán thành công');
      this.loadDataFromApi();
      this.loadAmountDebtTotal();
      this.loadAmountDebtPaidTotal();
      this.loadAmountDebtBalanceTotal();
    }, er => { })
  }



  getFormSaleOrder(id) {
    this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
  }

  exportExcelFile() {
    var paged = new SaleOrderPaymentMethodFilter();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.partnerId = this.partnerId;
    paged.journalType = 'debt';
    paged.search = this.search || '';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");

    this.saleOrderPaymentService.exportCustomerDebtExcelFile(paged).subscribe((res) => {
      let filename = "Sổ công nợ khách hàng";

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
