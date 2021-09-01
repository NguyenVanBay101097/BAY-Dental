import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { SaleOrderLineService, SaleOrderLinesPaged } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { SaleOrdersOdataService } from 'src/app/shared/services/sale-ordersOdata.service';

@Component({
  selector: 'app-partner-customer-treatment-sale-order-line',
  templateUrl: './partner-customer-treatment-sale-order-line.component.html',
  styleUrls: ['./partner-customer-treatment-sale-order-line.component.css']
})
export class PartnerCustomerTreatmentSaleOrderLineComponent implements OnInit {
  @Input() dateOrderFrom: string;
  @Input() dateOrderTo: string;
  @Input() public saleOrderId: string;
  skip = 0;
  limit = 10;
  gridData: GridDataResult;
  details: SaleOrderLineDisplay[];
  loading = false;
  searchUpdate = new Subject<string>();
  toothTypeDict = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
    { name: "Chọn răng", value: "manual" },
  ];

  public total: any;
  constructor(
    private saleOrderlineService: SaleOrderLineService,
    private intlService: IntlService,
    private saleOrderOdataService: SaleOrdersOdataService
  ) { }

  ngOnInit() {
    
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadDataFromApi();
      });

    this.loadDataFromApi();
    // this.loadDataFromOData();
  }

  loadDataFromOData() {
    this.loading = true;
    var val = {
      id: this.saleOrderId,
      func: "GetSaleOrderLines",
      options: {
        params: new HttpParams().set('$count', 'true')
      }
    }
    this.saleOrderOdataService.getSaleOrderLines(val).subscribe(
      result => {
        this.gridData = {
          data: result && result['value'],

          total: result['@odata.count']
        };
        this.loading = false;
      }, () => {
        this.loading = false;
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var filter = new SaleOrderLinesPaged();
    filter.OrderId = this.saleOrderId;
    filter.dateOrderFrom = this.dateOrderFrom || '';
    filter.dateOrderTo = this.dateOrderTo || '';
    filter.limit = this.limit;
    filter.offset = this.skip;

    this.saleOrderlineService.getPaged(filter).pipe(
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

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  getInfor(item: any) {   
    var res = '';
    if (item.toothType && item.toothType == "manual") {
      res = item.teeth.map(x => x.name).join(',');
    } else if (item.toothType && item.toothType != "manual") {
      res = this.toothTypeDict.find(x => x.value == item.toothType).name;
    }
    else {
      res = '';
    }
    var sub = item.diagnostic ? ((res ? '; ' : '') + item.diagnostic) : '';
    return res + sub;
  }
}
