import { Component, OnInit, Input } from '@angular/core';
import { AudienceFilterItem } from 'src/app/tcare/tcare.service';
import { Subject } from 'rxjs';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ProductService, ProductPaged } from 'src/app/products/product.service';

@Component({
  selector: 'app-audience-filter-service',
  templateUrl: './audience-filter-service.component.html',
  styleUrls: ['./audience-filter-service.component.css']
})
export class AudienceFilterServiceComponent implements OnInit {

  @Input() dataReceive: any;
  formGroup: FormGroup;

  AudienceFilter_Picker = {
    formula_types: ['contains', 'doesnotcontain'],
    formula_values: [],
    formula_displays: null,
  }

  selected_AudienceFilter_Picker: AudienceFilterItem;

  showButtonCreateService: boolean = false;
  searchServiceUpdate = new Subject<string>();

  constructor(private fb: FormBuilder, 
    private notificationService: NotificationService, 
    private productService: ProductService) { }

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
    this.loadListService();
    this.searchServiceUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadListService();
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
  }

  loadListService() {
    var val = new ProductPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.formGroup.get('name').value || '';
    val.type2 = 'service';
    this.productService.getPaged(val).subscribe(res => {
      var listItems = res['items'];
      
      if (listItems.length == 0) {
        this.showButtonCreateService = true;
      } else {
        this.showButtonCreateService = false;
      }
      this.AudienceFilter_Picker.formula_values = [];
      for (let i = 0; i < listItems.length; i++) {
        this.AudienceFilter_Picker.formula_values.push(listItems[i].name); // Add formula_values
      }
    }, err => {
      console.log(err);
    })
  }
}
