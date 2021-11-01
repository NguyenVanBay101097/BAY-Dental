import { KeyValue } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { AuthService } from 'src/app/auth/auth.service';
import { SaleOrderLineHistoryReq, SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';

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
  constructor(
    private authService: AuthService,
    private saleOrderLineService: SaleOrderLineService,
    private intl: IntlService,
    private router: Router,
    private saleOrderService: SaleOrderService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();
    this.today = this.intl.formatDate(new Date(), "dd/MM/yyyy");
  }

  loadDataFromApi() {
    var val = new SaleOrderLineHistoryReq()
    val.partnerId = this.partnerId;
    val.companyId = this.authService.userInfo.companyId;
    this.saleOrderLineService.getHistories(val).subscribe((res: any) => {
      this.listTreatments = res;
    })
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
}
