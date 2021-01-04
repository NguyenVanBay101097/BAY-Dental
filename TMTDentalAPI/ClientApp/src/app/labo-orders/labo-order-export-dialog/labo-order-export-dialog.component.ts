import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-export-dialog',
  templateUrl: './labo-order-export-dialog.component.html',
  styleUrls: ['./labo-order-export-dialog.component.css']
})
export class LaboOrderExportDialogComponent implements OnInit {
  labo : any;
  formGroup: FormGroup;
  submitted = false;

  get f() { return this.formGroup.controls; }

  
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private intelservice: IntlService,
    private laboOrderService: LaboOrderService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateExport: [null, Validators.required]
    });

    if (this.labo) {
      if (this.labo.dateExport) {
        this.formGroup.get('dateExport').setValue(new Date(this.labo.dateExport));
      } else {
        this.formGroup.get('dateExport').setValue(new Date());
      }
    }
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }
  
    var val = this.formGroup.value;
    val.id = this.labo.id;
    val.dateExport = val.dateExport ? this.intelservice.formatDate(val.dateExport, 'yyyy-MM-dd HH:mm:ss') : null;

    this.laboOrderService.updateExportLabo(val).subscribe(() => {   
      this.activeModal.close();
    });
  }

  onCancelReceipt() {
    this.laboOrderService.actionCancelReceipt([this.labo.id]).subscribe(() => {   
      this.activeModal.close();
    });
  }

  
}
