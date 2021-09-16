import { KeyValue } from '@angular/common';
import { AfterViewInit, Component, ElementRef, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { IntlService } from '@progress/kendo-angular-intl';
import { AccountInvoiceReportService, RevenueReportFilter, RevenueReportItem } from 'src/app/account-invoice-reports/account-invoice-report.service';
import { CashBookReportItem } from 'src/app/cash-book/cash-book.service';
import { RevenueReportService } from 'src/app/revenue-report/revenue-report.service';
import * as Chart from 'chart.js';

@Component({
  selector: 'app-sale-dashboard-invoice-report',
  templateUrl: './sale-dashboard-invoice-report.component.html',
  styleUrls: ['./sale-dashboard-invoice-report.component.css']
})
export class SaleDashboardInvoiceReportComponent implements AfterViewInit {
  @Input() groupby: string;
  @Input() revenues: RevenueReportItem[] = [];
  @Input() cashBooks: any;
  @Input() dataRevenues: any[] = [];
  @Input() dataCashBooks: any;
  @Input() totalDataCashBook: any;
  // @Input() dateFrom: any;
  // @Input() dateTo: any;
  barChart: any;
  cashBookData: CashBookReportItem[] = [];
  revenuCateg: any[] = [];
  dateCount: number = 0;
  public revenueCashBank: any;
  public revenueDebt: any;
  public revenueAdvance: any;
  public revenueCusDebt: any;
  public revenueCusAdvance: any;
  public cashbookCashBank: any;
  public cashbookCusDebt: any;
  public cashbookCusAdvance: any;
  revenueSeries: any[] = [];
  revenue: any;
  realIncome: any;
  data: any = {};
  options: any = {};
  maxTicks: number = 11;
  dataSet: any[] = [];

  constructor(private revenueReportService: AccountInvoiceReportService,
    private intlService: IntlService) { }
  ngAfterViewInit(): void {
    // this.barChartMethod();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadRevenueSeries();
    this.loadDataCashbookSeries();
    // this.barChartMethod();
    this.loadChartData();
    this.loadChartOption();
  }

  loadRevenueSeries() {
    if (this.dataRevenues) {
      this.revenueCashBank = this.dataRevenues[0];
      this.revenueCusDebt = this.dataRevenues[1];
      this.revenueCusAdvance = this.dataRevenues[2];
    }
  }

  loadDataCashbookSeries() {
    this.revenueSeries = [];
    if (this.cashBooks && this.revenues && this.dataCashBooks) {
      this.cashBookData = this.cashBooks;
      this.loadCashbookGroupby();
      // this.realIncome = this.loadDataColCashBook();
      // this.revenue = this.loadDataColRevenue();
      this.cashbookCashBank = this.dataCashBooks[0];
      this.cashbookCusDebt = this.dataCashBooks[1];
      this.cashbookCusAdvance = this.dataCashBooks[2];
    }

  }

  loadDataColRevenue() {
    let res = [];
    this.revenuCateg.forEach(x => {
      var total = this.revenues.find(s => (this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(s.invoiceDate), 'dd/MM/yyyy') : this.intlService.formatDate(new Date(s.invoiceDate), 'MM/yyyy')) == x);
      var item = { date: total ? total.invoiceDate : this.intlService.formatDate(new Date(x), 'yyyy-MM-ddT00:00:00'), total: total ? total.priceSubTotal : 0 };
      res.push(item);
    })

    return res;
  }

  loadDataColCashBook() {
    let res = [];
    this.revenuCateg.forEach(x => {
      var total = this.cashBookData.find(s => (this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(s.date), 'dd/MM/yyyy') : this.intlService.formatDate(new Date(s.date), 'MM/yyyy')) == x);
      var item = { date: total ? total.date : this.intlService.formatDate(new Date(x), 'yyyy-MM-ddT00:00:00'), total: total ? total.totalThu : 0 };
      res.push(item);
    })
    return res;
  }

  get totalDebit() {
    if (this.revenueCashBank) {
      return (this.revenueCashBank.balance ? this.revenueCashBank.balance : 0) + (this.revenueCusDebt.debit || 0) + (this.revenueCusAdvance.debit || 0);
    }

    return 0;
  }

  get ortherThu() {
    if (this.totalDataCashBook && this.cashbookCashBank) {
      return (this.totalDataCashBook.totalThu || 0) - ((this.cashbookCashBank.credit ? this.cashbookCashBank.credit : 0) + (this.cashbookCusDebt.credit || 0) + (this.cashbookCusAdvance.credit || 0));
    }

    return 0;
  }

  loadCashbookGroupby() {

    if (this.revenues && this.cashBookData) {
      var dateRevenues = this.revenues.map(s => this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(s.invoiceDate), 'yyyy-MM-ddT00:00:00') : this.intlService.formatDate(new Date(s.invoiceDate), 'MM/yyyy'));
      var dateCashbooks = this.cashBookData.map(s => this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(s.date), 'yyyy-MM-ddT00:00:00') : this.intlService.formatDate(new Date(s.date), 'MM/yyyy'));
      this.revenuCateg = this.arrayUnique(dateRevenues.concat(dateCashbooks).sort());

    }
  }

  arrayUnique(array) {
    var a = array.concat();
    for (var i = 0; i < a.length; ++i) {
      for (var j = i + 1; j < a.length; ++j) {
        if (a[i] === a[j])
          a.splice(j--, 1);
      }
    }
    return a;
  }

  public labelContent = (e: any) => {
    var res = e.value;
    return res;
  };
  
  loadChartOption() {
    this.options = {
      scaleShowVerticalLines: false,
      responsive: true,
      maintainAspectRatio: false,
      title: {
        text: 'BIỂU ĐỒ DOANH THU - THỰC THU',
        display: true,
        fontSize: '16',
      },
      legend: { position: 'bottom', },
      tooltips: {
        mode: 'label',
        titleFontStyle: 'bold',
        borderWidth: 1,
        callbacks: {
          label: function (tooltipItem, data) {
            let labelContent = data.datasets[tooltipItem.datasetIndex].label;
            const labelData = Number(tooltipItem.yLabel).toFixed(0).replace(/./g, function (c, i, a) {
              return i > 0 && c !== "," && (a.length - i) % 3 === 0 ? "." + c : c;
            });
            return `${labelContent}: ${labelData}`;
          }
        }
      },
      scales: {
        xAxes: [{
          distribution: 'linear',
          type: 'time',
          time: {
            tooltipFormat: this.groupby == 'groupby:day' ? 'DD/MM/YYYY' : 'MM/YYYY',
            displayFormats: {
              'day': 'DD/MM',
              'month': 'MM/YYYY',
            },
            unit: this.groupby == 'groupby:day' ? 'day' : 'month'
          },
          ticks: {
            maxTicksLimit: this.maxTicks
          }
        }],
        yAxes: [{
          ticks: {
            beginAtZero: true,
            callback: function (val, index) {
              return Intl.NumberFormat().format(val);
            },
          }
        }],
      }
    }
  }

  loadChartData() {
    let totalRevenues = [];
    let totalCashBooks = [];
    if (this.revenues) {
      for (const data of this.revenues) {
        const date = this.intlService.formatDate(new Date(data.invoiceDate), 'yyyy-MM-dd');
        const val = Object.create(null);
        val.x = date
        val.y = data.priceSubTotal;
        totalRevenues.push(val);
      }
    }

    if (this.cashBooks) {
      for (const data of this.cashBooks) {
        const date = this.intlService.formatDate(new Date(data.date), 'yyyy-MM-dd');
        const val = Object.create(null);
        val.x = date
        val.y = data.totalThu;
        totalCashBooks.push(val);
      }
      this.dataSet = [
        { data: totalRevenues, label: "Doanh thu", backgroundColor: '#2395FF', hoverBackgroundColor: '#4FAAFF', borderColor: '#2395FF' },
        { data: totalCashBooks, label: "Thực thu", backgroundColor: '#28A745', hoverBackgroundColor: '#53B96A', borderColor: '#28A745' },
      ]
    }
  }

  // barChartMethod() {
  //   var dates = [];
  //   var totals = [];
  //   var dateBettween = this.getAllDateBettween(this.dateFrom, this.dateTo);
  //   this.dateCount = dateBettween.length;
  //   var dateArr = this.arrayUnique(dateBettween.concat(this.revenuCateg));
  //   if (this.revenues) {
  //     dateArr.forEach(x => {
  //       var item = this.revenues.find(s => (this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(s.invoiceDate), 'yyyy-MM-ddT00:00:00') : this.intlService.formatDate(new Date(s.invoiceDate), 'MM/yyyy')) == x);
  //       // var item = {date: total ? total.invoiceDate : this.intlService.formatDate(new Date(x), 'yyyy-MM-ddT00:00:00') , total: total ? total.priceSubTotal : 0};
  //       // var date = item ? this.intlService.formatDate(new Date(item.invoiceDate), 'dd/MM/yyyy') : this.intlService.formatDate(new Date(x), 'dd/MM/yyyy');
  //       if (item) {
  //         var date = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(item.invoiceDate), 'dd/MM/yyyy') : this.intlService.formatDate(new Date(item.invoiceDate), 'MM/yyyy');
  //         var total = item.priceSubTotal;
  //         dates.push(date);
  //         totals.push(total);
  //       }
  //       else {
  //         var date = this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(x), 'dd/MM/yyyy') : this.intlService.formatDate(new Date(x), 'MM/yyyy');
  //         var total = 0;
  //         dates.push(date);
  //         totals.push(total);
  //       }

  //     });
  //   }

  //   // var realIncomeDates = [];
  //   var realTotals = [];
  //   dateArr.forEach(x => {
  //     var item = this.cashBookData.find(s => (this.groupby == 'groupby:day' ? this.intlService.formatDate(new Date(s.date), 'yyyy-MM-ddT00:00:00') : this.intlService.formatDate(new Date(s.date), 'MM/yyyy')) == x);
  //     // var item = {date: total ? total.date : this.intlService.formatDate(new Date(x), 'yyyy-MM-ddT00:00:00') , total: total ? total.totalThu : 0};
  //     var total = item ? item.totalThu : 0;
  //     realTotals.push(total);
  //   })
  //   var tickLimit = 0;
  //   if (this.groupby == 'groupby:day' && this.dateCount < 32)
  //     tickLimit = 31;
  //   else if (this.groupby == 'groupby:day' && this.dateCount > 31 && this.dateCount < 61)
  //     tickLimit = Math.floor(this.dateCount / 2) + 1;
  //   else if (this.groupby == 'groupby:day' && this.dateCount > 60 && this.dateCount < 91)
  //     tickLimit = Math.floor(this.dateCount / 7) + 1;
  //   else if (this.groupby == 'groupby:day' && this.dateCount > 90 && this.dateCount < 211)
  //     tickLimit = Math.floor(this.dateCount / 13) + 1;
  //   else
  //     tickLimit = Math.floor(this.dateCount / 30) + 1;
  //   this.data = {
  //     labels: dates,
  //     datasets: [{
  //       label: 'Doanh thu',
  //       data: totals,
  //       backgroundColor: 'rgba(35, 149, 255, 1)',
  //       hoverBackgroundColor: 'rgba(35, 149, 255, 0.8)',
  //       hoverBorderColor: 'rgba(35, 149, 255, 1)'
  //     },
  //     {
  //       label: 'Thực thu',
  //       data: realTotals,
  //       backgroundColor: 'rgba(40, 167, 69, 1)',
  //       hoverBackgroundColor: 'rgba(40, 167, 69, 0.8)',
  //       hoverBorderColor: 'rgba(40, 167, 69, 1)'
  //     }
  //     ]
  //   };

  //   // this.options = {
  //   //   scales: {
  //   //     xAxes: [{
  //   //       ticks: {
  //   //         maxTicksLimit: tickLimit,
  //   //         // stepSize: 5
  //   //       }
  //   //     }],
  //   //     yAxes: [{
  //   //       ticks: {
  //   //         beginAtZero: true,
  //   //         callback: function (val, index) {
  //   //           return Intl.NumberFormat().format(val)
  //   //         }
  //   //       }
  //   //     }]
  //   //   },
  //   //   responsive: true,
  //   //   tooltips: {
  //   //     mode: 'label',
  //   //     borderWidth: 0
  //   //   },
  //   //   legend: {
  //   //     position: 'bottom'
  //   //   },
  //   //   title: {
  //   //     text: 'BIỂU ĐỒ DOANH THU - THỰC THU',
  //   //     display: true,
  //   //     fontSize: '16',
  //   //   },

  //   // };


  // }

  // getAllDateBettween(startDate, endDate) {
  //   var arr = [];
  //   if (this.groupby == 'groupby:day') {
  //     for (var date = new Date(startDate); date <= endDate; date.setDate(date.getDate() + 1)) {
  //       arr.push(this.intlService.formatDate(new Date(date), 'yyyy-MM-ddTHH:mm:ss'));
  //     }
  //   }
  //   else {
  //     if (startDate && endDate) {
  //       for (const val of this.cashBooks) {
  //         arr.push(this.intlService.formatDate(new Date(val.date), 'MM/yyyy'));
  //       }
  //     } else {
  //       let year = new Date().getFullYear();
  //       for (let i = 1; i <= 12; i++) {
  //         arr.push(this.intlService.formatDate(new Date(`${year}-${i}-01 00:00`), 'MM/yyyy'))
  //       }
  //     }
  //   }
  //   return arr;
  // }

  // getMonthsArray(start, end) {
  //   let arr = [];

  //   return arr;
  // };

}
