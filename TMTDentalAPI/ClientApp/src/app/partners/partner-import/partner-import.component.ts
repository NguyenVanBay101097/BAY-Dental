import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { PartnerService, ImportExcelDirect } from '../partner.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-partner-import',
  templateUrl: './partner-import.component.html',
  styleUrls: ['./partner-import.component.css']
})
export class PartnerImportComponent implements OnInit {

  constructor(public activeModal: NgbActiveModal, private fb: FormBuilder, private activeRoute: ActivatedRoute,
    private service: PartnerService, private notificationService: NotificationService, private router: Router) { }

  isChange: boolean = false;
  formImport: FormGroup;
  selectedFile: File;
  isCustomer = false;
  title = '';
  isCreateNew = false;

  ngOnInit() {
    this.formImport = this.fb.group({
      file: [null, Validators.required],
    })
    this.routingChange();

    if (this.isCreateNew)
      this.title = 'Thêm mới từ Excel';
    else
      this.title = 'Cập nhật từ Excel';
  }

  routingChange() {
    this.activeRoute.queryParamMap.subscribe(
      params => {
        if (params['params']['customer'] == 'true') {
          this.isCustomer = true;
        }
        if (params['params']['supplier'] == 'true') {
          this.isCustomer = false;
        }
      },
      er => {
        console.log(er);
      }
    );
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
    var dir = new ImportExcelDirect;
    dir.isCreateNew = this.isCreateNew;
    dir.isCustomer = this.isCustomer;
    this.service.importFromExcelCreate(this.selectedFile, dir).subscribe(
      rs => {
        this.isChange = true;
        this.closeModal(null);
        this.notificationService.show({
          content: 'Thêm dữ liệu từ file thành công',
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
