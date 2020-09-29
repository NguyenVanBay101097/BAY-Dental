import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductCategoryService, ProductCategoryPaged } from 'src/app/product-categories/product-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';

@Component({
  selector: 'app-audience-filter-service-category',
  templateUrl: './audience-filter-service-category.component.html',
  styleUrls: ['./audience-filter-service-category.component.css']
})
export class AudienceFilterServiceCategoryComponent implements OnInit {
  
  formGroup: FormGroup;
  filteredCategs = [];
  submitted = false;
  type: string;
  name: string;
  @Output() saveClick = new EventEmitter<any>();
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  data: any;

  constructor(private fb: FormBuilder, 
    private productCategoryService: ProductCategoryService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      op: 'contains',
      categ: [null, Validators.required]
    });

    setTimeout(() => {
      if (this.data) {
        var categ = this.data.value;

        this.formGroup.patchValue({
          op: this.data.op,
          categ: categ
        });

        this.filteredCategs.push(categ);
      }

      this.loadFilteredCategs();

      this.categCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.categCbx.loading = true)),
        switchMap(value => this.searchCategories(value))
      ).subscribe((result: any) => {
        this.filteredCategs = result;
        this.categCbx.loading = false;
      });
    });
  }

  loadFilteredCategs() {
    this.searchCategories().subscribe(result => {
      this.filteredCategs = _.unionBy(this.filteredCategs, result, 'id');
    });
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'service';
    return this.productCategoryService.autocomplete(val);
  }

  getOpDisplay(op) {
    switch (op) {
      case 'contains':
        return 'Chứa';
      case 'not_contains':
        return 'Không chứa';
      default:
        return '';
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;

    var res = {
      type: this.type,
      op: value.op,
      name: this.name + " " + this.getOpDisplay(value.op) + " " + value.categ.name + ". ",
      value: { id: value.categ.id, name: value.categ.name }
    };

    this.saveClick.emit(res);
  }
}
