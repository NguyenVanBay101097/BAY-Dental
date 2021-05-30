import { Component, OnInit } from '@angular/core';
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

  constructor(private checkPermissionService: CheckPermissionService) { }

  ngOnInit() { }
}
