import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { SettingPublicApiService } from '../setting-public-api.service';

@Component({
  selector: 'app-generate-token',
  templateUrl: './generate-token.component.html',
  styleUrls: ['./generate-token.component.css']
})
export class GenerateTokenComponent implements OnInit {
  formGroup: FormGroup;
  constructor(private fb: FormBuilder , private settingPublicApiService : SettingPublicApiService,
    private modalService: NgbModal,
    private notifyService: NotifyService) 
  { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      token: null
    });
    this.loadDataApi();
  }

  loadDataApi(){
    this.settingPublicApiService.getToken().subscribe(res => {
      this.formGroup.get('token').patchValue(res);
    });
  }

  generateToken(){
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Tạo mới token';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn tạo mã token mới?';
    modalRef.result.then(() => {
      this.settingPublicApiService.generateToken().subscribe(() => {
        this.notifyService.notify('success','Vui lòng gửi mã token mới cho bên thứ ba để kết nội lại public API với TDental');
        this.loadDataApi();
      });
    }, () => {
    });
  }

}
