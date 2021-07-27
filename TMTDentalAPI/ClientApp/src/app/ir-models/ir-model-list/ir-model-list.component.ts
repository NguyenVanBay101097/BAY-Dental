import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { StockPickingTypeBasic, StockPickingTypeService } from 'src/app/stock-picking-types/stock-picking-type.service';
import { Subject } from 'rxjs';
import { IRModelService } from '../ir-model.service';
import { IRModelPaged, IRModelBasic } from '../ir-model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IrModelCuDialogComponent } from '../ir-model-cu-dialog/ir-model-cu-dialog.component';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-ir-model-list',
  templateUrl: './ir-model-list.component.html',
  styleUrls: ['./ir-model-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})

export class IrModelListComponent implements OnInit {
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;
  search: string;
  searchUpdate = new Subject<string>();
  title = 'Đối tượng';

  constructor(private modelService: IRModelService, private modalService: NgbModal) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
  }


  loadDataFromApi() {
    this.loading = true;
    var val = new IRModelPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    if (this.search) {
      val.filter = this.search;
    }

    this.modelService.getPaged(val).pipe(
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

  createItem() {
    let modalRef = this.modalService.open(IrModelCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Thêm ' + this.title;

    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  editItem(item: IRModelBasic) {
    let modalRef = this.modalService.open(IrModelCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Sửa ' + this.title;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  deleteItem(item: IRModelBasic) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa ' + this.title + ": " + item.name;
    modalRef.result.then(() => {
      this.modelService.delete(item.id).subscribe(() => {
        this.loadDataFromApi();
      });
    }, () => {
    });
  }
}

