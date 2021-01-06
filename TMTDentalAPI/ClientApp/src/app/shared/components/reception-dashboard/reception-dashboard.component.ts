import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { DataStateChangeEvent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { process, State } from '@progress/kendo-data-query';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountReportGeneralLedgerService, ReportCashBankGeneralLedger } from 'src/app/account-report-general-ledgers/account-report-general-ledger.service';
import { AppointmentPaged } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { LaboOrderReportInput, LaboOrderReportOutput, LaboOrderService } from 'src/app/labo-orders/labo-order.service';
import { PartnerCustomerReportInput, PartnerCustomerReportOutput, PartnerService } from 'src/app/partners/partner.service';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { SaleReportItem, SaleReportSearch, SaleReportService } from 'src/app/sale-report/sale-report.service';
import { DataBindingDirective } from '@progress/kendo-angular-grid';

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

  saleReport: SaleReportItem;
  limit = 20;
  offset = 0;
  appointmentStateCount: any = {};
  search: string = '';
  totalService: number;
  laboOrderReport: LaboOrderReportOutput;
  customerReport: PartnerCustomerReportOutput;
  reportValue: any;

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

  constructor(private intlService: IntlService,
    private appointmentService: AppointmentService,
    private saleReportService: SaleReportService,
    private laboOrderService: LaboOrderService,
    private partnerService: PartnerService,
    private router: Router,
    private authService: AuthService,
    private reportGeneralLedgerService: AccountReportGeneralLedgerService
  ) { }

  ngOnInit() {
    this.gridView = this.gridData;
    this.loadSaleReport();
    this.loadAppoiment();
    this.loadLaboOrderReport();
    this.loadPartnerCustomerReport();
    this.loadDataMoney();
    this.loadService();
  }

  loadSaleReport() {
    var val = new SaleReportSearch();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    // val.groupBy = "customer"
    this.saleReportService.getReport(val).subscribe(
      result => {
        if (result.length) {
          this.saleReport = result[0];
        } else {
          this.saleReport = null;
        }
      },
      error => {

      }
    );
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

  loadDataMoney() {
    var val = new ReportCashBankGeneralLedger();
    val.companyId = this.authService.userInfo.companyId;
    this.reportGeneralLedgerService.getCashBankReport(val).subscribe(result => {
      this.reportValue = result['accounts'].find(x => x.name == 'Tiền mặt');
    }, err => {
      console.log(err);
    });
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

  loadAppoiment() {
    var states = ["", "waiting,examination,done"];

    var obs = states.map(state => {
      var val = new AppointmentPaged();
      val.dateTimeFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
      val.dateTimeTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
      val.state = state;
      return this.appointmentService.getPaged(val);
    });

    forkJoin(obs).subscribe((result: any) => {
      this.appointmentStateCount['all'] = result[0].totalItems;
      this.appointmentStateCount['done'] = result[1].totalItems;
    });
  }

  loadLaboOrderReport() {
    var val = new LaboOrderReportInput();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');

    this.laboOrderService.getLaboOrderReport(val).subscribe(
      result => {
        this.laboOrderReport = result;
      },
      error => {

      }
    );
  }

  loadPartnerCustomerReport() {
    var val = new PartnerCustomerReportInput();
    val.dateFrom = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');
    val.dateTo = this.intlService.formatDate(new Date(), 'yyyy-MM-dd');

    this.partnerService.getPartnerCustomerReport(val).subscribe(
      result => {
        this.customerReport = result;
      },
      error => {

      }
    );
  }

  getStateDisplay(state) {
    switch(state) {
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
