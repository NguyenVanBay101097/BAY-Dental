import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TimeKeepingService } from '../time-keeping.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-time-keeping-import-file',
  templateUrl: './time-keeping-import-file.component.html',
  styleUrls: ['./time-keeping-import-file.component.css']
})
export class TimeKeepingImportFileComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder,
    private timeKeepingService: TimeKeepingService, private showErrorService: AppSharedShowErrorService) { }

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
    this.timeKeepingService.actionImport(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    }, (err) => {
      this.showErrorService.show(err);
    });
  }

}
