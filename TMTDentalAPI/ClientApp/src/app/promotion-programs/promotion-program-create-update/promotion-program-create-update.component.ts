import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductPriceListBasic, ProductPricelistPaged } from 'src/app/price-list/price-list';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { PriceListService } from 'src/app/price-list/price-list.service';
import * as _ from 'lodash';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PromotionProgramDisplay, PromotionProgramService } from '../promotion-program.service';
import { PromotionProgramRuleCuDialogComponent } from '../promotion-program-rule-cu-dialog/promotion-program-rule-cu-dialog.component';
import { CompanySimple, CompanyBasic, CompanyPaged, CompanyService } from 'src/app/companies/company.service';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-promotion-program-create-update',
  templateUrl: './promotion-program-create-update.component.html',
  styleUrls: ['./promotion-program-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class PromotionProgramCreateUpdateComponent implements OnInit {
  formGroup: FormGroup;
  id: string;

  program: PromotionProgramDisplay = new PromotionProgramDisplay();
  listCompanies: CompanyBasic[] = [];

  constructor(private fb: FormBuilder, private programService: PromotionProgramService,
    private router: Router, private route: ActivatedRoute, private notificationService: NotificationService,
    private modalService: NgbModal, private companyService: CompanyService,
    private intlService: IntlService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      dateFromObj: null,
      dateToObj: null,
      maximumUseNumber: 0,
      companies: [[]],
      rules: this.fb.array([]),
    });

    this.loadListCompanies();

    this.route.queryParamMap.subscribe(params => {
      this.id = params.get("id");
      if (this.id) {
        this.loadRecord();
      } else {
        this.loadDefault();
      }
    });

    // this.id = this.route.snapshot.paramMap.get('id');
    // if (this.id) {
    //   this.loadRecord();
    // }

    // this.pricelistCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.pricelistCbx.loading = true)),
    //   switchMap(value => this.searchPricelists(value))
    // ).subscribe(result => {
    //   this.filteredPricelists = result.items;
    //   this.pricelistCbx.loading = false;
    // });

    // this.loadPricelists();
  }

  loadListCompanies() {
    var val = new CompanyPaged();
    this.companyService.getPaged(val).subscribe(result => {
      this.listCompanies = result.items;
    });
  }

  loadDefault() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      dateFromObj: null,
      dateToObj: null,
      maximumUseNumber: 0,
      companies: [[]],
      rules: this.fb.array([]),
    });
  }

  addRule() {
    let modalRef = this.modalService.open(PromotionProgramRuleCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm điều kiện và phần thưởng';

    modalRef.result.then(result => {
      this.rules.push(result);
    }, () => {
    });
  }

  editRule(rule: FormGroup) {
    let modalRef = this.modalService.open(PromotionProgramRuleCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Cập nhật điều kiện và phần thưởng';
    modalRef.componentInstance.formGroup = _.cloneDeep(rule);
    modalRef.result.then(result => {
      rule.patchValue(result.value);
    }, () => {
    });
  }

  deleteRule(index: number) {
    this.rules.removeAt(index);
  }

  getRuleApplyDisplay(rule: FormControl) {
    var val = rule.value;
    if (val.discountApplyOn == '2_product_category') {
      return 'Vài nhóm dịch vụ: ' + val.categories.map(x => x.completeName).join(', ');
    } else if (val.discountApplyOn == '0_product_variant') {
      return 'Vài dịch vụ: ' + val.products.map(x => x.name).join(', ');
    } else {
      return 'Tổng tiền';
    }
  }

  getRuleDiscountDisplay(rule: FormControl) {
    var val = rule.value;
    if (val.discountType == 'fixed_amount') {
      return `Giảm ${val.discountFixedAmount | 0}đ `;
    } else {
      return `Giảm ${val.discountPercentage || 0}%`;
    }
  }

  get rules() {
    return this.formGroup.get('rules') as FormArray;
  }

  viewCoupons() {
    this.router.navigate(['/coupons'], { queryParams: { program_id: this.id } });
  }

  get discountType() {
    return this.formGroup.get('discountType').value;
  }

  loadRecord() {
    this.programService.get(this.id).subscribe(result => {
      this.program = result;
      this.formGroup.patchValue(result);
      if (result.dateFrom) {
        let dateFrom = new Date(result.dateFrom);
        this.formGroup.get('dateFromObj').patchValue(dateFrom);
      }

      if (result.dateTo) {
        let dateTo = new Date(result.dateTo);
        this.formGroup.get('dateToObj').patchValue(dateTo);
      }

      let control = this.formGroup.get('rules') as FormArray;
      control.clear();
      result.rules.forEach(line => {
        var g = this.fb.group(line);
        g.setControl('categories', this.fb.control(line.categories));
        g.setControl('products', this.fb.control(line.products));
        control.push(g);
      });
    });
  }

  createNew() {
    this.router.navigate(['/promotion-programs/form'], { queryParams: {} });
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    console.log(value);
    value.companyIds = value.companies.map(x => x.id);
    value.dateFrom = value.dateFromObj ? this.intlService.formatDate(value.dateFromObj, 'd', 'en-US') : null;
    value.dateTo = value.dateToObj ? this.intlService.formatDate(value.dateToObj, 'd', 'en-US') : null;

    value.rules.forEach(rule => {
      rule.categoryIds = rule.categories.map(x => x.id);
      rule.productIds = rule.products.map(x => x.id);
    });
    if (this.id) {
      this.programService.update(this.id, value).subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadRecord();
      });
    } else {
      this.programService.create(value).subscribe(result => {
        this.router.navigate(['/promotion-programs/form'], { queryParams: { id: result.id } });
      });
    }
  }

}


