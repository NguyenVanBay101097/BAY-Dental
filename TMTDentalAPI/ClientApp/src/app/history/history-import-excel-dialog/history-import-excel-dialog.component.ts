import { HistoryService, HistoryImportExcelBaseViewModel } from './../history.service';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
@Component({
  selector: 'app-history-import-excel-dialog',
  templateUrl: './history-import-excel-dialog.component.html',
  styleUrls: ['./history-import-excel-dialog.component.css']
})

export class HistoryImportExcelDialogComponent implements OnInit {
  fileBase64 = '';
  errors: string[];
  title: string;
  formGroup: FormGroup;
  correctFormat = true;
  constructor(private historyService: HistoryService, public activeModal: NgbActiveModal, private fb: FormBuilder,
    private notificationService: NotificationService) { }

    ngOnInit() {
      this.formGroup = this.fb.group({
        fileBase64: [null, Validators.required],
      });
    }
  
    onFileChange(data) {
      this.formGroup.get('fileBase64').patchValue(data);
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
  
    onSave() {
      if (!this.correctFormat){
        this.notificationService.show({
          content: 'Dữ liệu không đúng định dạng mẫu',
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
      this.actionImport(val).subscribe((result: any) => {
        if (result.success) {
          this.activeModal.close(true);
          this.notify('success','Import dữ liệu thành công');
        } else {
          this.errors = result.errors;
          this.notify('error','Import dữ liệu không thành công');
        }
      });
    }

  actionImport(val: any) {
    return this.historyService.importExcel(val);
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}
