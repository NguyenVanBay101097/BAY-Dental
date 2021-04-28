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
  showAppointmentLink = this.checkPermissionService.check('Basic.Appointment.Read');
  showCustomerLink = this.checkPermissionService.check('Basic.Partner.Read');
  showPurchaseOrderLink = this.checkPermissionService.check('Purchase.Order.Read');
  showTreatmentPaymentFastLink = this.checkPermissionService.check('Basic.SaleOrder.Read');
  showCashBankReport = this.checkPermissionService.check('Report.CashBankAccount');
  showLaboOrderReport = this.checkPermissionService.check('Report.LaboOrder');
  showPartnerCustomerReport = this.checkPermissionService.check('Report.PartnerOldNew');
  showSaleReport = this.checkPermissionService.check('Report.Sale');
  showAppointment = this.checkPermissionService.check('Report.Appointment');

  constructor(private checkPermissionService: CheckPermissionService) { }

  ngOnInit() { }
}
