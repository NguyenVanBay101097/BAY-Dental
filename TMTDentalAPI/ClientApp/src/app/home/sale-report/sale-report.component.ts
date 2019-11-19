import { Component, OnInit } from '@angular/core';
import { HomeService } from '../home.service';
import { SaleReportTopServicesCs, SaleReportTopServicesFilter } from '../sale-report';
import { FormGroup, FormBuilder } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { formatNumber } from '@angular/common';
import { SaleReportItem, SaleReportService, SaleReportSearch } from 'src/app/sale-report/sale-report.service';
import { TaiDateRange } from 'src/app/core/tai-date-range';

@Component({
  selector: 'app-sale-report',
  templateUrl: './sale-report.component.html',
  styleUrls: ['./sale-report.component.css']
})
export class SaleReportComponent implements OnInit {
  public today: Date = new Date(new Date().toDateString());
  public weekStart: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1)));
  public weekEnd: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1) + 6));
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  options: TaiDateRange[] = [];
  optionSelected: TaiDateRange;
  items: SaleReportItem[] = [];
  fieldDisplay = 'productUOMQty';

  constructor(private saleReportService: SaleReportService, private intlService: IntlService) { }


  ngOnInit() {
    this.options = [
      { text: 'Tuần này', dateFrom: this.weekStart, dateTo: this.weekEnd },
      { text: 'Tháng này', dateFrom: this.monthStart, dateTo: this.monthEnd },
      { text: 'Tháng trước', dateFrom: this.getLastMonthStart(), dateTo: this.getLastMonthEnd() },
    ];
    this.optionSelected = this.options[0];
    this.loadData();
  }

  getLastMonthStart() {
    var month = this.monthStart.getMonth();
    if (month > 0) {
      return new Date(this.monthStart.getFullYear(), month - 1, 1);
    } else {
      return new Date(this.monthStart.getFullYear() - 1, 11, 1);
    }
  }

  getLastMonthEnd() {
    var lastMonthStart = this.getLastMonthStart();
    return new Date(lastMonthStart.getFullYear(), lastMonthStart.getMonth() + 1, 0);
  }

  selectOption(option: TaiDateRange) {
    this.optionSelected = option;
    this.loadData();
  }

  loadData() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(this.optionSelected.dateFrom, 'd', 'en-US');
    val.dateTo = this.intlService.formatDate(this.optionSelected.dateTo, 'd', 'en-US');
    val.groupBy = "product";
    this.saleReportService.getReport(val).subscribe(result => {
      this.items = result;
    });
  }

  public labelContent = (e: any) => {
    console.log(formatNumber(e.value, 'vi-VN'));
    return formatNumber(e.value, 'vi-VN');
  };
}
