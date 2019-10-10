import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { StockPickingPaged, StockPickingService, StockPickingBasic } from '../stock-picking.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { StockPickingTypeBasic, StockPickingTypeService } from 'src/app/stock-picking-types/stock-picking-type.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-stock-picking-list',
  templateUrl: './stock-picking-list.component.html',
  styleUrls: ['./stock-picking-list.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class StockPickingListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 10;
  skip = 0;
  loading = false;
  pickingTypeId: string;
  pickingType: StockPickingTypeBasic;
  search: string;
  searchUpdate = new Subject<string>();

  constructor(private route: ActivatedRoute, private stockPickingService: StockPickingService,
    private pickingTypeService: StockPickingTypeService, private router: Router) { }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.pickingTypeId = params['picking_type_id'];
      this.loadPickingType();
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadDataFromApi();
      });

  }

  loadPickingType() {
    if (this.pickingTypeId) {
      this.pickingTypeService.getBasic(this.pickingTypeId).subscribe(result => {
        this.pickingType = result;
      });
    }
  }

  getState(item: StockPickingBasic) {
    switch (item.state) {
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Mới';
    }
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new StockPickingPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.pickingTypeId = this.pickingTypeId || '';

    this.stockPickingService.getPaged(val).pipe(
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
    this.router.navigate(['/pickings/create'], { queryParams: { picking_type_id: this.pickingTypeId } });
  }

  editItem(item: StockPickingBasic) {
    this.router.navigate(['/pickings/edit', item.id], { queryParams: { picking_type_id: this.pickingTypeId } });
  }
}
