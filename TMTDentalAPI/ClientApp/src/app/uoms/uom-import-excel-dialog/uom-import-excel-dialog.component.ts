import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { UomService } from '../uom.service';

@Component({
  selector: 'app-uom-import-excel-dialog',
  templateUrl: './uom-import-excel-dialog.component.html',
  styleUrls: ['./uom-import-excel-dialog.component.css']
})

export class UomImportExcelDialogComponent implements OnInit {
  fileBase64 = '';
  type: string;
  errors: string[] = [];
  title: string = "Import đơn vị tính";
  isUpdate: boolean = false;
  constructor(private uomService: UomService, public activeModal: NgbActiveModal, private notificationService: NotificationService) { }

  ngOnInit() {
  }

  onFileChange(value) {
    this.fileBase64 = value;
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

  onSave() {
    if (!this.fileBase64 || this.fileBase64 === '') {
      if (this.isUpdate) {
        this.notify('error', 'Vui lòng chọn file để cập nhật');
      }
      else {
        this.notify('error', 'Vui lòng chọn file để import');
      }
      return;
    }

    if (this.errors.length > 0)
      return;

    var val = { fileBase64: this.fileBase64 };
    this.uomService.importExcel(val).subscribe((result: any) => {
      if (result.success) {
        this.notify('success', 'Import dữ liệu thành công');
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    }, err => {
    });
  }

  notifyError(value) {
    this.errors = value;
  }
}

