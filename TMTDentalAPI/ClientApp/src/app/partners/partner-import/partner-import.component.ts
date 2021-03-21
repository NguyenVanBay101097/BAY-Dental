import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PartnerService, ImportExcelDirect } from '../partner.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { PartnerPaged } from '../partner-simple';

@Component({
  selector: 'app-partner-import',
  templateUrl: './partner-import.component.html',
  styleUrls: ['./partner-import.component.css']
})
export class PartnerImportComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder,
    private partnerService: PartnerService, private showErrorService: AppSharedShowErrorService, 
    private notificationService: NotificationService) { }

  formGroup: FormGroup;
  title = "";
  type: string;
  update:string;
  isUpdate:boolean;
  errors: any = [];

  ngOnInit() {
    this.formGroup = this.fb.group({
      fileBase64: [null, Validators.required],
      checkAddress: false
    });
  }

  notify(Style, Content) {
    this.notificationService.show({
      content: Content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: Style, icon: true }
    });
  }

  onFileChange(data) {
    this.formGroup.get('fileBase64').patchValue(data);
  }

  import() {
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

  updateFileExcel(){
    if (!this.formGroup.valid) {
      this.notificationService.show({
        content: 'Vui lòng chọn file để cập nhật',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }

    var val = this.formGroup.value;
    val.type = this.type;
    this.partnerService.actionImportUpdate(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
        this.notify('success', 'Cập nhật dữ liệu thành công');
      } else {
        this.errors = result.errors;
        this.notify('error', 'Cập nhật dữ liệu không thành công');
      }
    }, (err) => {
    });
  }

  loadExcelUpdateFile(){
    var paged = new PartnerPaged();
    paged.customer = true;
   // paged.search = this.search || "";

   // var categs = this.searchCategs || [];
   // paged.tagIds = categs.map(x => x.id);
    // paged.categoryId = this.searchCateg ? this.searchCateg.id : null;
    this.partnerService.exportPartnerExcelFile(paged).subscribe((rs) => {
      let filename = "danh_sach_khach_hang";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }
}
