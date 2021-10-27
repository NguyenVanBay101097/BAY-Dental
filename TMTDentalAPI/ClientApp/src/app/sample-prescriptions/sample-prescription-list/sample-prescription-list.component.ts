import { SamplePrescriptionBasic, SamplePrescriptionsSave, SamplePrescriptionsDisplay } from './../sample-prescriptions.service';
import { Component, Inject, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { SamplePrescriptionsService, SamplePrescriptionsPaged } from '../sample-prescriptions.service';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute } from '@angular/router';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SamplePrescriptionCreateUpdateDialogComponent } from '../sample-prescription-create-update-dialog/sample-prescription-create-update-dialog.component';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: 'app-sample-prescription-list',
  templateUrl: './sample-prescription-list.component.html',
  styleUrls: ['./sample-prescription-list.component.css']
})
export class SamplePrescriptionListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  pagerSettings: any;
  loading = false;
  opened = false;

  search: string;
  searchUpdate = new Subject<string>();
  title = 'Đơn thuốc mẫu';

  constructor(
    private samplePrescriptionsService: SamplePrescriptionsService,
    private modalService: NgbModal, private route: ActivatedRoute,
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
    var val = new SamplePrescriptionsPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    this.samplePrescriptionsService
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
    this.limit = event.take;
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(SamplePrescriptionCreateUpdateDialogComponent, { size: 'xl', windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Thêm đơn thuốc mẫu";
    modalRef.result.then(
      () => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.loadDataFromApi();
      },
      () => { }
    );
  }

  editItem(item: SamplePrescriptionsDisplay) {
    let modalRef = this.modalService.open(SamplePrescriptionCreateUpdateDialogComponent, { size: 'xl', windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Sửa đơn thuốc mẫu";
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(
      () => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.loadDataFromApi();
      },
      () => { }
    );
  }

  deleteItem(item: SamplePrescriptionBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Xóa đơn thuốc mẫu";
    modalRef.result.then(
      () => {
        this.samplePrescriptionsService.delete(item.id).subscribe(
          () => {
            this.notifyService.notify('success', 'Xóa thành công');
            this.loadDataFromApi();
          },
          (err) => {
            console.log(err);
          }
        );
      },
      () => { }
    );
  }
}
