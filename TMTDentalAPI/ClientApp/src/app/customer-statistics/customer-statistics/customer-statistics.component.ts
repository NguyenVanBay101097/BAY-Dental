import { Component, OnInit } from '@angular/core';
import { CustomerStatisticsInput, CustomerStatisticsOutput, PartnerService } from 'src/app/partners/partner.service';

@Component({
  selector: 'app-customer-statistics',
  templateUrl: './customer-statistics.component.html',
  styleUrls: ['./customer-statistics.component.css']
})
export class CustomerStatisticsComponent implements OnInit {
  dateFrom: any;
  dateTo: any;

  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  customerStatistics: CustomerStatisticsOutput;

  constructor(private partnerService: PartnerService) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new CustomerStatisticsInput();
    val.dateFrom = this.dateFrom; 
    val.dateTo = this.dateTo; 
    this.partnerService.getCustomerStatistics(val).subscribe(
      result => {
        console.log(result);
        this.customerStatistics = result;
      },
      error => {
        console.log(error);
      }
    );
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.loadDataFromApi();
  }
}
