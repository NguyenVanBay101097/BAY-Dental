import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { TimeKeepingService } from '../time-keeping.service';

@Component({
  selector: 'app-time-keeping-import-file',
  templateUrl: './time-keeping-import-file.component.html',
  styleUrls: ['./time-keeping-import-file.component.css']
})
export class TimeKeepingImportFileComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder,
    private timeKeepingService: TimeKeepingService,
    private notificationService: NotificationService) { }

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
      debugger
      if (result.success) {
        this.notificationService.show({
          content: 'Import thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    }, (err) => {
    });
  }

}
