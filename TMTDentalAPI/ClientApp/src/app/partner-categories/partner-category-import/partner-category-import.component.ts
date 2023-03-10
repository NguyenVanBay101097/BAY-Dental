import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PartnerCategoryService } from '../partner-category.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-partner-category-import',
  templateUrl: './partner-category-import.component.html',
  styleUrls: ['./partner-category-import.component.css']
})
export class PartnerCategoryImportComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder,
    private partnerCategoryService: PartnerCategoryService, 
    private notificationService: NotificationService) { }

  formGroup: FormGroup;
  title = 'Import excel';
  errors: any = [];
  correctFormat = true;
  ngOnInit() {
    this.formGroup = this.fb.group({
      fileBase64: [null, Validators.required],
    });
  }

  onFileChange(data) {
    this.formGroup.get('fileBase64').patchValue(data);
  }

  showError(event){
    if (event.length > 0) {
      this.notificationService.show({
        content: event[0],
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
    }
  }

  import() {
    if (!this.correctFormat){
      this.notificationService.show({
        content: 'File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }
    if (!this.formGroup.valid) {
      this.notificationService.show({
        content: 'Vui lòng chọn file để import',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    var val = this.formGroup.value;
    this.partnerCategoryService.actionImport(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    });
  }
}
