import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TCareRuleCondition } from '../../tcare.service';
import { AudienceFilterBirthdayComponent } from './audience-filter-dropdown/audience-filter-birthday/audience-filter-birthday.component';
import { AudienceFilterLastTreatmentDayComponent } from './audience-filter-dropdown/audience-filter-last-treatment-day/audience-filter-last-treatment-day.component';
import { AudienceFilterServiceComponent } from './audience-filter-dropdown/audience-filter-service/audience-filter-service.component';
import { AudienceFilterPartnerCategoryComponent } from './audience-filter-dropdown/audience-filter-partner-category/audience-filter-partner-category.component';
import { AudienceFilterServiceCategoryComponent } from './audience-filter-dropdown/audience-filter-service-category/audience-filter-service-category.component';
import { FormGroup, FormBuilder } from '@angular/forms';
declare var $: any;

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
  AudienceFilter_Item: TCareRuleCondition;
  listAudienceFilter_Picker = [
    {
      type: 'birthday',
      name: 'Sinh nhật'
    }, {
      type: 'lastSaleOrder',
      name: 'Ngày điều trị cuối'
    }, {
      type: 'categPartner',
      name: 'Nhóm khách hàng'
    }, {
      type: 'usedService',
      name: 'Dịch vụ sử dụng'
    }, {
      type: 'usedCategService',
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
      debugger
      this.formGroup.patchValue({
        logic: this.audience_filter_receive.logic,
        conditions: this.audience_filter_receive.conditions
      });
      this.listAudienceFilter_Items = this.audience_filter_receive.conditions;
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
      case 'not_contains':
        return 'không chứa';
      case 'lte':
        return 'trước';
      case 'gte':
        return 'sau';
      case 'bằng':
        return 'eq';
      case 'không bằng':
        return 'neq';
      case 'có chứa':
        return 'contains';
      case 'không chứa':
        return 'not_contains';
      case 'trước':
        return 'lte';
      case 'sau':
        return 'gte';
    }
  }

  setLogic(value) {
    this.formGroup.patchValue({
      logic: value
    });
    this.audience_filter_send.emit(this.outputAudienceFilterItems());
  }

  outputAudienceFilterItems() {
    this.formGroup.patchValue({
      conditions: this.listAudienceFilter_Items
    });
    return this.formGroup.value;
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
        case "lastSaleOrder":
          this.audience_filter_comp_data = {
            component: AudienceFilterLastTreatmentDayComponent,
            data: this.AudienceFilter_Item
          }
          break;
        case "categPartner":
          this.audience_filter_comp_data = {
            component: AudienceFilterPartnerCategoryComponent,
            data: this.AudienceFilter_Item
          }
          break;
        case "usedService":
          this.audience_filter_comp_data = {
            component: AudienceFilterServiceComponent,
            data: this.AudienceFilter_Item
          }
          break;
        case "usedCategService":
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
      this.audience_filter_send.emit(this.outputAudienceFilterItems());
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
    this.audience_filter_send.emit(this.outputAudienceFilterItems());
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
      op: null,
      value: null,
      displayValue: null
    }
    switch (item.type) {
      case "birthday":
        this.audience_filter_comp_data = {
          component: AudienceFilterBirthdayComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "lastSaleOrder":
        this.audience_filter_comp_data = {
          component: AudienceFilterLastTreatmentDayComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "categPartner":
        this.audience_filter_comp_data = {
          component: AudienceFilterPartnerCategoryComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "usedService":
        this.audience_filter_comp_data = {
          component: AudienceFilterServiceComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "usedCategService":
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
    if (this.AudienceFilter_Item.op && this.AudienceFilter_Item.value) {
      this.listAudienceFilter_Items.push(this.AudienceFilter_Item);
      this.audience_filter_send.emit(this.outputAudienceFilterItems());
      this.openAudienceFilter = false;
      this.clickedAudienceFilter = false;
      this.openAudienceFilter_Picker = false; //
    }
  }
}
