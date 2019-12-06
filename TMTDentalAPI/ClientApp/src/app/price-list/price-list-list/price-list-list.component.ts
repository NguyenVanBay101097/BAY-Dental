import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { PriceListService } from '../price-list.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { DialogService, DialogRef, DialogCloseResult } from '@progress/kendo-angular-dialog';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { ProductPricelistPaged } from '../price-list';

@Component({
  selector: 'app-price-list-list',
  templateUrl: './price-list-list.component.html',
  styleUrls: ['./price-list-list.component.css']
})
export class PriceListListComponent implements OnInit {

  loading = false;
  gridData: GridDataResult;
  skip = 0;
  pageSize = 20;
  constructor(private service: PriceListService, private dialogService: DialogService, private router: Router) { }

  search: string;
  searchUpdate = new Subject<string>();

  ngOnInit() {
    this.loadPriceLists();
    this.searchChange();
  }

  loadPriceLists() {
    this.loading = true;
    var plPaged = new ProductPricelistPaged;
    plPaged.limit = this.pageSize;
    plPaged.offset = this.skip;
    if (this.search) {
      plPaged.search = this.search;
    }

    this.service.loadPriceListList(plPaged).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2 => {
      this.gridData = rs2;
      this.loading = false;
    }, er => {
      this.loading = true;
    }
    )
  }

  searchChange() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadPriceLists();
      });
  }

  deletePriceList(id) {
    const dialogRef: DialogRef = this.dialogService.open({
      title: 'Xóa bảng giá',
      content: 'Bạn chắc chắn muốn xóa bảng giá này ?',
      width: 450,
      height: 200,
      minWidth: 250,
      actions: [
        { text: 'Hủy', value: false },
        { text: 'Đồng ý', primary: true, value: true }
      ]
    });
    dialogRef.result.subscribe(
      rs => {
        if (!(rs instanceof DialogCloseResult)) {
          if (rs['value']) {
            this.service.deletePriceList(id).subscribe(rs => {
              this.loadPriceLists();
            }
            );
          }
        }
      }
    )
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.pageSize = event.take;
  }

  createNew() {
    this.router.navigate(['/pricelists/create']);
  }

  editItem(id) {
    this.router.navigate(['/pricelists/edit/' + id]);
  }
}
