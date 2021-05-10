import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ServiceCardTypeService } from '../service-card-type.service';
import { ServiceCardTypePaged } from '../service-card-type-paged';
import { ServiceCardTypeCuDialogComponent } from '../service-card-type-cu-dialog/service-card-type-cu-dialog.component';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';

@Component({
  selector: 'app-service-card-type-list',
  templateUrl: './service-card-type-list.component.html',
  styleUrls: ['./service-card-type-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ServiceCardTypeListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  title = 'Loại thẻ tiền mặt';

  // permission 
  canServiceCardTypeCreate = this.checkPermissionService.check(["ServiceCard.Type.Create"]);
  canServiceCardTypeUpdate = this.checkPermissionService.check(["ServiceCard.Type.Update"]);
  canServiceCardTypeDelete = this.checkPermissionService.check(["ServiceCard.Type.Delete"]);

  constructor(private cardTypeService: ServiceCardTypeService,
    private modalService: NgbModal, private checkPermissionService: CheckPermissionService) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new ServiceCardTypePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.cardTypeService.getPaged(val).pipe(
      map((response: any) => (<GridDataResult>{
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

  createItem() {
    let modalRef = this.modalService.open(ServiceCardTypeCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: ' + this.title;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: any) {
    let modalRef = this.modalService.open(ServiceCardTypeCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = item.id;
    modalRef.componentInstance.title = 'Sửa: ' + this.title;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa: ' + this.title;
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.cardTypeService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}




