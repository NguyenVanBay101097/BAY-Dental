import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AudienceFilterItem } from 'src/app/tcare/tcare.service';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';

@Component({
  selector: 'app-audience-filter-service-category',
  templateUrl: './audience-filter-service-category.component.html',
  styleUrls: ['./audience-filter-service-category.component.css']
})
export class AudienceFilterServiceCategoryComponent implements OnInit {
  
  @Input() dataReceive: any;
  @Output() dataSend = new EventEmitter<boolean>();
  formGroup: FormGroup;

  AudienceFilter_Picker = {
    formula_types: ['contains', 'doesnotcontain'],
    formula_values: [],
    formula_displays: null,
  }

  selected_AudienceFilter_Picker: AudienceFilterItem;

  showButtonCreateProductCategory: boolean = false;
  searchProductCategoryUpdate = new Subject<string>();

  constructor(private fb: FormBuilder, 
    private notificationService: NotificationService, 
    private productCategoryService: ProductCategoryService) { }

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
    this.loadListProductCategory();
    this.searchProductCategoryUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadListProductCategory();
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

  loadListProductCategory() {
    var val = new ProductCategoryPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.formGroup.get('name').value || '';
    val.type = 'service';
    this.productCategoryService.getPaged(val).subscribe(res => {
      var listItems = res['items'];
      
      if (listItems.length == 0) {
        this.showButtonCreateProductCategory = true;
      } else {
        this.showButtonCreateProductCategory = false;
      }
      this.AudienceFilter_Picker.formula_values = [];
      for (let i = 0; i < listItems.length; i++) {
        this.AudienceFilter_Picker.formula_values.push(listItems[i].name); // Add formula_values
      }
    }, err => {
      console.log(err);
    })
  }

  createProductCategory() {
    var val = this.formGroup.value;
    val.type = 'service';
    val.parentId = null;
    this.productCategoryService.create(val).subscribe(res => {
      this.notificationService.show({
        content: 'Tạo nhãn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.loadListProductCategory();
      // console.log(res);
    }, err => {
      console.log(err);
    })
  }
}
