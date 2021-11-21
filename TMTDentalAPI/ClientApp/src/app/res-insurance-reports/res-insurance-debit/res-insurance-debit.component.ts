import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { PartnerService } from 'src/app/partners/partner.service';
import { ResInsuranceService } from 'src/app/res-insurance/res-insurance.service';
import { AccountInvoiceRegisterPaymentDialogV2Component } from 'src/app/shared/account-invoice-register-payment-dialog-v2/account-invoice-register-payment-dialog-v2.component';
import { ResInsuranceCuDialogComponent } from 'src/app/shared/res-insurance-cu-dialog/res-insurance-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { ResInsuranceDebtPaymentDialogComponent } from '../res-insurance-debt-payment-dialog/res-insurance-debt-payment-dialog.component';
import { InsuranceDebtFilter, InsuranceDebtReport } from '../res-insurance-report.model';
import { ResInsuranceReportService } from '../res-insurance-report.service';

@Component({
  selector: 'app-res-insurance-debit',
  templateUrl: './res-insurance-debit.component.html',
  styleUrls: ['./res-insurance-debit.component.css']
})
export class ResInsuranceDebitComponent implements OnInit {
  dateFrom: Date;
  dateTo: Date;
  search: string;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  insuranceInfo: any;
  insuranceDebt: InsuranceDebtReport[];
  searchUpdate = new Subject<string>();
  id: string;
  selectedIds: string[] = [];

  constructor(
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private activeRoute: ActivatedRoute,
    private resInsuranceService: ResInsuranceService,
    private resInsuranceReportService: ResInsuranceReportService,
    private intlService: IntlService,
    private accountPaymentService: AccountPaymentService,
    private partnerService: PartnerService,
  ) { }

  ngOnInit(): void {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    if(this.id){
      this.getDisplayInsurance();
      this.loadInsuranceDebtReport();
    }  
  }

  onSearchUpdate(): void {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadInsuranceDebtReport();
      });
  }

  getDisplayInsurance(): void {
    this.resInsuranceService.getById(this.id).subscribe((res: any) => {
      this.insuranceInfo = res;
    })
  }

  editInsurance(): void {
    let modalRef = this.modalService.open(ResInsuranceCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Sửa công ty bảo hiểm';
    modalRef.componentInstance.id = this.insuranceInfo ? this.insuranceInfo.id : '';
    modalRef.result.then(() => {
      this.notifyService.notify("success", "Lưu thành công")
    }, () => { });
  }

  loadInsuranceDebtReport(){
    var val = new InsuranceDebtFilter();
    val.search = this.search || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    val.insuranceId = this.id;
    this.resInsuranceReportService.getInsuranceDebtReport(val).subscribe((res :any) => {
      this.insuranceDebt = res;
    })

  }

  onSearchDateChange(e): void {
    this.dateFrom = e.dateFrom || '';
    this.dateTo = e.dateTo || '';
    this.loadInsuranceDebtReport();
  }

  onPayment() {
      this.accountPaymentService.insurancePaymentDefaultGet(this.selectedIds).subscribe(rs2 => {
        let modalRef = this.modalService.open(ResInsuranceDebtPaymentDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Thu tiền bảo hiểm';
        modalRef.componentInstance.defaultVal = rs2;
        modalRef.componentInstance.partnerId = this.insuranceInfo.partnerId || '';
        modalRef.result.then(() => {       

          this.selectedIds = [];
          this.loadInsuranceDebtReport();
        }, () => {
        });
      })  
  }
}
