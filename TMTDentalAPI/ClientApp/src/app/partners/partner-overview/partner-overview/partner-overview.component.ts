import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { AccountCommonPartnerReport, AccountCommonPartnerReportService } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AppointmentDisplay } from 'src/app/appointment/appointment';
import { SaleOrderLineService, SaleOrderLinesPaged } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderBasic } from 'src/app/sale-orders/sale-order-basic';
import { SaleOrderLineDisplay } from 'src/app/sale-orders/sale-order-line-display';
import { PartnersService } from 'src/app/shared/services/partners.service';
import { PartnerDisplay } from '../../partner-simple';
import { PartnerService } from '../../partner.service';

@Component({
  selector: 'app-partner-overview',
  templateUrl: './partner-overview.component.html',
  styleUrls: ['./partner-overview.component.css']
})
export class PartnerOverviewComponent implements OnInit {
  partnerId: string;
  partner: PartnerDisplay;
  customerAppointment: AppointmentDisplay;
  saleQuotations: SaleOrderLineDisplay;

  limit = 20;
  listSaleOrder: SaleOrderBasic[] = [];
  accountCommonPartnerReport: AccountCommonPartnerReport = new AccountCommonPartnerReport();
  skip = 0;
  search: string;

  constructor(
    private activeRoute: ActivatedRoute,
    private PartnerOdataService: PartnersService,
    private partnerService: PartnerService,
    private accountCommonPartnerReportService: AccountCommonPartnerReportService,
    private saleOrderLineService: SaleOrderLineService,
    private saleOrderService: SaleOrderService,
  ) { }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.GetPartner();
    this.loadCustomerAppointment();
    this.getSaleQoutation();
  }

  GetPartner() {
    this.PartnerOdataService.getDisplay(this.partnerId).subscribe((res: any) => {
      this.partner = res;
    });
  }

  loadCustomerAppointment() {
    this.partnerService.getNextAppointment(this.partnerId).subscribe(
      rs => {
        this.customerAppointment = rs;
      });
  }

  getSaleQoutation() {
    const val = new SaleOrderLinesPaged();
    val.Offset = 0;
    val.limit = 0;
    val.isQuotation = true;
    this.saleOrderLineService.get(val).subscribe((res: any) => {
      this.saleQuotations = res.items;
    });
  }

  loadSaleOrder() {
    var val = new SaleOrderPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.partnerId = this.partnerId;

    this.saleOrderService.getPaged(val).subscribe(res => {
      this.listSaleOrder = res.items;
    }, err => {
      console.log(err);
    })
  }

  loadReport() {
    this.accountCommonPartnerReportService.getSummaryByPartner(this.partnerId).subscribe(
      result => {
        this.accountCommonPartnerReport = result
      }
    )
    this.GetPartner();
    this.loadCustomerAppointment();
  }



}
