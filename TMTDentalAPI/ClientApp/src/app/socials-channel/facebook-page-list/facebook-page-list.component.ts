import { Component, OnInit } from "@angular/core";
import { GridDataResult, PageChangeEvent } from "@progress/kendo-angular-grid";
import { map, debounceTime, distinctUntilChanged } from "rxjs/operators";
import { Subject } from "rxjs";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { FacebookPageService } from "../facebook-page.service";
import { FacebookPagePaged } from "../facebook-page-paged";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { NotificationService } from '@progress/kendo-angular-notification';
import { FacebookService, LoginOptions, LoginResponse } from 'ngx-facebook';
import { FacebookConnectService } from '../facebook-connect.service';
import { ZaloOAConfigSave, ZaloOAConfigService } from 'src/app/zalo-oa-config/zalo-oa-config.service';

@Component({
  selector: "app-facebook-page-list",
  templateUrl: "./facebook-page-list.component.html",
  styleUrls: ["./facebook-page-list.component.css"],
  host: {
    class: "o_action o_view_controller",
  },
})
export class FacebookPageListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  title = "Danh sách kênh";

  constructor(private fb: FacebookService, private facebookConnectService: FacebookConnectService,
    private zaloOAConfigService: ZaloOAConfigService,
    private facebookPageService: FacebookPageService,
    private modalService: NgbModal,
    private router: Router,
    private notificationService: NotificationService,
  ) {

    fb.init({
      appId: '327268081110321',
      status: true,
      cookie: true,
      xfbml: true,
      version: 'v6.0'
    });
  }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new FacebookPagePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";

    this.facebookPageService
      .getPaged(val)
      .pipe(
        map(
          (response: any) =>
            <GridDataResult>{
              data: response.items,
              total: response.totalItems,
            }
        )
      )
      .subscribe(
        (res) => {
          this.gridData = res;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  createItem() { }

  editItem(item: any) {
    this.router.navigate(["/socials/channel/" + item.id]);
  }

  deleteItem(item: any) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      size: "sm",
      windowClass: "o_technical_modal",
    });
    modalRef.componentInstance.title = "Xóa kênh";
    modalRef.componentInstance.body = "Bạn chắc chắn muốn xóa?";
    modalRef.result.then(() => {
      this.facebookPageService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }

  refreshItem(item: any) {
    if (item.type === 'facebook') {
      this.refreshItemFacebook(item);
    } else if (item.type === 'zalo') {
      this.refreshItemZalo(item);
    }
  }

  refreshItemZalo(item: any) {
    var url =
      "https://oauth.zaloapp.com/v3/oa/permission?app_id=210286079830365439&redirect_uri=https://fba.tpos.vn/zalo/login-callback";

    this.popupWindow(url, url, window, 650, 500);
    window.addEventListener("message", (event) => {
      var data = JSON.parse(event.data);
      var val = new ZaloOAConfigSave();
      val.pageId = item.pageId;
      val.accessToken = data.data.access_token;
      this.zaloOAConfigService.create(val).subscribe((result) => {
        this.notificationService.show({
          content: "Làm mới kênh thành công",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });
      });
    }, { once: true });
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

  refreshItemFacebook(item: any) {
    const loginOptions: LoginOptions = {
      enable_profile_selector: true,
      return_scopes: true,
      scope: 'public_profile,manage_pages,publish_pages,pages_messaging,read_page_mailboxes'
    };
    this.fb.login(loginOptions)
      .then((res: LoginResponse) => {
        var val = {
          fbUserId: res.authResponse.userID,
          fbUserAccessToken: res.authResponse.accessToken
        };
        this.facebookConnectService.saveFromUI(val).subscribe(() => {
          this.facebookPageService.refreshPage(item).subscribe(() => {
            this.loadDataFromApi();
            this.notificationService.show({
              content: "Làm mới kênh thành công",
              hideAfter: 3000,
              position: { horizontal: "center", vertical: "top" },
              animation: { type: "fade", duration: 400 },
              type: { style: "success", icon: true },
            });
          });
        });
      }).catch((error) => {
        console.error('Error processing action', error);
      });
  }

  getTypeDisplay(type) {
    switch (type) {
      case "facebook":
        return "Facebook Page";
      case "zalo":
        return "Zalo Official Account";
      default:
        return "";
    }
  }
}
