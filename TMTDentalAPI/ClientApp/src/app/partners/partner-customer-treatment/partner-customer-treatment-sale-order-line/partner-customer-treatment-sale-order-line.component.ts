import { HttpParams } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { SaleOrdersOdataService } from 'src/app/shared/services/sale-ordersOdata.service';

@Component({
  selector: 'app-partner-customer-treatment-sale-order-line',
  templateUrl: './partner-customer-treatment-sale-order-line.component.html',
  styleUrls: ['./partner-customer-treatment-sale-order-line.component.css']
})
export class PartnerCustomerTreatmentSaleOrderLineComponent implements OnInit {

  @Input() public saleOrderId: string;
  skip = 0;
  limit = 10;
  gridData: any = [];
  details: SaleOrderLineDisplay[];
  loading = false;
  toothTypeDict = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
    { name: "Chọn răng", value: "manual" },
  ];

  public total: any;
  constructor(
    private saleOrderlineService: SaleOrderLineService,
    private saleOrderOdataService: SaleOrdersOdataService
  ) { }

  ngOnInit() {
    this.loadDataFromOData();
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
  getInfor(item:any){
    var res = '';
    if (item.ToothType && item.ToothType == "manual") {
      res = item.Teeth.map(x => x.Name).join(',');
    } else if (item.ToothType && item.ToothType != "manual") {
      res = this.toothTypeDict.find(x => x.value == item.ToothType).name;
    }
    else {
      res = '';
    }
    var sub = item.Diagnostic? (( res ?'; ' : '') + item.Diagnostic) : '';
    return res +  sub;
  }
}
