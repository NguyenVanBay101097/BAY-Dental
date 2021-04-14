import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { CommissionProductRuleDialogComponent } from '../commission-product-rule-dialog/commission-product-rule-dialog.component';
import { CommissionProductRule, CommissionProductRuleService } from '../commission-product-rule.service';

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
    var groups = new Set(commissionProductRules.map(item => item.categId)), results = [];
    groups.forEach(g => 
      results.push({
        categId: g,
        categName: commissionProductRules.find(i => i.categId === g).categ.name,
        values: commissionProductRules.filter(i => i.categId === g)
      }
    ))
    return results;
  }

  onSave() {
    this.commissionProductRules_group = this.group(this.commissionProductRules);
    var vals = [];
    this.commissionProductRules_group.forEach(el => {
      vals = vals.concat(el.values);
    });
    this.commissionProductRuleService.save(vals).subscribe(res => {
      this.notificationService.show({
        content: 'Lưu thành công',
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
    var temp = this.commissionProductRules.filter(x => 
      this.removeVietnamese(x.product.name).includes(this.removeVietnamese(val)) ||
      this.removeVietnamese(x.product.defaultCode).includes(this.removeVietnamese(val))
    );
    this.commissionProductRules_group = this.group(temp);
  }

  removeVietnamese(text) {
    text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    text = text.replace(/đ/g, "d");
    text = text.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    text = text.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    text = text.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    text = text.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    text = text.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    text = text.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    text = text.replace(/Đ/g, "D");
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
    modalRef.componentInstance.title = 'Áp dụng hoa hồng cho nhóm dịch vụ';
    modalRef.result.then(res => {
      commissionProductRule.values.forEach(el => {
        el.percentAdvisory = res.percentAdvisory;
        el.percentAssistant = res.percentAssistant;
        el.percentDoctor = res.percentDoctor;
      });
    }, () => {
    });
  }
}
