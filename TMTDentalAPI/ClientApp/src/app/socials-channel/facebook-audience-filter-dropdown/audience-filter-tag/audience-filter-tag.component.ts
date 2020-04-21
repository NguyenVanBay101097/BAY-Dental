import { Component, OnInit, Input } from '@angular/core';
import { AudienceFilterItem } from '../../facebook-mass-messaging.service';
import { FacebookTagsPaged, FacebookTagsService } from '../../facebook-tags.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-audience-filter-tag',
  templateUrl: './audience-filter-tag.component.html',
  styleUrls: ['./audience-filter-tag.component.css']
})
export class AudienceFilterTagComponent implements OnInit {

  @Input() dataReceive: any;

  AudienceFilter_Picker = {
    formula_types: ['eq', 'neq'],
    formula_values: [],
    formula_displays: null,
  }

  selected_AudienceFilter_Picker: AudienceFilterItem;

  showButtonCreateTag: boolean = false;
  inputSearchTag: string;
  searchTagUpdate = new Subject<string>();

  constructor(private notificationService: NotificationService, private facebookTagsService: FacebookTagsService) { }

  ngOnInit() {
    this.selected_AudienceFilter_Picker = this.dataReceive;
    if (!this.dataReceive.formula_type) {
      this.selected_AudienceFilter_Picker.formula_type = this.AudienceFilter_Picker.formula_types[0]
    }
    if (this.dataReceive.formula_value) {
      this.inputSearchTag = this.selected_AudienceFilter_Picker.formula_value;
    }
    this.loadListTags();
    this.searchTagUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadListTags();
      });
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

  selectFormulaType(item) {
    this.selected_AudienceFilter_Picker.formula_type = item;
  }
  
  selectFormulaValue(item, i) {
    this.selected_AudienceFilter_Picker.formula_value = item;
  }

  loadListTags() {
    var val = new FacebookTagsPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.inputSearchTag || '';
    this.facebookTagsService.getTags(val).subscribe(res => {
      var listTags = res['items'];
      
      if (listTags.length == 0) {
        this.showButtonCreateTag = true;
      } else {
        this.showButtonCreateTag = false;
      }
      this.AudienceFilter_Picker.formula_values = [];
      for (let i = 0; i < listTags.length; i++) {
        this.AudienceFilter_Picker.formula_values.push(listTags[i].name); // Add formula_values Tag
      }
    }, err => {
      console.log(err);
    })
  }

  createTag() {
    var val = {
      name: this.inputSearchTag
    };
    this.facebookTagsService.create(val).subscribe(res => {
      this.notificationService.show({
        content: 'Tạo nhãn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadListTags();
      // console.log(res);
    }, err => {
      console.log(err);
    })
  }
}
