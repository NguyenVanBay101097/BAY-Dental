import { Component, OnInit } from "@angular/core";
import {
  ZaloOAConfigService,
  ZaloOAConfigSave,
  ZaloOAConfigBasic,
} from "../zalo-oa-config.service";
import { FormGroup, FormBuilder } from "@angular/forms";
import { NotificationService } from "@progress/kendo-angular-notification";
import { FacebookPageService } from 'src/app/socials-channel/facebook-page.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map } from 'rxjs/operators';
import { FacebookPagePaged } from 'src/app/socials-channel/facebook-page-paged';

@Component({
  selector: "app-zalo-oa-config-establish",
  templateUrl: "./zalo-oa-config-establish.component.html",
  styleUrls: ["./zalo-oa-config-establish.component.css"],
  host: {
    class: "o_action o_view_controller",
  },
})
export class ZaloOaConfigEstablishComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  constructor(
    private zaloOAConfigService: ZaloOAConfigService,
    private notificationService: NotificationService,
    private facebookPageService: FacebookPageService
  ) {}

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;

    var val = new FacebookPagePaged();
    val.offset = this.skip;
    val.limit = this.limit;
    val.type = 'zalo';

    this.facebookPageService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  connectZalo() {
    var url =
      "https://oauth.zaloapp.com/v3/oa/permission?app_id=210286079830365439&redirect_uri=https://fba.tpos.vn/zalo/login-callback";
    this.popupWindow(url, url, window, 650, 500);
    window.addEventListener("message", (event) => {
      var data = JSON.parse(event.data);
      console.log(data.data.access_token);
      var val = new ZaloOAConfigSave();
      val.accessToken = data.data.access_token;
      this.zaloOAConfigService.create(val).subscribe((result) => {
        this.notificationService.show({
          content: "Kết nối thành công !",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });

        this.loadDataFromApi();
      });
    });
  }

  popupWindow(url, title, win, w, h) {
    const y = win.top.outerHeight / 2 + win.top.screenY - h / 2;
    const x = win.top.outerWidth / 2 + win.top.screenX - w / 2;
    return win.open(
      url,
      title,
      "toolbar=no, status=no, menubar=no, scrollbars=no, resizable=no, width=" +
        w +
        ", height=" +
        h +
        ", top=" +
        y +
        ", left=" +
        x
    );
  }
}
