import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AudienceFilterItem } from '../../tcare.service';
import { AudienceFilterBirthdayComponent } from './audience-filter-dropdown/audience-filter-birthday/audience-filter-birthday.component';
import { AudienceFilterLastTreatmentDayComponent } from './audience-filter-dropdown/audience-filter-last-treatment-day/audience-filter-last-treatment-day.component';
import { AudienceFilterServiceComponent } from './audience-filter-dropdown/audience-filter-service/audience-filter-service.component';
import { AudienceFilterPartnerCategoryComponent } from './audience-filter-dropdown/audience-filter-partner-category/audience-filter-partner-category.component';
import { AudienceFilterServiceCategoryComponent } from './audience-filter-dropdown/audience-filter-service-category/audience-filter-service-category.component';
import { FormGroup, FormBuilder } from '@angular/forms';
declare var $ :any;

@Component({
  selector: 'app-audience-filter',
  templateUrl: './audience-filter.component.html',
  styleUrls: ['./audience-filter.component.css']
})
export class AudienceFilterComponent implements OnInit {
  @Input() audience_filter_receive: any;
  @Output() audience_filter_send = new EventEmitter<any>();
  formGroup: FormGroup;
  
  audience_filter_comp_data: { component, data };
  listAudienceFilter_Items: any[] = [];
  selectedAudienceFilter_Item: any;
  openAudienceFilter_Picker: boolean = false;
  openAudienceFilter: boolean = false;
  clickedAudienceFilter_Item: boolean = false;
  clickedAudienceFilter: boolean = false;
  count_clickOutside: number = 1;
  AudienceFilter_Item: AudienceFilterItem;
  listAudienceFilter_Picker = [
    {
      type: 'birthday',
      name: 'Sinh nhật'
    }, {
      type: 'lastTreatmentDay',
      name: 'Ngày điều trị cuối'
    }, {
      type: 'partnerGroup',
      name: 'Nhóm khách hàng'
    }, {
      type: 'service',
      name: 'Dịch vụ sử dụng'
    }, {
      type: 'serviceGroup',
      name: 'Nhóm dịch vụ sử dụng'
    }
  ]

