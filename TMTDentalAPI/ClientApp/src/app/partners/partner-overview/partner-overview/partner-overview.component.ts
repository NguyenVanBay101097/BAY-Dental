import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query';
import { AccountCommonPartnerReport, AccountCommonPartnerReportSearchV2, AccountCommonPartnerReportService } from 'src/app/account-common-partner-reports/account-common-partner-report.service';
import { AppointmentDisplay } from 'src/app/appointment/appointment';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderLineService, SaleOrderLinesPaged } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderPaged, SaleOrderService } from 'src/app/core/services/sale-order.service';
import { DotKhamService } from 'src/app/dot-khams/dot-kham.service';
import { DotKhamBasic, DotKhamPaged } from 'src/app/dot-khams/dot-khams';
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
  dotkhams: DotKhamBasic[] = [];

  //for report
  debit: number = 0;
  credit: number = 0;
  saleCount: number = 0;

  constructor(
    private authService: AuthService,
    private activeRoute: ActivatedRoute,
    private PartnerOdataService: PartnersService,
    private partnerService: PartnerService,
    private accountCommonPartnerReportService: AccountCommonPartnerReportService,
    private saleOrderLineService: SaleOrderLineService,
    private saleOrderService: SaleOrderService,
    private saleCouponProgramService: SaleCouponProgramService,
    private router: Router,
    private dotkhamService: DotKhamService
  ) { }

  ngOnInit() {
    this.activeRoute.parent.params.subscribe(
      params => {
        this.partnerId = params.id;
        this.GetPartner();
        this.loadCustomerAppointment();
        this.getDotkhams();
        this.loadReport();  
      }
    )
  }

  GetPartner() {
    // this.PartnerOdataService.getDisplay(this.partnerId).subscribe((res: any) => {
    //   this.partner = res;
    // });

    this.partnerService.getCustomerInfo(this.partnerId).subscribe((result: any) => {
      this.partner = result;
    });
  }

  onPartnerUpdate() {
    this.partnerService.getCustomerInfo(this.partnerId).subscribe((result: any) => {
      this.partner = result;
    });
  }

  loadCustomerAppointment() {
    this.partnerService.getNextAppointment(this.partnerId).subscribe(
      rs => {
        this.customerAppointment = rs;
      });
  }

  getSaleOrders() {
    var val = new SaleOrderPaged();
    val.limit = 10000;
    val.companyId = this.authService.userInfo.companyId;
    val.partnerId = this.partnerId;
    this.saleOrderService.getPaged(val).subscribe(
      result => {
        this.saleOrders = result.items;
        this.saleCount = result.totalItems;
      }
    )
  }

  getDotkhams() {
    var val = new DotKhamPaged();
    val.limit = 0;
    val.partnerId = this.partnerId;
    this.dotkhamService.getPaged(val).subscribe(
      res=> {
        this.dotkhams = res.items;
      }
    );
  }

  createNewSaleOrder() {
    this.router.navigate(['sale-orders/form'], { queryParams: { partner_id: this.partnerId } });
  }

  onDeleteSaleOrder() {
    this.getSaleOrders();
  }

  getSaleQoutation() {
    const val = new SaleOrderLinesPaged();
    val.offset = 0;
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
        this.debit = result.debit;
        this.credit = result.credit;
      }
    )
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
