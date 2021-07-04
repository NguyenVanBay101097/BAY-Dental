import { Component, OnInit } from '@angular/core';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { IntlService } from '@progress/kendo-angular-intl';
import { forkJoin, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AppointmentGetCountVM } from 'src/app/appointment/appointment';
import { AppointmentService } from 'src/app/appointment/appointment.service';
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

  stateFilter: string = '';
  stateCount: any = {};
  states: any[] = [
    { value: '', text: 'Tất cả'},
    { value: 'waiting', text: 'Chờ khám'},
    { value: 'examination', text: 'Đang khám'},
    { value: 'done', text: 'Hoàn thành'},
  ]

  appointmentstates: any[] = [
    { value: '', text: 'Tất cả'},
    { value: 'examination', text: 'Đang hẹn'},
    { value: 'arrived', text: 'Đã đến'},
    { value: 'cancel', text: 'Hủy hẹn'},
  ]

  public pieData: any[] = [
    { category: "Tiền mặt", value:  300000, color:"#0066cc"},
    { category: "Ngân hàng", value: 200000, color:"#99ccff" },
    { category: "Khác", value: 100000, color: "#b3b3b3" },
   
  ];

  constructor(private checkPermissionService: CheckPermissionService , private appointmentService: AppointmentService,
    private intlService: IntlService) { 
      this.labelContent = this.labelContent.bind(this);
    }

  ngOnInit() { }

  loadStateCount() {
    forkJoin(this.states.map(x => {
      var val = new AppointmentGetCountVM();
      val.state = x.value;
      val.dateFrom = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
      val.dateTo = this.intlService.formatDate(this.today, 'yyyy-MM-dd');
      return this.appointmentService.getCount(val).pipe(
        switchMap(count => of({state: x.value, count: count}))
      );
    })).subscribe((result) => {
      result.forEach(item => {
        this.stateCount[item.state] = item.count;
      });
    });
  }

  setStateFilter(state: any) {
    this.stateFilter = state;  
  }

  public labelContent(args: LegendLabelsContentArgs): string {
    return `${args.dataItem.category} years old: ${this.intlService.formatNumber(
      args.dataItem.value,
      "p2"
    )}`;
  }

}
