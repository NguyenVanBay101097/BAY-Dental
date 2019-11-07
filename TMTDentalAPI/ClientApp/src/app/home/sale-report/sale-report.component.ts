import { Component, OnInit } from '@angular/core';
import { HomeService } from '../home.service';
import { SaleReportTopServicesCs, SaleReportTopServicesFilter } from '../sale-report';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { formatNumber } from '@angular/common';

@Component({
  selector: 'app-sale-report',
  templateUrl: './sale-report.component.html',
  styleUrls: ['./sale-report.component.css']
})
export class SaleReportComponent implements OnInit {

  constructor(private fb: FormBuilder, private homeService: HomeService) { }

  topServiceItems = new Array<SaleReportTopServicesCs>();
  invoiceOrQty = true;
  limit = 5;

  formFilter: FormGroup;
  ngOnInit() {
    this.formFilter = this.fb.group({
      by: 'byInvoice',
      limit: 5
    })
    this.getTopServices();
  }

  getTopServices() {
    var val = new SaleReportTopServicesFilter();
    if (this.invoiceOrQty) {
      val.byInvoice = true;
      val.byQty = false;
    } else {
      val.byInvoice = false;
      val.byQty = true;
    }
    val.number = this.limit;
    this.homeService.getSaleReportTopService(val).subscribe(
      rs => {
        this.topServiceItems = rs;
      }
    )
  }

  changeFilter(e: string) {
    if (e == 'byInvoice') {
      this.invoiceOrQty = true;
      this.getTopServices();
    }
    else if (e == 'byQty') {
      this.invoiceOrQty = false;
      this.getTopServices();
    }
  }

  changeLimit(e: string) {
    this.limit = parseInt(e);
    this.getTopServices();
  }


  public labelContent = (e: any) => {
    console.log(formatNumber(e.value, 'vi-VN'));
    return formatNumber(parseInt(e.value), 'vi-VN');
  };
}
