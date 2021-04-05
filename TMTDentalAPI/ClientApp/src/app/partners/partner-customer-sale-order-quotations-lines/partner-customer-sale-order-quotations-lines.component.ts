import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { AdvisoryLine, AdvisoryLinePaged, AdvisoryService } from 'src/app/advisories/advisory.service';

@Component({
  selector: 'app-partner-customer-sale-order-quotations-lines',
  templateUrl: './partner-customer-sale-order-quotations-lines.component.html',
  styleUrls: ['./partner-customer-sale-order-quotations-lines.component.css']
})
export class PartnerCustomerSaleOrderQuotationsLinesComponent implements OnInit {

  @Input() public advisoryId: string;
  skip = 0;
  limit = 10;
  gridData: any = [];
  details: AdvisoryLine[];
  loading = false;
  constructor(
    private advisoryService: AdvisoryService,
    private router: Router
  ) { }

  ngOnInit() {

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AdvisoryLinePaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.advisoryId = this.advisoryId;
    this.advisoryService.getAdvisoryLinePaged(val).subscribe(res => {
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      };
      this.loading = false;
    })
  }

  getFormReference(id , type){
    if(type="saleOrder") {
      this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
    }
    else {
      this.router.navigate(['/sale-orders/form'], { queryParams: { id: id } });
    }
  }

  pageChange(event:PageChangeEvent){
    this.skip = event.skip;
    this.loadDataFromApi();
  }

}