  constructor(private fb: FormBuilder, ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      logic: "and",
      conditions: []
    });

    if (this.audience_filter_receive) {
      var temp = this.convertAudienceFilterItemsToArray(this.audience_filter_receive);
      this.formGroup.patchValue({
        logic: temp.logic,
        conditions: temp.conditions
      });
      this.listAudienceFilter_Items = temp.conditions;
    }
    // console.log(this.listAudienceFilter_Items);

    $(document).on('click', '.allow-focus', function (e) {
      e.stopPropagation();
    });
  }

  convertLogic(logic) {
    switch (logic) {
      case "and":
        return "tất cả điều kiện";
      case "or":
        return "bất kỳ điều kiện nào";
    }
  }

  convertFormulaType(item) {
    switch (item) {
      case 'eq':
        return 'bằng';
      case 'neq':
        return 'không bằng';
      case 'contains':
        return 'có chứa';
      case 'doesnotcontain':
        return 'không chứa';
      case 'startswith':
        return 'bắt đầu với';
      case 'before':
        return 'trước';
      case 'after':
        return 'sau';
      case 'bằng':
        return 'eq';
      case 'không bằng':
        return 'neq';
      case 'có chứa':
        return 'contains';
      case 'không chứa':
        return 'doesnotcontain';
      case 'bắt đầu với':
        return 'startswith';
      case 'trước':
        return 'before';
      case 'sau':
        return 'after';
    }
  }

  isDay(item) {
    switch (item) {
      case 'birthday': 
      case 'lastTreatmentDay': 
        return true;
      default: 
        return false;
    }
  }

  setLogic(value) {
    this.formGroup.patchValue({ 
      logic: value 
    });
  }

  convertAudienceFilterItemsToString() {
    this.formGroup.patchValue({ 
      conditions: this.listAudienceFilter_Items 
    });
    return JSON.stringify(this.formGroup.value);
  }

  convertAudienceFilterItemsToArray(listAudienceFilter_Items_String) {
    return JSON.parse(listAudienceFilter_Items_String);
  }

  existListAudienceFilter_Items() {
    if (!this.listAudienceFilter_Items || this.listAudienceFilter_Items.length == 0) {
      return false;
    }
    return true;
  }

  clickAudienceFilter_Item(item) {
    if (this.AudienceFilter_Item !== item) {
      this.AudienceFilter_Item = item;
      switch (item.type) {
        case "birthday":
          this.audience_filter_comp_data = {
            component: AudienceFilterBirthdayComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "lastTreatmentDay":
          this.audience_filter_comp_data = {
            component: AudienceFilterLastTreatmentDayComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "partnerGroup":
          this.audience_filter_comp_data = {
            component: AudienceFilterPartnerCategoryComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "service":
          this.audience_filter_comp_data = {
            component: AudienceFilterServiceComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "serviceGroup":
          this.audience_filter_comp_data = {
            component: AudienceFilterServiceCategoryComponent,
            data: this.AudienceFilter_Item
          }
        break;
      } 
    }
    this.openAudienceFilter = false;
    this.clickedAudienceFilter = false;
    this.openAudienceFilter_Picker = false; //
    if (this.clickedAudienceFilter_Item === true) {
      this.selectedAudienceFilter_Item = null;
      this.clickedAudienceFilter_Item = false;
    } else {
      this.selectedAudienceFilter_Item = item;
      this.clickedAudienceFilter_Item = true;
    }
    this.count_clickOutside = 1;
  }

  clickOutsideAudienceFilter_Item_Picker() {
    this.count_clickOutside += 1;
    if (this.clickedAudienceFilter_Item === true && this.count_clickOutside >= 3) {
      this.selectedAudienceFilter_Item = null;
      this.clickedAudienceFilter_Item = false;
      this.audience_filter_send.emit(this.convertAudienceFilterItemsToString());
    }
  }

  closeAudienceFilter_Item_Picker(status) {
    if (status == false) {
      this.selectedAudienceFilter_Item = null;
      this.clickedAudienceFilter_Item = false;
    }
  }

  deleteAudienceFilterItem(index) {
    this.selectedAudienceFilter_Item = null;
    this.clickedAudienceFilter_Item = false;
    this.listAudienceFilter_Items.splice(index, 1);
    this.audience_filter_send.emit(this.convertAudienceFilterItemsToString());
  }

  clickAudienceFilter() {
    this.selectedAudienceFilter_Item = null;
    this.clickedAudienceFilter_Item = false;
    if (this.clickedAudienceFilter === true) {
      this.openAudienceFilter = false;
      this.clickedAudienceFilter = false;
      this.openAudienceFilter_Picker = false; //
    } else {
      this.openAudienceFilter = true;
      this.clickedAudienceFilter = true;
      this.openAudienceFilter_Picker = false; //
    }
    this.count_clickOutside = 1;
  }

  clickOutsideAudienceFilter() {
    this.count_clickOutside += 1;
    if (this.clickedAudienceFilter === true && this.count_clickOutside >= 3) {
      this.openAudienceFilter = false;
      this.clickedAudienceFilter = false;
      this.openAudienceFilter_Picker = false; //
    }
  }

  selectAudienceFilter(item) {
    this.AudienceFilter_Item = {
      type: item.type,
      name: item.name,
      formula_type: null,
      formula_value: null,
      formula_display: null
    }
    switch (item.type) {
      case "birthday":
        this.audience_filter_comp_data = {
          component: AudienceFilterBirthdayComponent,
          data: this.AudienceFilter_Item
        }
      break;
      case "lastTreatmentDay":
        this.audience_filter_comp_data = {
          component: AudienceFilterLastTreatmentDayComponent,
          data: this.AudienceFilter_Item
        }
      break;
      case "partnerGroup":
        this.audience_filter_comp_data = {
          component: AudienceFilterPartnerCategoryComponent,
          data: this.AudienceFilter_Item
        }
      break;
      case "service":
        this.audience_filter_comp_data = {
          component: AudienceFilterServiceComponent,
          data: this.AudienceFilter_Item
        }
      break;
      case "serviceGroup":
        this.audience_filter_comp_data = {
          component: AudienceFilterServiceCategoryComponent,
          data: this.AudienceFilter_Item
        }
      break;
    }
    document.getElementById("dropdown-item-audience-filter").style.display = 'none';
    this.openAudienceFilter_Picker = true;
  }

  addAudienceFilterItem() {
    if (this.AudienceFilter_Item.formula_type && this.AudienceFilter_Item.formula_value) {
      this.listAudienceFilter_Items.push(this.AudienceFilter_Item);
      this.audience_filter_send.emit(this.convertAudienceFilterItemsToString());
      this.openAudienceFilter = false;
      this.clickedAudienceFilter = false;
      this.openAudienceFilter_Picker = false; //
    }
  }
}
