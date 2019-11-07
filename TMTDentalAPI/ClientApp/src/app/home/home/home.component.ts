import { Component, OnInit } from '@angular/core';
import { AccountInvoiceReportService, AccountInvoiceReportHomeSummaryVM } from 'src/app/account-invoice-reports/account-invoice-report.service';
import { HomeService, TopServices } from '../home.service';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { ProductService } from 'src/app/products/product.service';

export class AppointmentState {
  state: string;
  count: number;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class HomeComponent implements OnInit {
  todaySummary: AccountInvoiceReportHomeSummaryVM;

  constructor(private intlService: IntlService, private homeService: HomeService,
    private reportService: AccountInvoiceReportService, private productService: ProductService) { }

  invoiceItems = [];
  appointItems = [];

  groupBy = 'day';

  appointLabel = 'Lịch hẹn';
  appointNull = false;
  invoiceLabel = 'Doanh thu';
  isAppointEmpty = false;
  isInvoiceEmpty = false;


  ngOnInit() {
    //Doanh thu
    this.getAmmountResidual();
    //Đếm lịch hẹn
    this.countAppoint();
    //Top dịch vụ
  }

  countAppoint() {
    var today = new Date();
    var todayString = this.intlService.formatDate(today, 'd', 'en-US');
    this.homeService.getAppointCount(todayString, todayString).subscribe(
      rs => {
        if (rs.filter(x => x.count > 0).length > 0) {
          this.appointItems = rs;
        } else {
          this.appointLabel = this.nullOrFailedAppoint;
        }
      },
      err => {
        console.log(err);
      }
    )
  }

  get nullOrFailedAppoint() {
    this.appointItems = [{ "state": "confirmed", "color": "#04c835", "count": 1 }, { "state": "cancel", "color": "#cc0000", "count": 1 }, { "state": "done", "color": "#666666", "count": 1 }, { "state": "waiting", "color": "#0080ff", "count": 1 }, { "state": "expired", "color": "#ffbf00", "count": 1 }];
    this.isAppointEmpty = true;
    return "Không có lịch hẹn nào";
  }

  getAmmountResidual() {
    this.homeService.getAmountResidualToday().subscribe(
      res => {
        if (res.filter(x => x.value > 0).length > 0) {
          this.invoiceItems = res;
        } else {
          this.nullOrFailedInvoice();
        }
      }, err => {
        console.log(err);
        this.nullOrFailedInvoice();
      })
  }

  nullOrFailedInvoice() {
    this.invoiceItems = [{ "name": "AmountTotal", "value": 1 }, { "name": "Residual", "value": 1 }];
    this.isInvoiceEmpty = true;
    this.invoiceLabel = "Không có doanh thu";
  }


  getState(e: any): string {
    switch (e.category) {
      case 'done':
        return 'Kết thúc' + ' \n(' + e.value + ')';
      case 'cancel':
        return 'Đã hủy' + ' \n(' + e.value + ')';
      case 'waiting':
        return 'Đang chờ' + ' \n(' + e.value + ')';
      case 'expired':
        return 'Quá hạn' + ' \n(' + e.value + ')';
      default:
        return 'Đang hẹn' + ' \n(' + e.value + ')';
    }
  }

  labelAmRs(e: any): string {
    switch (e.category) {
      case 'AmountTotal':
        return 'Tổng số' + ' \n' + e.value.toLocaleString('vi');
      default:
        return 'Còn nợ' + ' \n' + e.value.toLocaleString('vi');
    }
  }

  sample() {
    this.invoiceItems = [{ "name": "AmountTotal", "value": 0.0 }, { "name": "Residual", "value": 0.0 }];
    this.appointItems = [{ "state": "confirmed", "color": "#04c835", "count": 3 }, { "state": "cancel", "color": "#cc0000", "count": 1 }, { "state": "done", "color": "#666666", "count": 1 }, { "state": "waiting", "color": "#0080ff", "count": 2 }, { "state": "expired", "color": "#ffbf00", "count": 2 }];

  }
}