import { Component, OnInit, Inject, ViewChild, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ProductCategoryService, ProductCategoryBasic, ProductCategoryPaged, ProductCategoryDisplay } from '../product-category.service';
import { ProductCategory } from '../product-category';
import { WindowRef } from '@progress/kendo-angular-dialog';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Observable } from 'rxjs';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import * as _ from 'lodash';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-product-category-dialog',
  templateUrl: './product-category-dialog.component.html',
  styleUrls: ['./product-category-dialog.component.css'],
})

export class ProductCategoryDialogComponent implements OnInit {
  myform: FormGroup;
  filterdCategories: ProductCategoryBasic[];
  defaultCateg: ProductCategoryDisplay;
  @Input() domain: any;
  @ViewChild('form', { static: true }) formView: any;
  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;
  title: string;

  @Input() public id: string;
  type: string;

  constructor(private fb: FormBuilder, private productCategoryService: ProductCategoryService,
    public activeModal: NgbActiveModal) {
  }

  ngOnInit() {
    this.myform = this.fb.group({
      name: ['', Validators.required],
      parent: null,
      sequence: null,
      serviceCateg: false,
      laboCateg: false,
      productCateg: false,
      medicineCateg: false,
    });

    setTimeout(() => {
      if (this.id) {
        this.productCategoryService.get(this.id).subscribe((result) => {
          this.myform.patchValue(result);
          if (result.parent) {
            this.filterdCategories = _.unionBy(this.filterdCategories, [result.parent], 'id');
          }
        });
      } else if (this.defaultCateg) {
        this.myform.patchValue(this.defaultCateg);
      }

      this.searchCategories().subscribe(result => (this.filterdCategories = result));

      this.categCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.categCbx.loading = true)),
        switchMap(value => this.searchCategories(value))
      ).subscribe(result => {
        this.filterdCategories = result;
        this.categCbx.loading = false;
      });
    });
  }

  searchCategories(q?: string): Observable<ProductCategoryBasic[]> {
    var val = new ProductCategoryPaged();
    val.search = q;
    return this.productCategoryService.autocomplete(val);
  }

  onSave() {
    if (!this.myform.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(result => {
      if (result) {
        this.activeModal.close(result);
      } else {
        this.activeModal.close(true);
      }
    }, err => {
      console.log(err);
    });
  }

  saveOrUpdate() {
    var val = this.myform.value;
    val.type = this.type;
    val.parentId = val.parent ? val.parent.id : null;
    if (!this.id) {
      return this.productCategoryService.create(val);
    } else {
      return this.productCategoryService.update(this.id, val);
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}

