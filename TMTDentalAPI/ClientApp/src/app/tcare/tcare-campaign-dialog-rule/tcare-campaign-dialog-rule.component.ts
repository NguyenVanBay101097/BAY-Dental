import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService, TCareRule } from '../tcare.service';
import { PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { map } from 'rxjs/operators';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-tcare-campaign-dialog-rule',
  templateUrl: './tcare-campaign-dialog-rule.component.html',
  styleUrls: ['./tcare-campaign-dialog-rule.component.css']
})
export class TcareCampaignDialogRuleComponent implements OnInit {

  title: string;
  formGroup: FormGroup;
  isDisabledCondition: boolean = true;
  limit: number = 20;
  skip: number = 0;
  search: string;
  listop: any = [];
  listData: any;
  listTags: any = [];
  selectedTag: any;
  numbericvalue: number = 0;
  tCareRule: TCareRule;

  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private notificationService: NotificationService,
    private partnerCategoryService: PartnerCategoryService,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      logic: "and",
      type: "",
      op: "",
      value: 0,
      name: "",
      selectedvalue: null,
    });
    this.loadDataCell(this.tCareRule);
  }

  loadDataCell(data: TCareRule) {
    this.formGroup.patchValue({ logic: data.logic });
    this.listTags = data.conditions;
  }

  onSave() {
    this.tCareRule = {
      logic: this.formGroup.get('logic').value,
      conditions: this.listTags
    }
    console.log(this.tCareRule);
    this.activeModal.close(this.tCareRule);
  }

  unitCondition() {
    switch (this.formGroup.get('type').value) {
      case "birthday":
      case "lastTreatmentDay":
        return "Ngày";
      case "partnerGroup":
        return "Nhóm khách hàng";
      case "service":
        return "Dịch vụ";
      case "serviceGroup":
        return "Nhóm dịch vụ";
      default:
        return "";
    }
  }

  optionop() {
    switch (this.formGroup.get('type').value) {
      case "birthday":
        this.listop = ["before"];
        return;
      case "lastTreatmentDay":
        this.listop = ["after"];
        return;
      case "partnerGroup":
      case "service":
      case "serviceGroup":
        this.listop = ["contain", "does_not_contain"];
        return;
      default:
        return;
    }
  }

  placeholdervalue() {
    switch (this.formGroup.get('type').value) {
      case "partnerGroup":
        return "Chọn nhóm khách hàng";
      case "service":
        return "Chọn dịch vụ";
      case "serviceGroup":
        return "Chọn nhóm dịch vụ";
      default:
        return "";
    }
  }

  converttype(type) {
    switch (type) {
      case "birthday":
        return "Sinh nhật"
      case "lastTreatmentDay":
        return "Ngày điều trị cuối";
      case "partnerGroup":
        return "Nhóm khách hàng";
      case "service":
        return "Dịch vụ sử dụng";
      case "serviceGroup":
        return "Nhóm dịch vụ sử dụng";
      default:
        return "";
    }
  }

  convertop(op) {
    switch (op) {
      case "before":
        return "trước"
      case "after":
        return "sau";
      case "contain":
        return "thuộc";
      case "does_not_contain":
        return "không thuộc";
      default:
        return "";
    }
  }

  handletypeChange(value) {
    if (value !== "") {
      this.isDisabledCondition = false;
    } else {
      this.isDisabledCondition = true;
    }
    this.optionop();
    if (this.listop.length > 0) {
      this.formGroup.patchValue({ op: this.listop[0] });
    }
    switch (this.formGroup.get('type').value) {
      case "birthday":
      case "lastTreatmentDay":
        this.formGroup.patchValue({ value: 0 });
        return;
      default:
        this.search = null;
        this.formGroup.patchValue({
          value: 0,
          name: "",
          selectedvalue: null
        });
        this.loadListData();
        return;
    }
  }

  gettypeValue() {
    return this.formGroup.get('type').value;
  }

  isDay(type) {
    switch (type) {
      case "birthday":
      case "lastTreatmentDay":
        return true;
      default:
        return false;
    }
  }

  loadPartnerCategoryList() {
    var val = new PartnerCategoryPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';

    this.partnerCategoryService.getPaged(val).subscribe(res => {
      // console.log("res: ", res);
      this.listData = res.items;
    }, err => {
      console.log(err);
    })
  }

  loadServiceList() {
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.categId = '';
    val.type2 = 'service';

    this.productService.getPaged(val).subscribe(res => {
      // console.log("res: ", res);
      this.listData = res.items;
    }, err => {
      console.log(err);
    })
  }

  loadServiceCategoryList() {
    var val = new ProductCategoryPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    val.type = 'service';

    this.productCategoryService.getPaged(val).subscribe(res => {
      console.log("res: ", res);
      this.listData = res.items;
    }, err => {
      console.log(err);
    })
  }

  loadListData() {
    switch (this.formGroup.get('type').value) {
      case "partnerGroup":
        this.loadPartnerCategoryList();
        return;
      case "service":
        this.loadServiceList();
        return;
      case "serviceGroup":
        this.loadServiceCategoryList();
        return;
      default:
        return;
    }
  }

  onAddTag() {
    // console.log(this.formGroup.value);
    if (this.isDay(this.formGroup.get('type').value)) {
      this.listTags.push({
        type: this.formGroup.get('type').value,
        op: this.formGroup.get('op').value,
        value: this.formGroup.get('value').value,
        name: this.formGroup.get('name').value,
      });
      this.formGroup.patchValue({
        type: "",
        op: "",
        value: 0,
        name: "",
        selectedvalue: null
      });
    } else {
      if (this.formGroup.value.value !== 0) {
        this.listTags.push({
          type: this.formGroup.get('type').value,
          op: this.formGroup.get('op').value,
          value: this.formGroup.get('value').value,
          name: this.formGroup.get('name').value,
        });
        this.formGroup.patchValue({
          type: "",
          op: "",
          value: "",
          name: "",
          selectedvalue: null
        });
      } else {
        this.notificationService.show({
          content: 'Vui lòng' + this.placeholdervalue(),
          hideAfter: 5000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'error', icon: true }
        });
      }
    }
  }

  onCancelTag() {
    this.formGroup.patchValue({
      type: "",
      op: "",
      value: 0,
      name: "",
      selectedvalue: null
    });
    this.selectedTag = null;
  }

  onSaveTag() {
    this.listTags[this.selectedTag] = {
      type: this.formGroup.get('type').value,
      op: this.formGroup.get('op').value,
      value: this.formGroup.get('value').value,
      name: this.formGroup.get('name').value,
    };
    this.formGroup.patchValue({
      type: "",
      op: "",
      value: 0,
      name: "",
      selectedvalue: null
    });
    this.selectedTag = null;
  }

  handleNumbericvalueChange(value) {
    // console.log(value);
    this.formGroup.patchValue({
      value: value,
      name: ""
    });
  }

  handlevalueChange(value) {
    // console.log(value);
    this.formGroup.patchValue({
      value: value.id,
      name: value.name
    });
  }

  handlevalueFilter(value) {
    // console.log(value);
    this.search = value;
    this.loadListData();
  }

  clickTag(item, index) {
    // this.listTags.splice(index, 1);
    this.selectedTag = index;
    // console.log(this.selectedTag);
    this.formGroup.patchValue({
      type: item.type,
      op: item.op,
      value: item.value,
      name: item.name,
      selectedvalue: {
        completeName: item.name,
        id: item.value,
        name: item.name
      }
    });
    this.optionop();
    if (this.isDay(item.type)) {
      this.numbericvalue = item.value;
    } else {
      this.search = item.name;
      this.loadListData();
    }
    // console.log(this.formGroup.value);
  }

  deleteTag(index) {
    this.listTags.splice(index, 1);
  }
}
