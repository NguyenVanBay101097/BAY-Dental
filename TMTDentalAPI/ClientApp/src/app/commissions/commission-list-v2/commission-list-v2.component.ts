import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { CommissionProductRuleService } from '../commission-product-rule.service';
import { CommissionPaged, CommissionService } from '../commission.service';

@Component({
  selector: 'app-commission-list-v2',
  templateUrl: './commission-list-v2.component.html',
  styleUrls: ['./commission-list-v2.component.css'],
  host: {'class': 'h-100'}
})
export class CommissionListV2Component implements OnInit {
  
  searchCommission: string;
  sourceCommissions: any[];
  commissions: any[];
  commission: any;

  commissionProductRules: any[];

  constructor(private commissionService: CommissionService, 
    private commissionProductRuleService: CommissionProductRuleService) { }

  ngOnInit() {
    this.loadDataFromApi();
  }

  onSearchChange(val: string) {
    this.sourceCommissions = this.commissions.filter(x => x.name.toLowerCase().includes(val.toLowerCase()));
  }

  loadDataFromApi() {
    var val = new CommissionPaged();
    // val.limit = 0;
    val.offset = 0;
    val.search = '';
    this.commissionService.getPaged(val).subscribe(res => {
      this.sourceCommissions = res.items;
      console.log(res);
      if (!this.searchCommission) {
        this.commissions = this.sourceCommissions;
      }
    }, err => {
      console.log(err);
    })
  }

  onSelectCommission(item: any) {
    if (this.commission === item) {
      this.commission = null;
    } else {
      this.commission = item;

      this.commissionProductRuleService.getForCommission(item.id).subscribe(res => {
        console.log(res);
        // this.commissionProductRules = res.commissionProductRules;

      }, err => {
        console.log(err);
      })
    }
  }

  deleteCommission(item: any, i) {
    
  }




}
