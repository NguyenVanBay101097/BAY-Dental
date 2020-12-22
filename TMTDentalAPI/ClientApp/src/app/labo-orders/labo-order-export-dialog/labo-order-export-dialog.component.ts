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

  get f() { return this.formGroup.controls; }

  
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private intelservice: IntlService,
    private laboOrderService: LaboOrderService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateExportObj: [null, Validators.required]
    });

  }

}
