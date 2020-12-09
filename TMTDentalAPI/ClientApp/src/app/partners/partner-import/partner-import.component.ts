import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PartnerService, ImportExcelDirect } from '../partner.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-partner-import',
  templateUrl: './partner-import.component.html',
  styleUrls: ['./partner-import.component.css']
})
export class PartnerImportComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder,
    private partnerService: PartnerService, private showErrorService: AppSharedShowErrorService) { }

  formGroup: FormGroup;
  title = 'Import';
  type: string;
  errors: any = [];

  ngOnInit() {
    this.formGroup = this.fb.group({
      fileBase64: [null, Validators.required],
      checkAddress: false
    });
  }


  onFileChange(data) {
    this.formGroup.get('fileBase64').patchValue(data);
  }

  import() {
    if (!this.formGroup.valid) {
      return false;
    }

    var val = this.formGroup.value;
    val.type = this.type;
    this.partnerService.actionImport(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    }, (err) => {
    });
  }
}
