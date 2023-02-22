import { Component, OnInit } from '@angular/core';
import { SaleReportOldNewPartnerInput, SaleReportOldNewPartnerOutput, SaleReportService } from '../sale-report.service';

@Component({
  selector: 'app-sale-report-old-new-partner',
  templateUrl: './sale-report-old-new-partner.component.html',
  styleUrls: ['./sale-report-old-new-partner.component.css']
})
export class SaleReportOldNewPartnerComponent implements OnInit {

  dateFrom: any;
  dateTo: any;

  monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  saleReportOldNewPartner: SaleReportOldNewPartnerOutput;

  constructor(private saleReportService: SaleReportService) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.loadDataFromApi();
  }

  loadDataFromApi() {
    var val = new SaleReportOldNewPartnerInput(); 
    val.dateFrom = this.dateFrom; 
    val.dateTo = this.dateTo; 
    this.saleReportService.getReportOldNewPartner(val).subscribe(
      result => {
        this.saleReportOldNewPartner = result;
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
