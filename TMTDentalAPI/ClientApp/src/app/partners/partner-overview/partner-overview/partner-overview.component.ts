import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { AccountCommonPartnerReport, AccountCommonPartnerReportSearchV2, AccountCommonPartnerReportService } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AppointmentDisplay } from 'src/app/appointment/appointment';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderLineService, SaleOrderLinesPaged } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { PromotionProgramBasic, PromotionProgramPaged, PromotionProgramService } from 'src/app/promotion-programs/promotion-program.service';
import { SaleCouponProgramBasic, SaleCouponProgramPaged, SaleCouponProgramService } from 'src/app/sale-coupon-promotion/sale-coupon-program.service';
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
  promotions: SaleCouponProgramBasic[] = [];
  saleOrders: SaleOrderBasic[] = [];
  limit = 20;
  listSaleOrder: SaleOrderBasic[] = [];
  accountCommonPartnerReport: AccountCommonPartnerReport = new AccountCommonPartnerReport();
  skip = 0;
  search: string;

  constructor(
    private authService: AuthService,
    private activeRoute: ActivatedRoute,
    private PartnerOdataService: PartnersService,
    private partnerService: PartnerService,
    private accountCommonPartnerReportService: AccountCommonPartnerReportService,
    private saleOrderLineService: SaleOrderLineService,
    private saleOrderService: SaleOrderService,
    private saleCouponProgramService: SaleCouponProgramService
  ) { }

  ngOnInit() {
    this.partnerId = this.activeRoute.parent.snapshot.paramMap.get('id');
    this.GetPartner();
    this.loadCustomerAppointment();
    // this.getSaleQoutation();
    // this.loadPromotion();
    this.getSaleOrders();
    this.loadReport();
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

  getSaleOrders() {
    var val = {
      id: this.partnerId,
      func: "GetSaleOrders",
      options: {
        params: new HttpParams().set('$count', 'true').set('$orderby', 'dateOrder desc')
      }
    }

    this.PartnerOdataService.getSaleOrderByPartner(val).subscribe(
      result => {
        this.saleOrders = result['value'];
        this.accountCommonPartnerReport.countSaleOrder = result['@odata.count'];
      }
    )
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

  loadReport() {
    var val = new AccountCommonPartnerReportSearchV2();
    val.partnerIds.push(this.partnerId);
    val.companyId = this.authService.userInfo.companyId;
    val.resultSelection = "customer";
    this.accountCommonPartnerReportService.getSummaryPartner(val).subscribe(
      result => {
        this.accountCommonPartnerReport = result
      }
    )
    this.GetPartner();
    this.loadCustomerAppointment();
  }

  loadPromotion() {
    const val = new SaleCouponProgramPaged();
    val.limit = 0;
    val.offset = 0;
    val.programType = 'promotion_program';
    this.saleCouponProgramService.getPaged(val).subscribe((res: any) => {
      this.promotions = res.items;
    });
  }

}
