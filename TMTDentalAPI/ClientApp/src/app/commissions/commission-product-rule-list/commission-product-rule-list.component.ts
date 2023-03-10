import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { CommissionProductRuleDialogComponent } from '../commission-product-rule-dialog/commission-product-rule-dialog.component';
import { CommissionProductRuleService } from '../commission-product-rule.service';

@Component({
  selector: 'app-commission-product-rule-list',
  templateUrl: './commission-product-rule-list.component.html',
  styleUrls: ['./commission-product-rule-list.component.css']
})
export class CommissionProductRuleListComponent implements OnChanges, OnInit {

  @Input() commissionId: string;
  commissionProductRules: any[] = [];
  commissionProductRules_group: any[] = [];
  search: string;

  constructor(private commissionProductRuleService: CommissionProductRuleService, 
    private notificationService: NotificationService, 
    private modalService: NgbModal) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['commissionId']) {
      if (changes['commissionId'].currentValue) {
        this.loadDataFromApi();
      } else {
        this.commissionProductRules = [];
        this.commissionProductRules_group = [];
      }
    }
  }

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    if (this.commissionId) {
      this.commissionProductRuleService.getForCommission(this.commissionId).subscribe(res => {
        this.commissionProductRules = res;
        this.commissionProductRules_group = this.group(this.commissionProductRules);
        if (this.search) {
          this.onSearchChange(this.search);
        }
      }, err => {
        console.log(err);
      })
    }
  }

  group(commissionProductRules) {
    var groups = new Set(commissionProductRules.map(x => x.categId)), results = [];
    groups.forEach(g => 
      results.push({
        categId: g,
        categName: commissionProductRules.find(x => x.categId === g).categ.name,
        values: commissionProductRules.filter(x => x.categId === g)
      }
    ))
    return results;
  }

  onSave() {
    this.commissionProductRuleService.save(this.commissionProductRules).subscribe(res => {
      this.notificationService.show({
        content: 'L??u th??nh c??ng',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadDataFromApi();
    }, err => {
      console.log(err);
    });
  }

  onSearchChange(val: string) {
    var items = this.commissionProductRules.filter(x => 
      this.removeVietnamese(x.product.name).includes(this.removeVietnamese(val)) ||
      this.removeVietnamese(x.product.defaultCode).includes(this.removeVietnamese(val))
    );
    this.commissionProductRules_group = this.group(items);
  }

  removeVietnamese(text) {
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???|??|???|???|???|???|???/g, "a");
    text = text.replace(/??|??|???|???|???|??|???|???|???|???|???/g, "e");
    text = text.replace(/??|??|???|???|??/g, "i");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???|??|???|???|???|???|???/g, "o");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???/g, "u");
    text = text.replace(/???|??|???|???|???/g, "y");
    text = text.replace(/??/g, "d");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???|??|???|???|???|???|???/g, "A");
    text = text.replace(/??|??|???|???|???|??|???|???|???|???|???/g, "E");
    text = text.replace(/??|??|???|???|??/g, "I");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???|??|???|???|???|???|???/g, "O");
    text = text.replace(/??|??|???|???|??|??|???|???|???|???|???/g, "U");
    text = text.replace(/???|??|???|???|???/g, "Y");
    text = text.replace(/??/g, "D");
    text = text.toLowerCase();
    text = text
      .replace(/[&]/g, "-and-")
      .replace(/[^a-zA-Z0-9._-]/g, "-")
      .replace(/[-]+/g, "-")
      .replace(/-$/, "");
    return text;
  }

  setCommissionCategRule(commissionProductRule) {
    let modalRef = this.modalService.open(CommissionProductRuleDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = '??p d???ng hoa h???ng cho nh??m d???ch v???';
    modalRef.result.then(res => {
      var items = this.commissionProductRules.filter(x => x.categId === commissionProductRule.categId);
      items.map(function(x) { 
        x.percent = res.percent;       
        return x;
      });
      commissionProductRule.values.forEach(el => {
        el.percent = res.percent;
      });
    }, () => {
    });
  }

  setCommissionAllProductRule() {
    let modalRef = this.modalService.open(CommissionProductRuleDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = '??p d???ng hoa h???ng cho t???t c??? d???ch v???';
    modalRef.result.then(res => {
      this.commissionProductRules.map(function(x) { 
        x.percent = res.percent;
        return x;
      });
      this.commissionProductRules_group = this.group(this.commissionProductRules);
    }, () => {
    });
  }

  setNumberic(item, percent) {
    var items = this.commissionProductRules.find(x => 
      x.id == item.id && 
      x.categId == item.categId && 
      x.productId == item.productId
    );
    items.percent = percent;
    // if (typePercent == "percentAdvisory") {
    //   items.percentAdvisory = percent;
    // } else if (typePercent == "percentDoctor") {
    //   items.percentDoctor = percent;
    // } else if (typePercent == "percentAssistant") {
    //   items.percentAssistant = percent;
    // }
  }
}
