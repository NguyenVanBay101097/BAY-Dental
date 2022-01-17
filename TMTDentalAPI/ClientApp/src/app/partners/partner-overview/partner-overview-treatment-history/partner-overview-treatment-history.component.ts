import { KeyValue } from '@angular/common';
import { Component, Inject, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { PageGridConfig, PAGER_GRID_CONFIG } from 'src/app/shared/pager-grid-kendo.config';
import { PrintService } from 'src/app/shared/services/print.service';
import { PartnerService, SaleOrderLinePaged } from '../../partner.service';

@Component({
  selector: 'app-partner-overview-treatment-history',
  templateUrl: './partner-overview-treatment-history.component.html',
  styleUrls: ['./partner-overview-treatment-history.component.css']
})
export class PartnerOverviewTreatmentHistoryComponent implements OnInit {
  @Input() partnerId: any;
  listTime: any[] = [];
  listTreatments: any;
  today: any;
  
  toothType = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
    { name: "Chọn răng", value: "manual" },
  ];
  stateDisplay= {
    sale:"Đang điều trị",
    done: "Hoàn thành"
  }
  
  gridData: GridDataResult;
  loading = false;
  filter = new SaleOrderLinePaged();
  pagerSettings: any;
  constructor(
    private authService: AuthService,
    private saleOrderLineService: SaleOrderLineService,
    private intl: IntlService,
    private router: Router,
    private intlService: IntlService,
    private saleOrderService: SaleOrderService,
    private printService: PrintService,
    private partnerService: PartnerService,
    @Inject(PAGER_GRID_CONFIG) config: PageGridConfig
    ) { this.pagerSettings = config.pagerSettings }
  ngOnInit() {
    this.initFilter();
    this.loadDataFromApi();
    this.today = this.intl.formatDate(new Date(), "dd/MM/yyyy");
  }

  getDataApiParam() {
    var val = new SaleOrderLinePaged();
    val.limit = this.filter.limit;
    val.offset = this.filter.offset;
    val.partnerId = this.partnerId;
    val.companyId = this.authService.userInfo.companyId;
    val.state = 'sale,done,cancel';
    return val;
  }

  initFilter() {
    this.filter.limit = 20;
    this.filter.offset = 0;
  } 
  
  loadDataFromApi() {
    var val = this.getDataApiParam();
    this.loading = true;
    this.saleOrderLineService.getPaged(val).subscribe(res => {
      this.gridData = <GridDataResult>{
        data: res.items,
        total: res.totalItems
      }
      this.loading = false;
    },
      err => {
        this.loading = false;
      });
  }

  createNewSaleOrder() {
    this.saleOrderService.defaultGet({partnerId: this.partnerId}).subscribe(result => {
      var dateOrder = new Date(result.dateOrder);
      var val = {
        partnerId: this.partnerId,
        companyId: result.companyId,
        dateOrder: this.intl.formatDate(dateOrder, 'yyyy-MM-ddTHH:mm:ss')
      };

      this.saleOrderService.create(val).subscribe(result2 => {
        this.router.navigate(['/sale-orders', result2.id]);
      });
    });
  }

  viewTeeth(treatment: any) {
    let teethList = '';
    if(treatment.toothType && treatment.toothType != 'manual'){
      teethList = this.toothType.find(x => x.value == treatment.toothType).name;
    }
    else{
      teethList = treatment.teeth.map(x => x.name).join(', ');
    }
    return teethList;
  }

  formatDate(date) {
    return this.intl.formatDate(new Date(date), "dd/MM/yyyy");
  }

  reverseKeyTreatment = (a: KeyValue<number, string>, b: KeyValue<number, string>): number => {
    return a.key > b.key ? -1 : (b.key > a.key ? 1 : 0);
  }

  getStateDisplay(state = null) {
    switch (state) {
      case 'sale':
        return 'Đang điều trị';
      case 'done':
        return 'Hoàn thành';
      case 'cancel':
        return 'Ngừng điều trị';
      default:
        return 'Nháp';
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.filter.limit = event.take;
    this.filter.offset = event.skip;
    this.loadDataFromApi();
  }

  printTreatmentHistories() {
    let val;
    this.partnerService.printTreatmentHistories(val).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    })
  }
}
