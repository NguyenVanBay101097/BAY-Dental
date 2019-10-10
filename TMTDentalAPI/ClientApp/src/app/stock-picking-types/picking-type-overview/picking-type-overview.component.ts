import { Component, OnInit } from '@angular/core';
import { StockPickingTypePaged, StockPickingTypeService, StockPickingTypeBasic } from '../stock-picking-type.service';

@Component({
  selector: 'app-picking-type-overview',
  templateUrl: './picking-type-overview.component.html',
  styleUrls: ['./picking-type-overview.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PickingTypeOverviewComponent implements OnInit {
  limit = 20;
  skip = 0;
  pickingTypes: StockPickingTypeBasic[] = [];
  constructor(private stockPickingTypeService: StockPickingTypeService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new StockPickingTypePaged();
    val.limit = this.limit;
    val.offset = this.skip;

    this.stockPickingTypeService.getPaged(val).subscribe(res => {
      this.pickingTypes = res.items;
    }, err => {
      console.log(err);
    })
  }
}
