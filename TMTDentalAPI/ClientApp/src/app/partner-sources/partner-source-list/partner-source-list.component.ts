import {
  PartnerSourcePaged,
  PartnerSourceBasic,
} from "./../partner-source.service";
import { Component, OnInit } from "@angular/core";
import { GridDataResult, PageChangeEvent } from "@progress/kendo-angular-grid";
import { Subject } from "rxjs";
import { PartnerSourceService } from "../partner-source.service";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ActivatedRoute } from "@angular/router";
import { map, debounceTime, distinctUntilChanged } from "rxjs/operators";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { PartnerSourceCreateUpdateDialogComponent } from "../partner-source-create-update-dialog/partner-source-create-update-dialog.component";

@Component({
  selector: "app-partner-source-list",
  templateUrl: "./partner-source-list.component.html",
  styleUrls: ["./partner-source-list.component.css"],
})
export class PartnerSourceListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;

  search: string;
  searchUpdate = new Subject<string>();
  type: string;
  constructor(
    private partnerSourceService: PartnerSourceService,
    private modalService: NgbModal,
    private route: ActivatedRoute
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
    var val = new PartnerSourcePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    this.partnerSourceService
      .getPaged(val)
      .pipe(
        map(
          (response) =>
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

  createItem() {
    let modalRef = this.modalService.open(
      PartnerSourceCreateUpdateDialogComponent,
      {
        size: "lg",
        windowClass: "o_technical_modal",
        keyboard: false,
        backdrop: "static",
      }
    );
    modalRef.componentInstance.title = "Thêm nguồn khách hàng";

    modalRef.result.then(
      () => {
        this.loadDataFromApi();
      },
      () => {}
    );
  }

  editItem(item: PartnerSourceBasic) {
    let modalRef = this.modalService.open(
      PartnerSourceCreateUpdateDialogComponent,
      {
        size: "lg",
        windowClass: "o_technical_modal",
        keyboard: false,
        backdrop: "static",
      }
    );
    modalRef.componentInstance.title = "Sửa nguồn khách hàng";
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(
      () => {
        this.loadDataFromApi();
      },
      () => {}
    );
  }

  deleteItem(item: PartnerSourceBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      size: "sm",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Xóa: Nguồn khách hàng";
    modalRef.result.then(
      () => {
        this.partnerSourceService.delete(item.id).subscribe(
          () => {
            this.loadDataFromApi();
          },
          (err) => {
            console.log(err);
          }
        );
      },
      () => {}
    );
  }
}
