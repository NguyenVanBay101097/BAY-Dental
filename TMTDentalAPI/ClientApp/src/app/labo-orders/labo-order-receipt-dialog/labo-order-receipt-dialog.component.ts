import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { LaboOrderService } from '../labo-order.service';

@Component({
  selector: 'app-labo-order-receipt-dialog',
  templateUrl: './labo-order-receipt-dialog.component.html',
  styleUrls: ['./labo-order-receipt-dialog.component.css']
})
export class LaboOrderReceiptDialogComponent implements OnInit {
  labo : any;
  formGroup: FormGroup;
  submitted = false;
  
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private intelservice: IntlService,
    private laboOrderService: LaboOrderService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      dateReceipt : [null, Validators.required],
      warrantyCode : [null, Validators.required],
      warrantyPeriodObj: null,
      warrantyPeriod: null,
    });

    this.loadData();
  }

  loadData() {
    if (this.labo) {
     this.laboOrderService.get(this.labo.id).subscribe(rs => {   
      this.formGroup.patchValue(rs);
      let warrantyPeriodObj = new Date(rs.warrantyPeriod);
      this.formGroup.get("warrantyPeriodObj").patchValue(rs.warrantyPeriod ? new Date(rs.warrantyPeriod) : null);
      // this.formGroup.get('warrantyPeriod').setValue(rs.warrantyPeriod ? new Date(rs.warrantyPeriod) : null);
      // this.formGroup.get('warrantyCode').setValue(rs.warrantyCode ? this.labo.warrantyCode : null);
      this.formGroup.get('dateReceipt').setValue(new Date());
    });
     
    }
  }

  onChangeWarrantyPeriod() {
    var res = this.formGroup.value;
    if (this.getWarrantyPeriodType !== 'orther') {
      var date = new Date(this.getDateReceipt);
      res.warrantyPeriod = date.setFullYear(date.getFullYear() + this.getWarrantyPeriodType);
    }else{
      res.warrantyPeriod = new Date(this.getDateReceipt);
    }
    
    this.formGroup.patchValue(res);
  }

  onChangeCustomWarrantyPeriod(val){
    var res = this.formGroup.value;
    var date = new Date(this.getDateReceipt);
    res.warrantyPeriod = date.setFullYear(date.getFullYear() + val);
    this.formGroup.patchValue(res);
  }

  get getWarrantyPeriodType() {
    return this.formGroup.get('warrantyPeriodType').value;
  }

  get getDateReceipt() {
    return this.formGroup.get('dateReceipt').value;
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return false;
    }
  
    var val = this.formGroup.value;
    val.id = this.labo.id;
    val.warrantyPeriod = val.warrantyPeriodObj ? this.intelservice.formatDate(val.warrantyPeriodObj, 'yyyy-MM-dd HH:mm:ss') : null;
    val.warrantyCode = val.warrantyCode;
    val.dateReceipt = val.dateReceipt ? this.intelservice.formatDate(val.dateReceipt, 'yyyy-MM-dd HH:mm:ss') : null;

    this.laboOrderService.updateReceiptLabo(val).subscribe(() => {   
      this.activeModal.close();
    });
  }

  

  get f() {
    return this.formGroup.controls;
  }

}
