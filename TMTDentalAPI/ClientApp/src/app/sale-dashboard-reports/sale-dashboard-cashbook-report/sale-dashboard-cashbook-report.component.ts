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
  // @Input() dateFrom: any;
  // @Input() dateTo: any;
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

  chartOptions: any = {}
  // chartOptions: any = {
  //   scaleShowVerticalLines: false,
  //   responsive: true,
  //   maintainAspectRatio: false,
  //   title: {
  //     text: 'BIỂU ĐỒ THU - CHI',
  //     display: true,
  //     fontSize: '16',
  //   },
  //   legend: { position: 'bottom', },
  //   scales: {
  //     xAxes: [{
  //       distribution: 'linear',
  //       type: 'time',
  //       time: {
  //         unit: 'day'
  //       },
  //       // type: "time",
  //       // time: {
  //       //   format: 'DD/MM/YYYY',
  //       //   // tooltipFormat: 'll',
  //       //   unit: 'month'
  //       // }
  //       ticks: {
  //         // maxTicksLimit: 10, //10
  //       }
  //     }],
  //     yAxes: [{
  //       // ticks: {
  //       //   min: 0,
  //       //   max: 100,
  //       //   stepSize: 20,
  //       // }
  //     }],

  //   }
  // };
  chartType: string = 'bar';
  dataSet: any[] = [];
  defaultDataSet = [{ x: '', y: 0 }];
  // dataSet: any[] = [
  //   {
  //     label: "# of Votes",
  //     fill: false,
  //     data: [
  //       { x: '', y: 0 }
  //       // { x: '2021-09-01', y: 20 }
  //     ]
  //   }
  // ];
  maxTicks: number = 11;

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

  loadChartOption() {
    this.chartOptions = {
      scaleShowVerticalLines: false,
      responsive: true,
      maintainAspectRatio: false,
      title: {
        text: 'BIỂU ĐỒ THU - CHI',
        display: true,
        fontSize: '16',
      },
      legend: { position: 'bottom', },
      tooltips: {
        mode: 'label',
        // usePointStyle: true,
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
          display: true,
          offset: true,
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
    let dataThu = [];
    let dataChi = [];
    let dataTonQuy = [];

    if (this.cashBooks && this.cashBooks.length > 0) {
      for (const data of this.cashBooks) {
        const date = this.intlService.formatDate(new Date(data.date), 'yyyy-MM-dd');
        const thu = Object.create(null);
        thu.x = date
        thu.y = data.totalThu;
        dataThu.push(thu);

        const chi = Object.create(null);
        chi.x = date
        chi.y = data.totalChi;
        dataChi.push(chi);

        const tonQuy = Object.create(null);
        tonQuy.x = date
        tonQuy.y = data.totalAmount;
        dataTonQuy.push(tonQuy);
      }
    } else {
      dataThu = this.defaultDataSet;
      dataChi = this.defaultDataSet;
      dataTonQuy = this.defaultDataSet;
    }
    
    this.dataSet = [
      { data: dataThu, order: 1, label: "Thu", backgroundColor: '#2395FF', hoverBackgroundColor: '#4FAAFF', borderColor: '#2395FF' },
      { data: dataChi, order: 2, label: "Chi", backgroundColor: '#28A745', hoverBackgroundColor: '#53B96A', borderColor: '#28A745' },
      { data: dataTonQuy, order: 0, label: "Tồn Quỹ", fill: false, type: "line", backgroundColor: '#ff0000', hoverBackgroundColor: '#ff0000', borderColor: '#ff0000' }
    ]
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