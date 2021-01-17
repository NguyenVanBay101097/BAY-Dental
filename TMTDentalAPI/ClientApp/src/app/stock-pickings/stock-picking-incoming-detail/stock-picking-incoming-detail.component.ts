import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { StockPickingBasic, StockPickingService } from '../stock-picking.service';

@Component({
  selector: 'app-stock-picking-incoming-detail',
  templateUrl: './stock-picking-incoming-detail.component.html',
  styleUrls: ['./stock-picking-incoming-detail.component.css']
})
export class StockPickingIncomingDetailComponent implements OnInit {
  @Input() public item: StockPickingBasic;
  skip = 0;
  limit = 10;
  data = [];

  constructor( private stockPickingService: StockPickingService,) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {

    this.stockPickingService.get(this.item.id).subscribe(res => {
      this.data = res.moveLines;
    }, err => {
      console.log(err);
    })
  }
}
