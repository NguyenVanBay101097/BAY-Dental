import { Component, OnInit } from '@angular/core';
import { ZaloOAConfigService, ZaloOAConfigSave, ZaloOAConfigBasic } from '../zalo-oa-config.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-zalo-oa-config-establish',
  templateUrl: './zalo-oa-config-establish.component.html',
  styleUrls: ['./zalo-oa-config-establish.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ZaloOaConfigEstablishComponent implements OnInit {

  config: ZaloOAConfigBasic;
  formGroup: FormGroup;

  constructor(private fb: FormBuilder, private zaloOAConfigService: ZaloOAConfigService,
    private notificationService: NotificationService) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      autoSendBirthdayMessage: false,
      birthdayMessageContent: null
    });

    this.loadConfig();
  }

  get autoSendBirthdayMessage() {
    return this.formGroup.get('autoSendBirthdayMessage').value;
  }

  loadConfig() {
    this.zaloOAConfigService.get().subscribe(result => {
      this.config = result;
      if (this.config) {
        this.formGroup.patchValue(this.config);
      }
    });
  }

  connectZalo() {
    var url = 'https://oauth.zaloapp.com/v3/oa/permission?app_id=210286079830365439&redirect_uri=https://zalo.kiotapi.com';
    this.popupWindow(url, url, window, 650, 500);
    window.addEventListener('message', event => {
      var data = JSON.parse(event.data);
      var val = new ZaloOAConfigSave();
      val.accessToken = data.data.access_token;
      this.zaloOAConfigService.create(val).subscribe(result => {
        this.config = result;
      });
    });
  }

  deleteConfig() {
    this.zaloOAConfigService.remove().subscribe(() => {
      this.loadConfig();
    });
  }

  popupWindow(url, title, win, w, h) {
    const y = win.top.outerHeight / 2 + win.top.screenY - (h / 2);
    const x = win.top.outerWidth / 2 + win.top.screenX - (w / 2);
    return win.open(url, title, 'toolbar=no, status=no, menubar=no, scrollbars=no, resizable=no, width=' + w + ', height=' + h + ', top=' + y + ', left=' + x);
  }

  onSave() {
    if (this.config) {
      var val = this.formGroup.value;
      this.zaloOAConfigService.update(this.config.id, val).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }

  }
}
