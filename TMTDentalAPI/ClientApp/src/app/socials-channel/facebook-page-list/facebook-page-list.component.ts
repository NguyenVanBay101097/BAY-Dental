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

  constructor(
    private facebookPageService: FacebookPageService,
    private modalService: NgbModal,
    private router: Router,
    private notificationService: NotificationService,
  ) {}

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

  createItem() {}

  editItem(item: any) {
    this.router.navigate(["/channel/" + item.id]);
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

  refreshItem(item: any){
    this.facebookPageService.refreshPage(item).subscribe(() => {
      this.loadDataFromApi();
      this.notificationService.show({
        content: "Làm mới kênh thành công!",
        hideAfter: 3000,
        position: { horizontal: "center", vertical: "top" },
        animation: { type: "fade", duration: 400 },
        type: { style: "success", icon: true },
      });
     
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
