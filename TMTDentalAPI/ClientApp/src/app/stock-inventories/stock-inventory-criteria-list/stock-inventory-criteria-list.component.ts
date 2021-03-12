import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfigurationService } from '@progress/kendo-angular-charts';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { StockInventoryCriteriaCuDialogComponent } from '../stock-inventory-criteria-cu-dialog/stock-inventory-criteria-cu-dialog.component';
import { StockInventoryCriteriaBasic, StockInventoryCriteriaPaged, StockInventoryCriteriaPaging, StockInventoryCriteriaSave, StockInventoryCriteriaService } from '../stock-inventory-criteria.service';

@Component({
  selector: 'app-stock-inventory-criteria-list',
  templateUrl: './stock-inventory-criteria-list.component.html',
  styleUrls: ['./stock-inventory-criteria-list.component.css']
})
export class StockInventoryCriteriaListComponent implements OnInit {

  constructor(
    private criteriaService: StockInventoryCriteriaService,
    private ngbModal: NgbModal,
    private notifyService: NotifyService
  ) { }

  search: string;
  searchUpdate = new Subject<string>();
  gridData: GridDataResult;
  limit= 20;
  skip = 0;
  loading = false;

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(500),
      distinctUntilChanged(),
    ).subscribe((val) => {
      this.loadDataFromApi();
    });
    
    this.loadDataFromApi();
  }

  createItem() {
   var dg = this.ngbModal.open(StockInventoryCriteriaCuDialogComponent,{size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
   dg.componentInstance.title= 'Thêm Tiêu chí kiểm kho';
   dg.result.then((val)=>{
    this.loadDataFromApi();
   });
  }

  loadDataFromApi() {
    var page = new StockInventoryCriteriaPaged();
    page.limit = this.limit;
    page.offset = this.skip;
    page.search = this.search || '';
    this.criteriaService.getPaged(page).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(
      (res: GridDataResult) => {
        this.gridData = res;
      }
    );
  }

  pageChange(e: PageChangeEvent) {
    this.skip = e.skip;
    this.loadDataFromApi();
  }

  editItem(item: StockInventoryCriteriaBasic) {
    var dg = this.ngbModal.open(StockInventoryCriteriaCuDialogComponent,{size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    dg.componentInstance.title= 'Sửa Tiêu chí kiểm kho: '+ item.name;
    dg.componentInstance.id = item.id;
    dg.result.then((val)=>{
     this.criteriaService.update(item.id, val).subscribe(
       ()=> {this.loadDataFromApi();
      }
     );
    });
  }

  deleteItem(item: StockInventoryCriteriaBasic) {
    var dg = this.ngbModal.open(ConfirmDialogComponent,{size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    dg.componentInstance.title= 'Bạn có chắc chắn muốn xóa tiêu chí: ' + item.name;
    dg.result.then(()=>{
     this.criteriaService.delete(item.id).subscribe(() => {
       this.notifyService.notify('success', 'Xóa thành công');
       this.loadDataFromApi();
     });
    });
  }

}
