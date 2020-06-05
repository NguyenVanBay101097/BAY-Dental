import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TCareRuleCondition } from 'src/app/tcare/tcare.service';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { PartnerCategoryService, PartnerCategoryPaged } from 'src/app/partner-categories/partner-category.service';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-audience-filter-partner-category',
  templateUrl: './audience-filter-partner-category.component.html',
  styleUrls: ['./audience-filter-partner-category.component.css']
})
export class AudienceFilterPartnerCategoryComponent implements OnInit {
  
  @Input() dataReceive: any;
  @Output() dataSend = new EventEmitter<any>();
  formGroup: FormGroup;

  AudienceFilter_Picker = {
    formula_types: ['contains', 'not_contains'],
    formula_values: [],
    formula_displays: []
  }

  selected_AudienceFilter_Picker: TCareRuleCondition;

  showButtonCreatePartnerCategory: boolean = false;
  searchPartnerCategoryUpdate = new Subject<string>();

  constructor(private fb: FormBuilder, 
    private notificationService: NotificationService, 
    private partnerCategoryService: PartnerCategoryService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ""
    });

    this.selected_AudienceFilter_Picker = this.dataReceive;
    if (!this.dataReceive.op) {
      this.selected_AudienceFilter_Picker.op = this.AudienceFilter_Picker.formula_types[0]
    }

    this.loadListPartnerCategory();
    this.searchPartnerCategoryUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadListPartnerCategory();
      });
  }

  convertFormulaType(item) {
    switch (item) {
      case 'contains':
        return 'có chứa';
      case 'not_contains':
        return 'không chứa';
      case 'có chứa':
        return 'contains';
      case 'không chứa':
        return 'not_contains';
    }
  }

  selectFormulaType(item) {
    this.selected_AudienceFilter_Picker.op = item;
  }

  selectFormulaValue(item, i) {
    this.selected_AudienceFilter_Picker.value = this.AudienceFilter_Picker.formula_values[i];
    this.selected_AudienceFilter_Picker.displayValue = item;
    this.dataSend.emit(false);
  }

  loadListPartnerCategory() {
    var val = new PartnerCategoryPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.formGroup.get('name').value || '';
    this.partnerCategoryService.getPaged(val).subscribe(res => {
      var listItems = res['items'];
      // filter
      var index_item =  listItems.findIndex(x => x.id == this.selected_AudienceFilter_Picker.value);
      if (index_item >= 0) {
        listItems.splice(index_item, 1);
      }
      listItems.splice(0, 0, {
        id: this.selected_AudienceFilter_Picker.value,
        name: this.selected_AudienceFilter_Picker.displayValue,
        completeName: this.selected_AudienceFilter_Picker.displayValue
      });
      
      if (listItems.length == 0) {
        this.showButtonCreatePartnerCategory = true;
      } else {
        this.showButtonCreatePartnerCategory = false;
      }
      this.AudienceFilter_Picker.formula_values = [];
      this.AudienceFilter_Picker.formula_displays = [];
      for (let i = 0; i < listItems.length; i++) {
        this.AudienceFilter_Picker.formula_values.push(listItems[i].id); 
        this.AudienceFilter_Picker.formula_displays.push(listItems[i].name); 
      }
    }, err => {
      console.log(err);
    })
  }

  createPartnerCategory() {
    var val = this.formGroup.value;
    val.parentId = null;
    this.partnerCategoryService.create(val).subscribe(res => {
      this.notificationService.show({
        content: 'Tạo nhãn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadListPartnerCategory();
      // console.log(res);
    }, err => {
      console.log(err);
    })
  }
}
