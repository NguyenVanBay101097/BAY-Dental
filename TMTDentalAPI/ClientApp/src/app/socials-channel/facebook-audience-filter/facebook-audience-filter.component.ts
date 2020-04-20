import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AudienceFilterTagComponent } from '../facebook-audience-filter-dropdown/audience-filter-tag/audience-filter-tag.component';
import { AudienceFilterGenderComponent } from '../facebook-audience-filter-dropdown/audience-filter-gender/audience-filter-gender.component';
import { AudienceFilterInputComponent } from '../facebook-audience-filter-dropdown/audience-filter-input/audience-filter-input.component';
import { AudienceFilterItem } from '../facebook-mass-messaging.service';
declare var $ :any;

@Component({
  selector: 'app-facebook-audience-filter',
  templateUrl: './facebook-audience-filter.component.html',
  styleUrls: ['./facebook-audience-filter.component.css']
})
export class FacebookAudienceFilterComponent implements OnInit {
  @Input() audience_filter_receive: any;
  @Output() audience_filter_send = new EventEmitter<any>();;

  constructor() { }

  ngOnInit() {
    if (this.audience_filter_receive) {
      this.listAudienceFilter_Items = this.convertAudienceFilterItemsToArray(this.audience_filter_receive);
    }
    console.log(this.listAudienceFilter_Items);

    $(document).on('click', '.allow-focus', function (e) {
      e.stopPropagation();
    });
  } 

  audience_filter_comp_data: { component, data };
  listAudienceFilter_Items: any[] = [];
  selectedAudienceFilter_Item: any;
  openAudienceFilter_Picker: boolean = false;
  openAudienceFilter: boolean = false;
  AudienceFilter_Item: AudienceFilterItem;
  listAudienceFilter_Picker = [
    {
      type: 'Tag',
      name: 'Nhãn'
    }, {
      type: 'Gender',
      name: 'Giới tính'
    }, {
      type: 'FirstName',
      name: 'Tên'
    }, {
      type: 'LastName',
      name: 'Họ'
    }
  ]
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
    }
  }
  convertAudienceFilterItemsToString() {
    var result = {
      type: "and",
      items: this.listAudienceFilter_Items
    }
    return JSON.stringify(result);
  }
  convertAudienceFilterItemsToArray(listAudienceFilter_Items_String) {
    return JSON.parse(listAudienceFilter_Items_String).items;
  }
  clickedAudienceFilter_Item: boolean = false;
  clickAudienceFilter_Item(item) {
    if (this.AudienceFilter_Item !== item) {
      this.AudienceFilter_Item = item;
      switch (item.type) {
        case "Tag":
          this.audience_filter_comp_data = {
            component: AudienceFilterTagComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "Gender":
          this.audience_filter_comp_data = {
            component: AudienceFilterGenderComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "FirstName":
          this.audience_filter_comp_data = {
            component: AudienceFilterInputComponent,
            data: this.AudienceFilter_Item
          }
        break;
        case "LastName":
          this.audience_filter_comp_data = {
            component: AudienceFilterInputComponent,
            data: this.AudienceFilter_Item
          }
        break;
      } 
    }
    this.selectedAudienceFilter_Item = item;
    this.clickedAudienceFilter_Item = true;
  }
  count_clickOutside: number = 1;
  clickOutsideAudienceFilter_Item_Picker() {
    if (this.count_clickOutside === 2) {
      if (this.clickedAudienceFilter_Item === true) {
        this.clickedAudienceFilter_Item = false;
      } else {
        this.selectedAudienceFilter_Item = null;
      }
      this.count_clickOutside = 1;
    } else {
      this.count_clickOutside += 1;
    }
  }
  deleteAudienceFilterItem(index) {
    this.selectedAudienceFilter_Item = null;
    this.listAudienceFilter_Items.splice(index, 1);
    this.audience_filter_send.emit(this.convertAudienceFilterItemsToString());
  }
  clickedAudienceFilter: boolean = false;
  clickAudienceFilter() {
    this.openAudienceFilter = true;
    this.openAudienceFilter_Picker = false;
    this.clickedAudienceFilter = true;
  }
  clickOutsideAudienceFilter() {
    if (this.count_clickOutside === 2) {
      if (this.clickedAudienceFilter === true) {
        this.clickedAudienceFilter = false;
      } else {
        this.openAudienceFilter = false;
        this.openAudienceFilter_Picker = false;
      }
      this.count_clickOutside = 1;
    } else {
      this.count_clickOutside += 1;
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
      case "Tag":
        this.audience_filter_comp_data = {
          component: AudienceFilterTagComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "Gender":
        this.audience_filter_comp_data = {
          component: AudienceFilterGenderComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "FirstName":
        this.audience_filter_comp_data = {
          component: AudienceFilterInputComponent,
          data: this.AudienceFilter_Item
        }
        break;
      case "LastName":
        this.audience_filter_comp_data = {
          component: AudienceFilterInputComponent,
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
      this.openAudienceFilter_Picker = false;
    }
  }
}
