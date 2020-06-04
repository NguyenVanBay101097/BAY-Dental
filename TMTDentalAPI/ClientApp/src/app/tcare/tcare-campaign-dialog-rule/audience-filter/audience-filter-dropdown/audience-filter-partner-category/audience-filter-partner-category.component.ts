import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AudienceFilterItem } from 'src/app/tcare/tcare.service';
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
    formula_types: ['contains', 'doesnotcontain'],
    formula_values: [],
    formula_displays: null,
  }

  selected_AudienceFilter_Picker: AudienceFilterItem;

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
    if (!this.dataReceive.formula_type) {
      this.selected_AudienceFilter_Picker.formula_type = this.AudienceFilter_Picker.formula_types[0]
    }
    if (this.dataReceive.formula_value) {
      this.formGroup.patchValue({ name: this.selected_AudienceFilter_Picker.formula_value });
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
      case 'doesnotcontain':
        return 'không chứa';
      case 'có chứa':
        return 'contains';
      case 'không chứa':
        return 'doesnotcontain';
    }
  }

  selectFormulaType(item) {
    this.selected_AudienceFilter_Picker.formula_type = item;
  }

  selectFormulaValue(item, i) {
    this.selected_AudienceFilter_Picker.formula_value = item;
    this.dataSend.emit(false);
  }

  loadListPartnerCategory() {
    var val = new PartnerCategoryPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.formGroup.get('name').value || '';
    this.partnerCategoryService.getPaged(val).subscribe(res => {
      var listItems = res['items'];
      
      if (listItems.length == 0) {
        this.showButtonCreatePartnerCategory = true;
      } else {
        this.showButtonCreatePartnerCategory = false;
      }
      this.AudienceFilter_Picker.formula_values = [];
      for (let i = 0; i < listItems.length; i++) {
        this.AudienceFilter_Picker.formula_values.push(listItems[i].name); // Add formula_values
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
