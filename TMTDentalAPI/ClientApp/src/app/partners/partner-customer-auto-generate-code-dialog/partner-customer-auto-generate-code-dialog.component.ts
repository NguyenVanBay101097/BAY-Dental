import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IRSequencesService } from 'src/app/shared/services/ir-sequences.service';
import { IrSequenceDisplay, IrsequenceService } from '../irsequence.service';

@Component({
  selector: 'app-partner-customer-auto-generate-code-dialog',
  templateUrl: './partner-customer-auto-generate-code-dialog.component.html',
  styleUrls: ['./partner-customer-auto-generate-code-dialog.component.css']
})
export class PartnerCustomerAutoGenerateCodeDialogComponent implements OnInit {
  title = "Cấu hình mã sinh tự động";
  formGroup: FormGroup;
  id: string;
  code: string;
  constructor(
    private activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private irSequencesService: IRSequencesService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      NumberNext: [1, Validators.required],
      Padding: [1, Validators.required],
      Prefix: ['KH', Validators.required],
    });
    this.loadFormApi();
  }

  loadFormApi() {
    this.irSequencesService.getByCode(this.code).subscribe(
      result => {
        if (result.data.length) {
          var item = result.data[0];
          this.id = item.Id;
          this.formGroup.patchValue(item);
        }
      }
    );
  }

  onSave() {
    if (this.formGroup.invalid || !this.id) {
      return;
    }
    var value = this.formGroup.value;
    this.irSequencesService.update(this.id, value).subscribe(
      () => {
        this.activeModal.close(true);
      }, err => {
        this.activeModal.close(false);
        console.log(err);
      }
    )
  }

}
