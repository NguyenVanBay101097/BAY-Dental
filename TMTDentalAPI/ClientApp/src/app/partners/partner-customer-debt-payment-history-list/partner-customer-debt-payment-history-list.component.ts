import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { PhieuThuChiPaged, PhieuThuChiService } from 'src/app/phieu-thu-chi/phieu-thu-chi.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { PartnerService } from '../partner.service';
import { PrintService } from "src/app/shared/services/print.service";

@Component({
  selector: 'app-partner-customer-debt-payment-history-list',
  templateUrl: './partner-customer-debt-payment-history-list.component.html',
  styleUrls: ['./partner-customer-debt-payment-history-list.component.css']
})
export class PartnerCustomerDebtPaymentHistoryListComponent implements OnInit {
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
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor( private intlService: IntlService,
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private phieuthuchiService: PhieuThuChiService,
    private router: Router,
    private printService: PrintService,
    private route: ActivatedRoute,
    private notifyService: NotifyService,) { }

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
  }

  loadDataFromApi(){
    this.loading = true;
    var paged = new PhieuThuChiPaged();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.partnerId = this.partnerId;
    paged.accountType = 'customer_debt';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.phieuthuchiService.getPaged(paged).pipe(
      map((response: any) => (<GridDataResult>{
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

  printItem(item) {
    this.phieuthuchiService.getPrint2(item.id).subscribe((data: any) => {
      this.printService.printHtml(data.html);
    });
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa thanh toán công nợ';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa thanh toán công nợ ?';
    modalRef.result.then(() => {
      this.phieuthuchiService.actionCancel([item.id]).subscribe(() =>{
        this.phieuthuchiService.delete(item.id).subscribe(() => {
          this.notifyService.notify('success','Xóa thành công');
          this.loadDataFromApi();
        }, () => {
        });
      })
    }, () => {
    });
  }

}
