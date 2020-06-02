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
  listFlagCondition: any = [];
  listData: any;
  listTags: any = [];
  selectedTag: any;
  numbericValueCondition: number = 0;
  TCareRule: TCareRule;

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
      typeCondition: "",
      flagCondition: "",
      valueCondition: 0,
      nameCondition: "",
      selectedValueCondition: null,
    });
  }

  onSave() {
    this.TCareRule = {
      logic: this.formGroup.get('logic').value,
      conditions: this.listTags
    }
    console.log(this.TCareRule);
    this.activeModal.close(this.TCareRule);
  }

  unitCondition() {
    switch (this.formGroup.get('typeCondition').value) {
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

  optionFlagCondition() {
    switch (this.formGroup.get('typeCondition').value) {
      case "birthday": 
        this.listFlagCondition = ["before"];
        return;
      case "lastTreatmentDay":
        this.listFlagCondition = ["after"];
        return;
      case "partnerGroup":
      case "service":
      case "serviceGroup":
        this.listFlagCondition = ["contain", "does_not_contain"];
        return;
      default:
        return;
    }
  }

  placeholderValueCondition() {
    switch (this.formGroup.get('typeCondition').value) {
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

  convertTypeCondition(typeCondition) {
    switch (typeCondition) {
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

  convertFlagCondition(flagCondition) {
    switch (flagCondition) {
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

  handleTypeConditionChange(value) {
    if (value !== "") {
      this.isDisabledCondition = false;
    } else {
      this.isDisabledCondition = true;
    }
    this.optionFlagCondition();
    if (this.listFlagCondition.length > 0) {
      this.formGroup.patchValue({ flagCondition: this.listFlagCondition[0] });
    }
    switch (this.formGroup.get('typeCondition').value) {
      case "birthday": 
      case "lastTreatmentDay":
        this.formGroup.patchValue({ valueCondition: 0 });
        return;
      default:
        this.search = null;
        this.formGroup.patchValue({ 
          valueCondition: 0,
          nameCondition: "",
          selectedValueCondition: null 
        });
        this.loadListData();
        return;
    }
  }

  getTypeConditionValue() {
    return this.formGroup.get('typeCondition').value;
  }

  isDay(typeCondition) {
    switch (typeCondition) {
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
    switch (this.formGroup.get('typeCondition').value) {
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
    if (this.isDay(this.formGroup.get('typeCondition').value)) {
      this.listTags.push({
        typeCondition: this.formGroup.get('typeCondition').value,
        flagCondition: this.formGroup.get('flagCondition').value,
        valueCondition: this.formGroup.get('valueCondition').value,
        nameCondition: this.formGroup.get('nameCondition').value,
      });
      this.formGroup.patchValue({ 
        typeCondition: "",
        flagCondition: "",
        valueCondition: 0,
        nameCondition: "",
        selectedValueCondition: null
      });
    } else {
      if (this.formGroup.value.valueCondition !== 0) {
        this.listTags.push({
          typeCondition: this.formGroup.get('typeCondition').value,
          flagCondition: this.formGroup.get('flagCondition').value,
          valueCondition: this.formGroup.get('valueCondition').value,
          nameCondition: this.formGroup.get('nameCondition').value,
        });
        this.formGroup.patchValue({ 
          typeCondition: "",
          flagCondition: "",
          valueCondition: "",
          nameCondition: "",
          selectedValueCondition: null
        });
      } else {
        this.notificationService.show({
          content: 'Vui lòng' + this.placeholderValueCondition(),
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
      typeCondition: "",
      flagCondition: "",
      valueCondition: 0,
      nameCondition: "",
      selectedValueCondition: null
    });
    this.selectedTag = null;
  }

  onSaveTag() {
    this.listTags[this.selectedTag] = {
      typeCondition: this.formGroup.get('typeCondition').value,
      flagCondition: this.formGroup.get('flagCondition').value,
      valueCondition: this.formGroup.get('valueCondition').value,
      nameCondition: this.formGroup.get('nameCondition').value,
    };
    this.formGroup.patchValue({ 
      typeCondition: "",
      flagCondition: "",
      valueCondition: 0,
      nameCondition: "",
      selectedValueCondition: null
    });
    this.selectedTag = null;
  }

  handleNumbericValueConditionChange(value) {
    // console.log(value);
    this.formGroup.patchValue({ 
      valueCondition: value, 
      nameCondition: ""
    });
  }

  handleValueConditionChange(value) {
    // console.log(value);
    this.formGroup.patchValue({ 
      valueCondition: value.id, 
      nameCondition: value.name
    });
  }

  handleValueConditionFilter(value) {
    // console.log(value);
    this.search = value;
    this.loadListData();
  }

  clickTag(item, index) {
    // this.listTags.splice(index, 1);
    this.selectedTag = index;
    // console.log(this.selectedTag);
    this.formGroup.patchValue({ 
      typeCondition: item.typeCondition,
      flagCondition: item.flagCondition,
      valueCondition: item.valueCondition,
      nameCondition: item.nameCondition,
      selectedValueCondition: {
        completeName: item.nameCondition,
        id: item.valueCondition,
        name: item.nameCondition
      }
    });
    this.optionFlagCondition();
    if (this.isDay(item.typeCondition)) {
      this.numbericValueCondition = item.valueCondition;
    } else {
      this.search = item.nameCondition;
      this.loadListData();
    }
    // console.log(this.formGroup.value);
  }

  deleteTag(index) {
    this.listTags.splice(index, 1);
  }
}
