import { filter } from 'rxjs/operators';
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { IntlService, load } from '@progress/kendo-angular-intl';
import { CashBookReportFilter, CashBookReportItem, CashBookService } from 'src/app/cash-book/cash-book.service';
import { ChartOptions, ChartType } from 'chart.js';
import { Label, SingleDataSet } from 'ng2-charts';

@Component({
  selector: 'app-sale-dashboard-cashbook-report',
  templateUrl: './sale-dashboard-cashbook-report.component.html',
  styleUrls: ['./sale-dashboard-cashbook-report.component.css']
})
export class SaleDashboardCashbookReportComponent implements OnInit {
  @Input() groupby: string;
  @Input() cashBooks: any;
  @Input() dataCashBooks: any;
  @Input() totalDataCashBook: any;
  @Input() dateFrom: any;
  @Input() dateTo: any;
  cashBookData: CashBookReportItem[] = [];
  cashbookThu: any[] = [];
  cashbookChi: any[] = [];
  cashbookTotal: any[] = [];
  // cashbookCategs: any[] = [];
  cashbookSeries: any[] = [];
  public cashbookCashBank: any;
  public cashbookCusDebt: any;
  public cashbookCusAdvance: any;
  public cashbookSuppRefund: any;
  public cashbookSupp: any;
  public cashbookCusSalary: any;
  public cashbookAgentCommission: any;
  public totalCashbook: any;

  barChartType = 'bar';
  barChartLegend = true;
  barChartLabels: any[] = [];
  dataThu: any[] = [];
  dataChi: any[] = [];
  dataTonQuy: any[] = [];
  barChartOptions: any;
  barChartData = [
    { data: this.dataThu, label: 'Thu', order: 1, backgroundColor: '#2395FF', hoverBackgroundColor: '#4FAAFF' },
    { data: this.dataChi, label: 'Chi', order: 2, backgroundColor: '#28A745', hoverBackgroundColor: '#53B96A' },
    { data: this.dataTonQuy, type: "line", order: 0, fill: false, label: 'Tồn sổ quỹ', backgroundColor: '#ff0000', hoverBackgroundColor: '#ff0000', borderColor: '#ff0000' },
  ];

