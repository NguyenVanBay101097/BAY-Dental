import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { dateFieldName, IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { LaboOrderLineService } from '../../labo-order-line.service';
import * as jsonpatch from 'fast-json-patch';

@Component({
  selector: 'app-labo-order-statistic-update-dialog',
  templateUrl: './labo-order-statistic-update-dialog.component.html',
  styleUrls: ['./labo-order-statistic-update-dialog.component.css']
})
export class LaboOrderStatisticUpdateDialogComponent implements OnInit {

  line: any;
  lineinitial: any;

  constructor(
    private fb: FormBuilder,
    private intelservice: IntlService,
    private laboOrderLineService: LaboOrderLineService,
    private activeModal: NgbActiveModal,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.lineinitial = Object.assign({}, this.line);
      this.loadData();
  }

  loadData() {
    if (this.line) {
      this.line.warrantyPeriod = this.line.warrantyPeriod ? new Date(this.line.warrantyPeriod) : null;
      this.line.receivedDate = this.line.receivedDate ? new Date(this.line.receivedDate) : null;
    }
  }

  onCheckIsReceived(e) {
    if(e.target.checked) {
      this.line.receivedDate = new Date();
    }
  }

  onSave() {

    this.line.warrantyPeriod = this.intelservice.formatDate(this.line.warrantyPeriod, 'yyyy-MM-dd HH:mm:ss');
    this.line.receivedDate = this.intelservice.formatDate(this.line.receivedDate, 'yyyy-MM-dd HH:mm:ss');

    var observer = jsonpatch.observe(this.lineinitial);

    var patch = jsonpatch.generate(observer);

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
