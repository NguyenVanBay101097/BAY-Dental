import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-service-card-cards-preferential-import-dialog',
  templateUrl: './service-card-cards-preferential-import-dialog.component.html',
  styleUrls: ['./service-card-cards-preferential-import-dialog.component.css']
})
export class ServiceCardCardsPreferentialImportDialogComponent implements OnInit {
  @Input() title: string;
  formGroup: FormGroup;
  type: string;
  update: string;
  isUpdate: boolean;
  errors: any = [];
  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private notifyService: NotifyService,
    private showErrorService: AppSharedShowErrorService,
  ) { }


  ngOnInit(): void {
    this.formGroup = this.fb.group({
      fileBase64: [null, Validators.required],
      checkAddress: false
    });
  }

  import() {
    if (!this.formGroup.valid) {
      this.notifyService.notify('error', 'Vui lòng chọn file đúng định dạng để import');
      return false;
    }

    var val = this.formGroup.value;
    val.type = this.type;
    // this.partnerService.actionImport(val).subscribe((result: any) => {
    //   if (result.success) {
    //     this.activeModal.close(true);
    //   } else {
    //     this.errors = result.errors;
    //   }
    // }, (err) => {
    // });
  }

  onFileChange(data) {
    this.formGroup.get('fileBase64').patchValue(data);
  }

  notifyError(value) {
    this.errors = value;
  }
}
