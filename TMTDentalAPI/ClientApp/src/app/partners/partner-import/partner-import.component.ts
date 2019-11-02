import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PartnerService } from '../partner.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-partner-import',
  templateUrl: './partner-import.component.html',
  styleUrls: ['./partner-import.component.css']
})
export class PartnerImportComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder, private service: PartnerService, private notificationService: NotificationService) { }

  isChange: boolean = false;
  formImport: FormGroup;
  selectedFile: File;

  ngOnInit() {
    this.formImport = this.fb.group({
      file: [null, Validators.required],
    })
  }

  closeModal(rs) {
    if (this.isChange) {
      if (rs) {
        this.activeModal.close(rs);
      } else {
        this.activeModal.close(true);
      }
    }
    else {
      this.activeModal.dismiss();
    }
  }

  import() {
    this.service.importFromExcel(this.selectedFile).subscribe(
      rs => {
        this.isChange = true;
        this.closeModal(null);
        this.notificationService.show({
          content: 'Xuất file thành công',
          hideAfter: 3000,
          position: { horizontal: 'right', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }
    )
  }

  changeAttachment(e) {
    this.selectedFile = e.target.files[0];
  }
}
