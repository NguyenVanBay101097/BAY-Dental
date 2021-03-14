import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { StockInventoryCriteriaService } from '../stock-inventory-criteria.service';

@Component({
  selector: 'app-stock-inventory-criteria-cu-dialog',
  templateUrl: './stock-inventory-criteria-cu-dialog.component.html',
  styleUrls: ['./stock-inventory-criteria-cu-dialog.component.css']
})
export class StockInventoryCriteriaCuDialogComponent implements OnInit {

  constructor(
    public activeModal: NgbActiveModal,
    public fb: FormBuilder,
    public criteriaService: StockInventoryCriteriaService,
    public notifyService: NotifyService
  ) { }

  title: string;
  formGroup: FormGroup;
  id: string;
  submitted = false;

  ngOnInit() {
    this.formGroup = this.fb.group({
      id: null,
      name: [null, Validators.required],
      note: [null]
    });
    if (this.id) {
      this.loadDataFromApi();
    }
  }

  loadDataFromApi() {
    this.criteriaService.get(this.id).subscribe(res => {
      this.formGroup.patchValue(res);
    })
  }

  onSave() {
    if (this.formGroup.invalid) return;
    var val = this.formGroup.value;
    if (this.id) {
      this.criteriaService.update(this.id, val).subscribe(res => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.activeModal.close(res);
      })
    } else {
      this.criteriaService.create(val).subscribe(res => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.activeModal.close(res);
      })
    }
  }

  get f() { return this.formGroup.controls; }
}
