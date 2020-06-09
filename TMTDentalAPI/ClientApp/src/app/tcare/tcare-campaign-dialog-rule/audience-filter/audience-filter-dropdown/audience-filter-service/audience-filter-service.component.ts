import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TCareRuleCondition } from 'src/app/tcare/tcare.service';
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
  @Output() dataSend = new EventEmitter<any>();
  formGroup: FormGroup;

  AudienceFilter_Picker = {
    formula_types: ['contains', 'not_contains'],
    formula_values: [],
    formula_displays: []
  }

  selected_AudienceFilter_Picker: TCareRuleCondition;

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
    if (!this.dataReceive.op) {
      this.selected_AudienceFilter_Picker.op = this.AudienceFilter_Picker.formula_types[0]
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

  loadListService() {
    var val = new ProductPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.formGroup.get('name').value || '';
    val.type2 = 'service';
    this.productService.getPaged(val).subscribe(res => {
      var listItems = res['items'];
      // filter
      var index_item = listItems.findIndex(x => x.id == this.selected_AudienceFilter_Picker.value);
      if (index_item >= 0) {
        listItems.splice(index_item, 1);
      }
      var temp_item = {
        id: this.selected_AudienceFilter_Picker.value,
        name: this.selected_AudienceFilter_Picker.displayValue,
        categName: null,
        listPrice: null,
        type: null,
        defaultCode: null,
        qtyAvailable: null,
        uomId: null,
        uom: null
      };
      listItems.splice(0, 0, temp_item);

      if (listItems.length == 0) {
        this.showButtonCreateService = true;
      } else {
        this.showButtonCreateService = false;
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
}