  constructor(private cashBookService: CashBookService,
    private router: Router,
    private intlService: IntlService) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadDataApi();
    this.loadChartData();
    this.loadChartOption();
  }

  ngOnInit() {
    this.loadDataApi();
  }

  getDaysArray(start, end) {
    for (var arr = [], date = new Date(start); date <= end; date.setDate(date.getDate() + 1)) {
      arr.push(this.intlService.formatDate(new Date(date), 'yyyy-MM-ddTHH:mm:ss'));
    }
    return arr;
  };

  getMonthsArray(start, end) {
    let arr = [];
    if (start && end) {
      for (let date = new Date(start); date <= end; date.setDate(date.getDate() + 1)) {
        arr.push(this.intlService.formatDate(new Date(date.getFullYear(), date.getMonth(), 1), 'yyyy-MM-ddTHH:mm:ss'));
      }
      arr = [...new Set(arr)];
    } else {
      let year = new Date().getFullYear();
      for (let i = 1; i <= 12; i++) {
        arr.push(this.intlService.formatDate(new Date(`${year}-${i}-01 00:00`), 'yyyy-MM-ddTHH:mm:ss'))
      }
    }
    return arr;
  };

  loadChartOption() {
    var tickLimit = 0;
    const length = this.barChartLabels.length;
    if (this.groupby == 'groupby:day' && length < 32)
      tickLimit = 31;
    else if (this.groupby == 'groupby:day' && length > 31 && length < 61)
      tickLimit = Math.floor(length / 2) + 1;
    else if (this.groupby == 'groupby:day' && length > 60 && length < 91)
      tickLimit = Math.floor(length / 7) + 1;
    else if (this.groupby == 'groupby:day' && length > 90 && length < 211)
      tickLimit = Math.floor(length / 13) + 1;
    else
      tickLimit = Math.floor(length / 30) + 1;

    this.barChartOptions = {
      scaleShowVerticalLines: false,
      responsive: true,
      legend: {
        position: 'bottom',
      },
      tooltips: {
        mode: 'label',
        borderWidth: 0,
      },
      title: {
        text: 'BIỂU ĐỒ THU CHI',
        display: true,
        fontSize: '16',
      },
      scales: {
        xAxes: [{
          ticks: {
            maxTicksLimit: tickLimit, //10
          }
        }],
        yAxes: [{
          ticks: {
            // callback: function (val, index) {
            //   // return Intl.NumberFormat().format(val);
            //   return val / 1000000 + ' triệu';
            // },
          }
        }]
      }
    };
  }

  loadChartData() {
    if (this.cashBooks) {
      this.dataThu = [];
      this.dataChi = [];
      this.dataTonQuy = [];
      this.barChartLabels = [];

      var cashBooksData = this.cashBooks.reduce(function (map, obj) {
        map[obj.date] = obj;
        return map;
      }, Object.create(null));

      if (this.groupby == 'groupby:day') {
        let dateArr = this.getDaysArray(this.dateFrom, this.dateTo);
        console.log(dateArr);
        for (let key of dateArr) {
          const value = cashBooksData[key];
          this.dataThu.push(value ? value.totalThu : 0);
          this.dataChi.push(value ? value.totalChi : 0);
          this.dataTonQuy.push(value ? value.totalAmount : 0);
        }
        this.barChartLabels = dateArr.map(date => this.intlService.formatDate(new Date(date), 'dd/MM'));
      } else {
        let monthArr = this.getMonthsArray(this.dateFrom, this.dateTo);
        for (let key of monthArr) {
          const value = cashBooksData[key];
          this.dataThu.push(value ? value.totalThu : 0);
          this.dataChi.push(value ? value.totalChi : 0);
          this.dataTonQuy.push(value ? value.totalAmount : 0);
        }
        this.barChartLabels = monthArr.map(date => this.intlService.formatDate(new Date(date), 'MM/yyyy'));
      }
      this.barChartData = [
        { data: this.dataThu, label: 'Thu', order: 1, backgroundColor: '#2395FF', hoverBackgroundColor: '#4FAAFF' },
        { data: this.dataChi, label: 'Chi', order: 2, backgroundColor: '#28A745', hoverBackgroundColor: '#53B96A' },
        { data: this.dataTonQuy, type: "line", order: 0, fill: false, label: 'Tồn sổ quỹ', backgroundColor: '#ff0000', hoverBackgroundColor: '#ff0000', borderColor: '#ff0000' },
      ];
    }
  }

  loadDataApi() {
    if (this.cashBooks) {
      this.cashBookData = this.cashBooks;
      this.cashbookSeries = [];
      // this.loadCashbookGroupby();
      this.loadDataCashbookSeries();
    }
  }

  // public labelContent = (e: any) => {
  //   var res = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(e.value), 'dd/MM/yyyy') : this.intlService.formatDate(new Date(e.value), 'MM/yyyy');
  //   return res;
  // };


  loadDataCashbookSeries() {
    if (this.dataCashBooks && this.totalDataCashBook) {
      this.cashbookCashBank = this.dataCashBooks[0];
      this.cashbookCusDebt = this.dataCashBooks[1];
      this.cashbookCusAdvance = this.dataCashBooks[2];
      this.cashbookSupp = this.dataCashBooks[3];
      this.cashbookCusSalary = this.dataCashBooks[4];
      this.cashbookAgentCommission = this.dataCashBooks[5];
      this.totalCashbook = this.totalDataCashBook;
      this.cashbookThu = this.cashBookData.map(x => x.totalThu);
      this.cashbookChi = this.cashBookData.map(x => x.totalChi);
      this.cashbookTotal = this.cashBookData.map(x => x.totalAmount);
    }

  }

  get ortherThu() {
    if (this.totalCashbook && this.dataCashBooks) {
      return (this.totalCashbook.totalThu ? this.totalCashbook.totalThu : 0) - (this.dataCashBooks.reduce((total, val) => total += val.credit, 0));
    }

    return 0;
  }

  get ortherChi() {
    if (this.totalCashbook) {
      return (this.totalCashbook.totalChi ? this.totalCashbook.totalChi : 0) - (this.cashbookSupp.debit + this.cashbookCusAdvance.debit + this.cashbookCusSalary.debit + this.cashbookAgentCommission.debit);
    }

    return 0;

  }

  get totalThu() {
    return (this.cashbookCashBank ? this.cashbookCashBank.credit : 0) + (this.cashbookCusAdvance ? this.cashbookCusAdvance.credit : 0) + (this.cashbookCusDebt ? this.cashbookCusDebt.credit : 0) + (this.cashbookSupp ? this.cashbookSupp.credit : 0) + this.ortherThu;
  }

  get totalChi() {
    return (this.cashbookSupp ? this.cashbookSupp.debit : 0) + (this.cashbookCusAdvance ? this.cashbookCusAdvance.debit : 0) + (this.cashbookCusSalary ? this.cashbookCusSalary.debit : 0) + (this.cashbookAgentCommission ? this.cashbookAgentCommission.debit : 0) + this.ortherChi;
  }

  // loadCashbookGroupby() {
  //   if (this.cashBookData) {
  //     this.cashbookCategs = this.cashBookData.map(s => s.date).sort();
  //   }
  // }

  redirectTo() {
    return this.router.navigateByUrl("cash-book/tab-cabo");
  }

}


[
  '2021-08-01', '2021-09-01', '2021-10-01'
]