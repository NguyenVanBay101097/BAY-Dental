import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IrSequenceDisplay, IrsequenceService } from '../irsequence.service';

@Component({
  selector: 'app-partner-customer-auto-generate-code-dialog',
  templateUrl: './partner-customer-auto-generate-code-dialog.component.html',
  styleUrls: ['./partner-customer-auto-generate-code-dialog.component.css']
})
export class PartnerCustomerAutoGenerateCodeDialogComponent implements OnInit {
  title = "Mã sinh tự động";
  formGroup: FormGroup;
  id: string;
  code: string;
  constructor(
    private activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private irSequenceService: IrsequenceService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      Code: ['customer', Validators.required],
      Name: ['Mã khách hàng', Validators.required],
      NumberNext: [1, Validators.required],
      Padding: [1, Validators.required],
      Prefix: ['KH', Validators.required],
      NumberInCrement: [1, Validators.required]
    });
    this.loadFormApi();
  }

  loadFormApi() {
    this.irSequenceService.get(this.code).subscribe(
      result => {
        this.id = result.value[0].Id;
        this.formGroup.patchValue(result.value[0]);
      }
    )
  }

  onSave() {
    if (this.formGroup.invalid || !this.id) {
      return;
    }
    var value = this.formGroup.value;
    this.irSequenceService.update(this.id, value).subscribe(
      () => {
        this.activeModal.close(true);
      }, err => {
        this.activeModal.close(false);
        console.log(err);
      }
    )
  }

}
