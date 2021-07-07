import { AuthService } from 'src/app/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AppointmentGetCountVM } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { CashBookService, CashBookSummarySearch } from 'src/app/cash-book/cash-book.service';
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
  // permission
  canAppointmentLink = this.checkPermissionService.check(['Basic.Appointment.Read']);
  canCustomerLink = this.checkPermissionService.check(['Basic.Partner.Read']);
  canPurchaseOrderLink = this.checkPermissionService.check(['Purchase.Order.Read']);
  canTreatmentPaymentFastLink = this.checkPermissionService.check(['Basic.SaleOrder.Read']);
  canCashBankReport = this.checkPermissionService.check(['Report.CashBankAccount']);
  canLaboOrderReport = this.checkPermissionService.check(['Report.LaboOrder']);
  canPartnerCustomerReport = this.checkPermissionService.check(['Report.PartnerOldNew']);
  canSaleReport = this.checkPermissionService.check(['Report.Sale']);
  canAppointment = this.checkPermissionService.check(['Report.Appointment']);
  public today: Date = new Date(new Date().toDateString());
  totalCash : number = 0;
  stateFilter: string = '';
  stateCount: any = {};
  states: any[] = [
    { value: '', text: 'Tất cả'},
    { value: 'waiting', text: 'Chờ khám'},
    { value: 'examination', text: 'Đang khám'},
    { value: 'done', text: 'Hoàn thành'},
  ]


  constructor(private checkPermissionService: CheckPermissionService , private appointmentService: AppointmentService,
    private intlService: IntlService,
    private authService: AuthService,
    private cashBookService: CashBookService) { 
     
    }

  ngOnInit() {
    this.loadTotalCash();
   }

  loadTotalCash() {
    var val = new CashBookSummarySearch();
    val.companyId = this.authService.userInfo.companyId;
    // val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    // val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
    val.resultSelection = 'cash';
    this.cashBookService.getTotal(val).subscribe(rs =>{
      
      this.totalCash = rs;
    }, () => {

    });

  }

}
