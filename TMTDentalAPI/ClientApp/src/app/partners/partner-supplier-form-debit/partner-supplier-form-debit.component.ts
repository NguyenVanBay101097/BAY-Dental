import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AccountCommonPartnerReportItemDetail, AccountCommonPartnerReportSearch, AccountCommonPartnerReportService, AccountMoveBasic } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PartnerSupplierFormDebitPaymentDialogComponent } from '../partner-supplier-form-debit-payment-dialog/partner-supplier-form-debit-payment-dialog.component';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-supplier-form-debit',
  templateUrl: './partner-supplier-form-debit.component.html',
  styleUrls: ['./partner-supplier-form-debit.component.css']
})
export class PartnerSupplierFormDebitComponent implements OnInit {
  id: string;
  skip = 0;
  countPayment = 0;
  limit = 10;
  gridData: GridDataResult;
  details: AccountMoveBasic[];
  loading = false;
  selectableSettings = {
    model: 'multiple', enabled: true, checkboxOnly: true
  }
  rowsSelected: any[] = [];
  search: string;
  searchUpdate = new Subject<string>();
  constructor(
    private reportService: AccountCommonPartnerReportService,
    private activeRoute: ActivatedRoute,
    private authService: AuthService,
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    if (this.id) {
      this.loadDataFromApi();

      this.searchUpdate.pipe(
        debounceTime(400),
        distinctUntilChanged())
        .subscribe(() => {
          this.loadDataFromApi();
        });

    }
  }

  exportExcelFile() {
    var val = new AccountCommonPartnerReportSearch();
    val.partnerId = this.id;
    val.search = this.search ? this.search : '';
    val.companyId = this.authService.userInfo.companyId;
    this.partnerService.exportExcelFileNCC(val).subscribe((res) => {
      let filename = "CongNoNCC";
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

  loadDataFromApi() {
    var val = new AccountCommonPartnerReportSearch();
    val.partnerId = this.id;
    val.search = this.search ? this.search : '';
    val.companyId = this.authService.userInfo.companyId;
    this.reportService.getListReportPartner(val).subscribe(
      res => {
        this.details = res;
        this.loadItems();
        this.loading = false;
      }
    )
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();
  }

  loadItems(): void {
    this.gridData = {
      data: this.details.slice(this.skip, this.skip + this.limit),
      total: this.details.length
    };
  }

  onPayment() {
    if (this.details && this.details.length <= 0) {
      this.notificationService.show({
        content: 'Bạn không còn hóa đơn để thanh toán',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return;
    }
    var val = {
      partnerId: this.id,
      companyId: this.authService.userInfo.companyId,
      invoiceIds: this.rowsSelected.map(x => x.id)
    }
    this.partnerService.GetDefaultPayment(val).subscribe(result => {
      let modalRef = this.modalService.open(PartnerSupplierFormDebitPaymentDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.rowsSelected = this.rowsSelected;
      modalRef.componentInstance.defaultVal = result;
      modalRef.result.then(() => {
        this.loadDataFromApi();
        this.rowsSelected = [];
        this.countPayment = 0;
      }, () => {
      });
    })

  }

  chooseRows(events: any[]) {
    this.rowsSelected = [];
    if (events && events.length > 0) {
      events.forEach(element => {
        var model = this.details.find(x => x.id == element);
        if (model) {
          this.rowsSelected.push(model);
        }
      });
    }
    this.countPayment = this.rowsSelected.length;
  }

}
