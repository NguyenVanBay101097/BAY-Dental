import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ResInsuranceCuDialogComponent } from 'src/app/shared/res-insurance-cu-dialog/res-insurance-cu-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-res-insurance-debit',
  templateUrl: './res-insurance-debit.component.html',
  styleUrls: ['./res-insurance-debit.component.css']
})
export class ResInsuranceDebitComponent implements OnInit {
  dateFrom: Date;
  dateTo: Date;
  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private modalService: NgbModal,
    private notifyService: NotifyService,
  ) { }

  ngOnInit(): void {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

  }

  editInsurance(): void {
    let modalRef = this.modalService.open(ResInsuranceCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Sửa công ty bảo hiểm';
    modalRef.result.then(() => {
      this.notifyService.notify("success", "Lưu thành công")
    }, () => { });
  }

  onSearchDateChange(e): void {
    this.dateFrom = e.dateFrom || '';
    this.dateTo = e.dateTo || '';
  }
}
