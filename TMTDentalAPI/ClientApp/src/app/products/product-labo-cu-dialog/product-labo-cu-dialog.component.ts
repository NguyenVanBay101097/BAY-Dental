import { Component, OnInit, ViewChild } from '@angular/core';
import { ProductService } from '../product.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import * as _ from 'lodash';
import { UomService } from 'src/app/uoms/uom.service';

@Component({
  selector: 'app-product-labo-cu-dialog',
  templateUrl: './product-labo-cu-dialog.component.html',
  styleUrls: ['./product-labo-cu-dialog.component.css']
})
export class ProductLaboCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  title: string;
  filterdCategories: ProductCategoryBasic[] = [];
  filterdUoMs = [];
  filterdUoMPOs = [];
  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  constructor(private productService: ProductService, public activeModal: NgbActiveModal,
    private productCategoryService: ProductCategoryService, private uoMService: UomService,
    private fb: FormBuilder) {
  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      saleOK: false,
      purchaseOK: false,
      categ: [null, Validators.required],
      uom: null,
      uompo: null,
      type: 'consu',
      type2: 'labo',
      listPrice: 1,
      standardPrice: 0,
      companyId: null,
      defaultCode: '',
      keToaNote: null,
      keToaOK: true,
      isLabo: false,
      purchasePrice: 0,
      laboPrice:0
    });

    this.categCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.categCbx.loading = true)),
      switchMap(value => this.searchCategories(value))
    ).subscribe(result => {
      this.filterdCategories = result;
      this.categCbx.loading = false;
    });
    this.loadCate();

    this.default();
  }

  default() {
    if (this.id) {
      this.productService.get(this.id).subscribe((result: any) => {
        this.formGroup.patchValue(result);

        this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ], 'id');

        this.filterdUoMs = _.unionBy(this.filterdUoMs, [result.uom], 'id');

        if (result.uompo) {
          this.uoMService.getPaged({ categoryId: result.uompo.categoryId }).subscribe((result2: any) => {
            this.filterdUoMPOs = result2.items;
            this.filterdUoMPOs = _.unionBy(this.filterdUoMPOs, [result.uompo], 'id');
          });
        }
      });
    } else {
      this.productService.defaultGet().subscribe((result: any) => {
        this.formGroup.patchValue(result);

        if (result.categ) {
          this.filterdCategories = _.unionBy(this.filterdCategories, [result.categ as ProductCategoryBasic], 'id');
        }

        if (result.uom) {
          this.filterdUoMs = _.unionBy(this.filterdUoMs, [result.uom], 'id');
        }

        if (result.uompo) {
          this.uoMService.getPaged({ categoryId: result.uompo.categoryId }).subscribe((result2: any) => {
            this.filterdUoMPOs = result2.items;
            this.filterdUoMPOs = _.unionBy(this.filterdUoMPOs, [result.uompo], 'id');
          });
        }

        this.formGroup.get('type').setValue('consu');
        this.formGroup.get('type2').setValue('labo');
        this.formGroup.get('saleOK').setValue(false);
        this.formGroup.get('purchaseOK').setValue(false);
        this.formGroup.get('keToaOK').setValue(true);
      });
    } }

  loadCate() {
    this.searchCategories().subscribe(result => {
      this.filterdCategories = result;
    });
  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'labo';
    return this.productCategoryService.autocomplete(val);
  }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }
    var val = this.formGroup.value;
    val.categId = val.categ.id;
    val.uoMIds = [];
    val.uomId = val.uom.id;
    val.uompoId = val.uompo.id;
    val.uoMIds.push(val.uompo.id);
    val.uoMIds.push(val.uom.id);
    if (this.id) {
      this.productService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      });
    } else {
      return this.productService.create(val).subscribe(result => {
        this.activeModal.close(result);
      });;
    }
  }
}

