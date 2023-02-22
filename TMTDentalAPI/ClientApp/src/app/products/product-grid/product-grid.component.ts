import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ProductBasic2 } from '../product.service';

@Component({
  selector: 'app-product-grid',
  templateUrl: './product-grid.component.html',
  styleUrls: ['./product-grid.component.css']
})
export class ProductGridComponent implements OnInit {
  @Input() gridData: GridDataResult;
  @Input() limit: number = 20;
  @Input() skip: number = 0;
  @Input() loading: boolean = false;
  @Input() nameTitle: string;
  @Input() defaultCodeTitle: string;
  @Input() categTitle: string;

  @Input() hiddenColumns: string[] = [];
  @Output() pageChange = new EventEmitter<PageChangeEvent>();
  @Output() editItem = new EventEmitter<ProductBasic2>();
  @Output() deleteItem = new EventEmitter<ProductBasic2>();


  constructor() { }

  ngOnInit() {
  }

  onPageChange(event: PageChangeEvent): void {
    this.pageChange.emit(event);
  }

  editBtnClick(item: ProductBasic2) {
    this.editItem.emit(item);
  }

  deleteBtnClick(item: ProductBasic2) {
    this.deleteItem.emit(item);
  }
}
