import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { ServiceCardTypeService } from '../service-card-type.service';
import { ServiceCardTypePaged } from '../service-card-type-paged';
import { ServiceCardTypeCuDialogComponent } from '../service-card-type-cu-dialog/service-card-type-cu-dialog.component';

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

  constructor(private cardTypeService: ServiceCardTypeService,
    private modalService: NgbModal) { }

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

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: any) {
    let modalRef = this.modalService.open(ServiceCardTypeCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.id = item.id;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa loại thẻ';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.cardTypeService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    });
  }
}




