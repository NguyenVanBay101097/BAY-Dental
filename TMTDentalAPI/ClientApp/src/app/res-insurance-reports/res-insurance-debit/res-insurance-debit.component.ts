import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { aggregateBy } from '@progress/kendo-data-query';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { ResInsuranceService } from 'src/app/res-insurance/res-insurance.service';
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
  sumAmount: number = 0;

  constructor(
    private modalService: NgbModal,
    private notifyService: NotifyService,
    private resInsuranceReportService: ResInsuranceReportService,
    private intlService: IntlService,
    private accountPaymentService: AccountPaymentService,
    private activeRoute: ActivatedRoute,
    private resInsuranceService: ResInsuranceService,
  ) { }

  ngOnInit(): void {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.onSearchUpdate();
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    if(this.id){
      this.loadDisplayInsurance();
      this.loadInsuranceDebtReport();
    } 
  }

  loadDisplayInsurance(): void {
    this.resInsuranceService.getById(this.id).subscribe((res: any) => {
      this.insuranceInfo = res;
    })
  }

  onSearchUpdate(): void {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadInsuranceDebtReport();
      });
  }

  editInsurance(): void {
    let modalRef = this.modalService.open(ResInsuranceCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Sửa công ty bảo hiểm';
    modalRef.componentInstance.id = this.insuranceInfo ? this.insuranceInfo.id : '';
    modalRef.result.then((res: any) => {
      this.loadDisplayInsurance();
      this.notifyService.notify("success", "Lưu thành công")
    }, () => { });
  }

  loadInsuranceDebtReport() {
    var val = new InsuranceDebtFilter();
    val.search = this.search || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    val.insuranceId = this.id ? this.id : '';
    this.resInsuranceReportService.getInsuranceDebtReport(val).subscribe((res: any) => {
      this.insuranceDebt = res;
      const result = aggregateBy(res, [
        { aggregate: "sum", field: "amountTotal" },
      ]);
      this.sumAmount = result.amountTotal ? result.amountTotal.sum : 0;
    }, (error) => console.log(error))
  }

  onSearchDateChange(e): void {
    this.dateFrom = e.dateFrom || '';
    this.dateTo = e.dateTo || '';
    this.loadInsuranceDebtReport();
  }

  onPayment() {
    if (this.selectedIds.length === 0) {
      this.notifyService.notify('error', 'Bạn chưa chọn khoản tiền bảo hiểm phải thu');
      return;
    }
    this.accountPaymentService.insurancePaymentDefaultGet(this.selectedIds).subscribe(rs2 => {
      let modalRef = this.modalService.open(ResInsuranceDebtPaymentDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
      modalRef.componentInstance.title = 'Thu tiền bảo hiểm';
      modalRef.componentInstance.defaultVal = rs2;
      modalRef.componentInstance.partnerId = this.insuranceInfo.partnerId || '';
      modalRef.result.then(() => {
        this.selectedIds = [];
        this.loadInsuranceDebtReport();
        this.notifyService.notify('success', 'Thu tiền thành công');
      }, () => { });
    })
  }

  exportExcelFile() : void {
    var val = new InsuranceDebtFilter();
    val.search = this.search || '';
    val.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    val.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    val.insuranceId = this.id ? this.id : '';

    this.resInsuranceReportService.exportExcelFile(val).subscribe((res: any) => {
      let filename = "BaoCaoBaoHiem";
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
        window.URL.revokeObjectURL(data);
      }, 100);
    })
  }
}
