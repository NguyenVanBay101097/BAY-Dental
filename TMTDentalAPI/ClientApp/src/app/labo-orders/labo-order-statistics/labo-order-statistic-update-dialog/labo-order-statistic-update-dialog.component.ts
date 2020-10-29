import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { dateFieldName, IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { LaboOrderLineService } from '../../labo-order-line.service';
import { compare } from 'fast-json-patch';

@Component({
  selector: 'app-labo-order-statistic-update-dialog',
  templateUrl: './labo-order-statistic-update-dialog.component.html',
  styleUrls: ['./labo-order-statistic-update-dialog.component.css']
})
export class LaboOrderStatisticUpdateDialogComponent implements OnInit {

  line: any;
  lineinitial: any;
  warrantyPeriod: any;
  receivedDate: any;

  constructor(
    private fb: FormBuilder,
    private intelservice: IntlService,
    private laboOrderLineService: LaboOrderLineService,
    private activeModal: NgbActiveModal,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.loadData();
    setTimeout(() => {
      this.lineinitial = Object.assign({}, this.line);
    }, 200);
  }

  loadData() {
    if (this.line) {
      this.warrantyPeriod = this.line.warrantyPeriod ? new Date(this.line.warrantyPeriod) : null;
      this.receivedDate = this.line.receivedDate ? new Date(this.line.receivedDate) : null;
    }
  }

  onCheckIsReceived(e) {
    if(e.target.checked) {
      this.line.receivedDate = new Date();
    } else {
      this.line.receivedDate = null;
    }
  }

  onSave() {

    this.line.warrantyPeriod = this.warrantyPeriod ? this.intelservice.formatDate(this.warrantyPeriod, 'yyyy-MM-dd HH:mm:ss') : null;
    this.line.receivedDate = this.line.isReceived ? this.intelservice.formatDate(this.receivedDate, 'yyyy-MM-dd HH:mm:ss') : null;

    const patch = compare(this.lineinitial, this.line);

    this.laboOrderLineService.updateStatistic(this.line.id, patch).subscribe(() => {
      this.notificationService.show({
        content: 'Cập nhật thành công!',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.activeModal.close();
    });
  }
}
