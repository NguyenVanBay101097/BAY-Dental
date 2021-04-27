import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { DataStateChangeEvent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { process, State } from '@progress/kendo-data-query';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppointmentPaged } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { LaboOrderReportOutput } from 'src/app/labo-orders/labo-order.service';
import { SaleReportSearch, SaleReportService } from 'src/app/sale-report/sale-report.service';
import { DataBindingDirective } from '@progress/kendo-angular-grid';
import { CashBookService } from 'src/app/cash-book/cash-book.service';
import { CheckPermissionService } from '../../check-permission.service';

@Component({
  selector: 'app-reception-dashboard',
  templateUrl: './reception-dashboard.component.html',
  styleUrls: ['./reception-dashboard.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ReceptionDashboardComponent implements OnInit {
  @ViewChild(DataBindingDirective, { static: true }) dataBinding: DataBindingDirective;

  limit = 20;
  offset = 0;
  appointmentStateCount: any = {};
  search: string = '';
  totalService: number;
  reportValueCashByDate: any;
  reportValueBankByDate: any;
  public state: State = {
    skip: this.offset,
    take: this.limit,
    // Initial filter descriptor
    filter: {
      logic: 'and',
      filters: [{ field: 'name', operator: 'contains', value: '' }]
    }
  };
  public gridData: any[];
  public gridView: any[];

  // permission
  showCashBankReport = this.checkPermissionService.check('Report.CashBankAccount');
  showLaboOrderReport = this.checkPermissionService.check('');
  showPartnerCustomerReport = this.checkPermissionService.check('Report.PartnerOldNew');
  showSaleReport = this.checkPermissionService.check('Report.Sale');

  constructor(private intlService: IntlService,
    private appointmentService: AppointmentService,
    private saleReportService: SaleReportService,
    private router: Router,
    private authService: AuthService,
    private cashBookService: CashBookService, 
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.gridView = this.gridData;
    this.loadAppoiment();
    this.loadDataMoneyByDateTime();
    this.loadService();
  }

  loadService() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    val.companyId = this.authService.userInfo.companyId;
    val.search = this.search;
    val.state = 'draft';
    this.saleReportService.getReportService(val).pipe(
      map((response: any) =>
      (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridView = res.data;
      this.gridData = res.data;
      this.totalService = res.total;
    }, err => {
      console.log(err);
    })
  }

  loadDataMoneyByDateTime() {
    var dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    var dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    var companyId = this.authService.userInfo.companyId;

    let cash = this.cashBookService.getTotal({ resultSelection: "cash", dateFrom: dateFrom, dateTo: dateTo, companyId: companyId });
    let bank = this.cashBookService.getTotal({ resultSelection: "bank", dateFrom: dateFrom, dateTo: dateTo, companyId: companyId });

    forkJoin([cash, bank]).subscribe(results => {
      this.reportValueCashByDate = results[0];
      this.reportValueBankByDate = results[1];
    });
  }

  loadAppoiment() {
    var states = ["", "waiting,examination,done"];

    var obs = states.map(state => {
      var val = new AppointmentPaged();
      val.dateTimeFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
      val.dateTimeTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
      val.companyId = this.authService.userInfo.companyId;
      val.state = state;
      return this.appointmentService.getPaged(val);
    });

    forkJoin(obs).subscribe((result: any) => {
      this.appointmentStateCount['all'] = result[0].totalItems;
      this.appointmentStateCount['done'] = result[1].totalItems;
    });
  }

  exportServiceExcelFile() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-ddT23:59');
    val.companyId = this.authService.userInfo.companyId;
    val.search = this.search;
    val.state = 'draft';
    // paged.categoryId = this.searchCateg ? this.searchCateg.id : null;
    this.saleReportService.exportServiceReportExcelFile(val).subscribe((rs) => {
      let filename = "danh_sach_dich_vu_trong_ngay";
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.download = filename;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }

  public dataStateChange(state: DataStateChangeEvent): void {
    this.state = state;
    this.loadService();
  }

  public onFilter(inputValue: string): void {
    this.gridView = process(this.gridData, {
      filter: {
        logic: "or",
        filters: [
          {
            field: 'order.name',
            operator: 'contains',
            value: inputValue
          },
          {
            field: 'orderPartner.name',
            operator: 'contains',
            value: inputValue
          },
          {
            field: 'employee.name',
            operator: 'contains',
            value: inputValue
          },
          {
            field: 'name',
            operator: 'contains',
            value: inputValue
          }
        ],
      }
    }).data;

    this.dataBinding.skip = 0;
  }

  pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.loadService();
  }

  redirectSaleOrder(item) {
    if (item) {
      this.router.navigateByUrl(`sale-orders/form?id=${item.id}`)
    }
  }

  stateGet(state) {
    switch (state) {
      case 'waiting':
        return 'Chờ khám';
      case 'examination':
        return 'Đang khám';
      case 'done':
        return 'Hoàn thành';
      case 'cancel':
        return 'Hủy hẹn';
      case 'all':
        return 'Tổng hẹn';
      default:
        return 'Đang hẹn';
    }
  }

  onAppointmentTodayChange() {
    this.loadAppoiment();
  }




  getStateDisplay(state) {
    switch (state) {
      case 'sale':
        return 'Đang điều trị';
      case 'done':
        return 'Hoàn thành';
      default:
        return 'Nháp';
    }
  }

  getTeethDisplay(teeth) {
    return teeth.map(x => x.name).join(',');
  }
}
