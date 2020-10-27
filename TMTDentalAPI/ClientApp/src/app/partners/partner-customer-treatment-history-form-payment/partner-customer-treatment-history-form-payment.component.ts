import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { PaymentInfoContent } from 'src/app/account-invoices/account-invoice.service';
import { AccountPaymentBasic } from 'src/app/account-payments/account-payment.service';
import { DiscountDefault } from 'src/app/core/services/sale-order.service';
import { LaboOrderBasic } from 'src/app/labo-orders/labo-order.service';
import { ProductPriceListBasic } from 'src/app/price-list/price-list';
import { SaleOrderDisplay } from 'src/app/sale-orders/sale-order-display';
import { AccountPaymentPrintComponent } from 'src/app/shared/account-payment-print/account-payment-print.component';
import { UserSimple } from 'src/app/users/user-simple';
import { PartnerSimple } from '../partner-simple';

@Component({
  selector: 'app-partner-customer-treatment-history-form-payment',
  templateUrl: './partner-customer-treatment-history-form-payment.component.html',
  styleUrls: ['./partner-customer-treatment-history-form-payment.component.css']
})
export class PartnerCustomerTreatmentHistoryFormPaymentComponent implements OnInit {
  @Input() id: string;
  @Input() partnerId: string;

  formGroup: FormGroup;
  filteredPartners: PartnerSimple[];
  filteredUsers: UserSimple[];
  filteredPricelists: ProductPriceListBasic[];
  discountDefault: DiscountDefault;

  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('pricelistCbx', { static: true }) pricelistCbx: ComboBoxComponent;
  @ViewChild(AccountPaymentPrintComponent, { static: true }) accountPaymentPrintComponent: AccountPaymentPrintComponent;

  saleOrder: SaleOrderDisplay = new SaleOrderDisplay();
  saleOrderPrint: any;
  laboOrders: LaboOrderBasic[] = [];

  payments: AccountPaymentBasic[] = [];
  paymentsInfo: PaymentInfoContent[] = [];

  searchCardBarcode: string;
  partnerSend: any;
  type: string;
  constructor() { }

  ngOnInit() {
  }

}
