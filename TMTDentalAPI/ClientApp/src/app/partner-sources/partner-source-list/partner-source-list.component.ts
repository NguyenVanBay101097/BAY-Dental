import { Component, Inject, OnInit } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { GridDataResult, PageChangeEvent } from "@progress/kendo-angular-grid";
import { Subject } from "rxjs";
import { debounceTime, distinctUntilChanged, map } from "rxjs/operators";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { PageGridConfig, PAGER_GRID_CONFIG } from "src/app/shared/pager-grid-kendo.config";
import { NotifyService } from "src/app/shared/services/notify.service";
import { PartnerSourceCreateUpdateDialogComponent } from "../partner-source-create-update-dialog/partner-source-create-update-dialog.component";
import { PartnerSourceService } from "../partner-source.service";
import { PartnerSourceBasic, PartnerSourcePaged } from "./../partner-source.service";

@Component({
  selector: "app-partner-source-list",
  templateUrl: "./partner-source-list.component.html",
  styleUrls: ["./partner-source-list.component.css"],
})
export class PartnerSourceListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;

  search: string;
  searchUpdate = new Subject<string>();
  constructor(
    private partnerSourceService: PartnerSourceService,
    private modalService: NgbModal,
    private notifyService: NotifyService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
  ) { this.pagerSettings = config.pagerSettings }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PartnerSourcePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    this.partnerSourceService.getPaged(val).pipe(map((response) => <GridDataResult>{
      data: response.items,
      total: response.totalItems,
    }
    )
    ).subscribe(
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
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(PartnerSourceCreateUpdateDialogComponent, { size: 'xl', windowClass: "o_technical_modal", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.title = "Th??m ngu???n kh??ch h??ng";
    modalRef.result.then(() => {
      this.notifyService.notify("success","L??u th??nh c??ng");
      this.loadDataFromApi();
    }, () => { }
    );
  }

  editItem(item: PartnerSourceBasic) {
    let modalRef = this.modalService.open(PartnerSourceCreateUpdateDialogComponent, { size: 'xl', windowClass: "o_technical_modal", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.title = "S???a ngu???n kh??ch h??ng";
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.notifyService.notify("success","L??u th??nh c??ng");
      this.loadDataFromApi();
    }, () => { }
    );
  }

  deleteItem(item: PartnerSourceBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static", });
    modalRef.componentInstance.title = "X??a ngu???n kh??ch h??ng";
    modalRef.result.then(() => {
      this.partnerSourceService.delete(item.id).subscribe(() => {
      this.notifyService.notify("success","X??a th??nh c??ng");
        this.loadDataFromApi();
      }
      );
    }
    );
  }
}
