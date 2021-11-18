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

  constructor(
    private modalService: NgbModal,
    private notifyService: NotifyService,
  ) { }

  ngOnInit(): void {
  }

  editInsurance(): void {
    let modalRef = this.modalService.open(ResInsuranceCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Sửa công ty bảo hiểm';
    modalRef.result.then(() => {
      this.notifyService.notify("success", "Lưu thành công")
    }, () => { });
  }
}
