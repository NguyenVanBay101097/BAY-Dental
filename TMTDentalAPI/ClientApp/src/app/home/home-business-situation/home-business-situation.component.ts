import { Component, OnInit } from '@angular/core';
import { TaiDateRange } from 'src/app/core/tai-date-range';
import { SaleReportService, SaleReportSearch, SaleReportItem } from 'src/app/sale-report/sale-report.service';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-home-business-situation',
  templateUrl: './home-business-situation.component.html',
  styleUrls: ['./home-business-situation.component.css']
})
export class HomeBusinessSituationComponent implements OnInit {
  public today: Date = new Date(new Date().toDateString());
  public yesterday: Date = new Date(new Date(new Date().setDate(new Date().getDate() - 1)).toDateString());
  public weekStart: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1)));
  public weekEnd: Date = new Date(new Date().setDate(new Date().getDate() - new Date().getDay() + (new Date().getDay() == 0 ? -6 : 1) + 6));
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 0).getDate())).toDateString());
  options: TaiDateRange[] = [];
  optionSelected: TaiDateRange;
  item: SaleReportItem;
  constructor(private saleReportService: SaleReportService, private intlService: IntlService) { }

  ngOnInit() {
    this.options = [
      { text: 'Hôm nay', dateFrom: this.today, dateTo: this.today },
      { text: 'Hôm qua', dateFrom: this.yesterday, dateTo: this.yesterday },
      { text: 'Tuần này', dateFrom: this.weekStart, dateTo: this.weekEnd },
      { text: 'Tháng này', dateFrom: this.monthStart, dateTo: this.monthEnd },
    ];
    this.optionSelected = this.options[0];
    this.loadData();
  }

  selectOption(option: TaiDateRange) {
    this.optionSelected = option;
    this.loadData();
  }

  loadData() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(this.optionSelected.dateFrom, 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(this.optionSelected.dateTo, 'yyyy-MM-dd');
    val.groupBy
    this.saleReportService.getReport(val).subscribe(result => {
      if (result.length) {
        this.item = result[0];
      } else {
        this.item = null;
      }
    });
  }
}
