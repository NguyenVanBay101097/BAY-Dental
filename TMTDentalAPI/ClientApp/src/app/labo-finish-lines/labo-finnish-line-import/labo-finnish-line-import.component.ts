import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { LaboBiteJointService } from 'src/app/labo-bite-joints/labo-bite-joint.service';
import { LaboBridgeService } from 'src/app/labo-bridges/labo-bridge.service';
import { ImportExcelBaseViewModel, LaboFinishLineService } from '../labo-finish-line.service';

@Component({
  selector: 'app-labo-finnish-line-import',
  templateUrl: './labo-finnish-line-import.component.html',
  styleUrls: ['./labo-finnish-line-import.component.css']
})
export class LaboFinnishLineImportComponent implements OnInit {
  fileBase64 = '';
  type: string;
  errors: string[];
  title: string;
  correctFormat = true;
  constructor(private laboFinishLineService: LaboFinishLineService, 
    public activeModal: NgbActiveModal, 
    private notificationService: NotificationService,
    private laboBridgeService: LaboBridgeService,
    private laboBiteJointService: LaboBiteJointService) { }

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
    if (!this.correctFormat){
      this.notify('error','File import sai định dạng. Vui lòng tải file mẫu và nhập dữ liệu đúng');
      return;
    }
    if (!this.fileBase64 || this.fileBase64 === '') {
      this.notify('error', 'Vui lòng chọn file để import');
      return;
    }
    var val = new ImportExcelBaseViewModel();
    val.fileBase64 = this.fileBase64;
    this.actionImport(val).subscribe((result: any) => {
      if (result.success) {
        this.notify('success', 'Import dữ liệu thành công');
        this.activeModal.close(true);
      } else {
        this.errors = result.errors;
      }
    }, err => {
    });
  }

  actionImport(val: any) {
    if (this.type == 'finish_line') {
      return this.laboFinishLineService.ImportExcel(val);
    } else if(this.type == 'labo_bridge') {
      return this.laboBridgeService.ImportExcel(val);

    } else if(this.type == 'labo_bitejoint') {
      return this.laboBiteJointService.ImportExcel(val);

    }
     else {

    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }


}
