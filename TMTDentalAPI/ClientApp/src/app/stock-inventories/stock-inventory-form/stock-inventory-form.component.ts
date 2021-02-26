import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { PrintService } from 'src/app/print.service';
import { ProductCategoryBasic, ProductCategoryPaged, ProductCategoryService } from 'src/app/product-categories/product-category.service';
import { ProductService } from 'src/app/products/product.service';
import { StockInventoryService } from '../stock-inventory.service';

@Component({
  selector: 'app-stock-inventory-form',
  templateUrl: './stock-inventory-form.component.html',
  styleUrls: ['./stock-inventory-form.component.css']
})
export class StockInventoryFormComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  id: string;
  state: string;
  valueSearch: string;
  // stockInventory: any;
  filterdCategories: ProductCategoryBasic[] = [];
  public minDateTime: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public maxDateTime: Date = new Date(new Date(this.monthEnd.setHours(23, 59, 59)).toString());
  submitted = false;

  @ViewChild('categCbx', { static: true }) categCbx: ComboBoxComponent;

  filterInventories = [
    { name: 'Tất cả sản phẩm', value: 'none' },
    { name: 'Một nhóm sản phẩm', value: 'category' },
    { name: 'Chọn sản phẩm thủ công', value: 'partial' },
  ];

  constructor(
    private fb: FormBuilder,
    private modalService: NgbModal,
    private route: ActivatedRoute,
    private router: Router,
    private notificationService: NotificationService,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService,
    private stockInventorySevice: StockInventoryService,
    private authService: AuthService,
    private intlService: IntlService,
    private printService: PrintService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: null,
      dateObj: [null, Validators.required],
      date: null,
      locationId: null,
      location: null,
      productId: null,
      categoryId: null,
      category: null,
      filter: null,
      note: null,
      exhausted: false,
      state: 'draft',
      companyId: null,
      lines: this.fb.array([])
    });


    this.route.queryParamMap.subscribe(params => {
      this.id = params.get('id');
      this.loadDataFromApi();
    });

    setTimeout(() => {

      this.categCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.categCbx.loading = true)),
        switchMap(value => this.searchCategories(value))
      ).subscribe(result => {
        this.filterdCategories = result;
        this.categCbx.loading = false;
      });

      this.loadCategories();
    });



  }

  get f() { return this.formGroup.controls; }

  get stockInventory() { return this.formGroup.value; }

  get stateControl() {
    return this.formGroup.get('state').value;
  }

  get filterControl() {
    return this.formGroup.get('filter').value;
  }

  get lines() {
    return this.formGroup.get('lines') as FormArray;
  }

  loadDataFromApi() {
    if (this.id) {
      this.stockInventorySevice.get(this.id).subscribe(result => {
        this.formGroup.patchValue(result);
        let date = new Date(result.date);
        this.formGroup.get('dateObj').patchValue(date);

        if (result.category) {
          this.filterdCategories = _.unionBy(this.filterdCategories, [result.category], "id");
        }

        let control = this.formGroup.get('lines') as FormArray;
        control.clear();
        result.lines.forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
        console.log(control);
      });
    } else {
      var companyId = this.authService.userInfo.companyId;
      this.stockInventorySevice.getDefault(companyId).subscribe(result => {
        this.formGroup.patchValue(result);
        let date = new Date(result.date);
        this.formGroup.get('dateObj').patchValue(date);

        let control = this.formGroup.get('lines') as FormArray;
        control.clear();
        result.lines.forEach(line => {
          var g = this.fb.group(line);
          control.push(g);
        });
      });
    }
  }

  createNew() {
    this.router.navigate(['stock-inventories/form']);
  }

  loadCategories() {
    this.searchCategories().subscribe((result) => {
      this.filterdCategories = _.unionBy(this.filterdCategories, result, "id");
    });
  }

  onChangeFilter() {
    var res = this.formGroup.value;
    if (res.filter !== 'category') {
      res.category = null;
    }
    this.formGroup.patchValue(res);

  }

  searchCategories(q?: string) {
    var val = new ProductCategoryPaged();
    val.search = q || '';
    val.type = 'product';
    return this.productCategoryService.autocomplete(val);
  }

  computeForm(val) {
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm");
    val.locationId = val.location ? val.location.id : null;
    val.productId = val.product ? val.product.id : null;
    val.categoryId = val.category ? val.category.id : null;
    val.companyId = this.authService.userInfo.companyId;
    return val;
  }


  onSave() {
    if (this.formGroup.invalid) {
      return false;
    }

    var val = this.formGroup.value;
    val = this.computeForm(val);

    if (this.id) {
      this.stockInventorySevice.update(this.id, val).subscribe(
        () => {
          this.notificationService.show({
            content: 'Cập nhật thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.loadDataFromApi();
        }
      );
    } else {
      this.stockInventorySevice.create(val).subscribe(
        result => {
          this.router.navigate(['/stock-inventories/form'], { queryParams: { id: result.id } });
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
        }
      );
    }

  }

  addLine(val) {
    var res = this.fb.group(val);

    // line.teeth = this.fb.array(line.teeth);
    if (!this.lines.controls.some(x => x.value.productId === res.value.productId)) {
      this.lines.push(res);
    } else {
      var line = this.lines.controls.find(x => x.value.productId === res.value.productId);
      if (line) {
        line.value.theoreticalQty += 1;
        line.patchValue(line.value);
      }
    }
  }

  onChangeSearch(value) {
    this.valueSearch = value;
  }

  prepareInventory() {
    if (this.id) {
      var val = this.formGroup.value;
      val = this.computeForm(val);
      this.stockInventorySevice.update(this.id, val).subscribe(
        () => {
          this.stockInventorySevice.prepareInventory([this.id]).subscribe(rs => {
            this.loadDataFromApi();
          })
        }
      );
    }
  }

  actionDone() {
    if (this.id) {
      var val = this.formGroup.value;
      val = this.computeForm(val);
      this.stockInventorySevice.update(this.id, val).subscribe(() => {
        this.stockInventorySevice.actionDone([this.id]).subscribe(() => {
          this.notificationService.show({
            content: 'Xác nhận thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });
          this.loadDataFromApi();
        })
      })
    }
  }

  getFilter(filter) {
    switch (filter) {
      case 'none':
        return 'Tất cả sản phẩm';
      case 'category':
        return 'Nhóm sản phẩm';
      case 'partial':
        return 'Chọn sản phẩm thủ công';
    }
  }


}
