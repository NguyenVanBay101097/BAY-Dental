import { SamplePrescriptionBasic, SamplePrescriptionsSave, SamplePrescriptionsDisplay } from './../sample-prescriptions.service';
import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { SamplePrescriptionsService, SamplePrescriptionsPaged } from '../sample-prescriptions.service';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute } from '@angular/router';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { SamplePrescriptionCreateUpdateDialogComponent } from '../sample-prescription-create-update-dialog/sample-prescription-create-update-dialog.component';

@Component({
  selector: 'app-sample-prescription-list',
  templateUrl: './sample-prescription-list.component.html',
  styleUrls: ['./sample-prescription-list.component.css']
})
export class SamplePrescriptionListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  opened = false;

  search: string;
  searchUpdate = new Subject<string>();
  title = 'Đơn thuốc mẫu';

  constructor(private samplePrescriptionsService: SamplePrescriptionsService, private modalService: NgbModal, private route: ActivatedRoute) { }

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
    this.loadDataFromApi();
  }

  createItem() {
    let modalRef = this.modalService.open(SamplePrescriptionCreateUpdateDialogComponent, { size: "lg", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Thêm: " + this.title;
    modalRef.result.then(
      () => {
        this.loadDataFromApi();
      },
      () => { }
    );
  }

  editItem(item: SamplePrescriptionsDisplay) {
    let modalRef = this.modalService.open(SamplePrescriptionCreateUpdateDialogComponent, { size: "lg", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Sửa: " + this.title;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(
      () => {
        this.loadDataFromApi();
      },
      () => { }
    );
  }

  deleteItem(item: SamplePrescriptionBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
    modalRef.componentInstance.title = "Xóa: " + this.title;
    modalRef.result.then(
      () => {
        this.samplePrescriptionsService.delete(item.id).subscribe(
          () => {
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
