import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ProductSimple } from 'src/app/products/product-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { RoutingService, RoutingLineDisplay } from '../routing.service';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { RoutingLineCuDialogComponent } from '../routing-line-cu-dialog/routing-line-cu-dialog.component';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import * as _ from 'lodash';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-routing-create-update',
  templateUrl: './routing-create-update.component.html',
  styleUrls: ['./routing-create-update.component.css']
})
export class RoutingCreateUpdateComponent implements OnInit {
  routingForm: FormGroup;
  filteredProducts: ProductSimple[];
  opened = false;
  id: string;

  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private productService: ProductService,
    private routingService: RoutingService, private windowService: WindowService,
    private route: ActivatedRoute, private notificationService: NotificationService,
    private router: Router) { }

  ngOnInit() {
    this.routingForm = this.fb.group({
      product: [null, Validators.required],
      name: null,
      nameGet: null,
      lines: this.fb.array([]),
    });

    this.id = this.route.snapshot.params['id'];
    if (this.id) {
      this.routingService.get(this.id).subscribe(result => {
        if (result.product) {
          this.filteredProducts = _.unionBy(this.filteredProducts, [result.product], 'id');
        }
        this.routingForm.patchValue(result);
        result.lines.forEach(line => {
          this.lines.push(this.fb.group(line));
        });
      });
    }

    this.productCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.productCbx.loading = true)),
      switchMap(value => this.searchProducts(value))
    ).subscribe(result => {
      this.filteredProducts = result;
      this.productCbx.loading = false;
    });

    this.loadProducts();
  }

  loadProducts() {
    this.searchProducts().subscribe(result => this.filteredProducts = result);
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.search = search;
    val.saleOK = true;
    return this.productService.autocomplete2(val);
  }

  get lines() {
    return this.routingForm.get('lines') as FormArray;
  }

  onSave() {
    if (!this.routingForm.valid) {
      return;
    }

    var val = this.routingForm.value;
    val.productId = val.product.id;
    this.routingService.create(val).subscribe(result => {
      this.router.navigate(['routings/edit/', result.id]);
    });
  }

  onNew() {
    this.router.navigate(['routings/create']);
  }

  onUpdate() {
    if (!this.routingForm.valid) {
      return;
    }

    var val = this.routingForm.value;
    val.productId = val.product.id;
    this.routingService.update(this.id, val).subscribe(() => {
      this.notificationService.show({
        content: 'Lưu thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      })
      this.loadRecord();
    });
  }

  loadRecord() {
    if (this.id) {
      this.routingService.get(this.id).subscribe(result => {

        if (result.product) {
          this.filteredProducts = _.unionBy(this.filteredProducts, [result.product], 'id');
        }

        this.routingForm.patchValue(result);
        let control = this.routingForm.get('lines') as FormArray;
        control = this.fb.array([]); //reset form array
        result.lines.forEach(line => {
          control.push(this.fb.group(line));
        });
      });
    }
  }

  addLine() {
    var line = new RoutingLineDisplay();
    this.lines.push(this.fb.group(line));
  }

  showAddLineWindow() {
    const windowRef = this.windowService.open({
      title: `Thêm công đoạn`,
      content: RoutingLineCuDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        var line = result as RoutingLineDisplay;
        this.lines.push(this.fb.group(line));
      }
    });
  }

  editLine(item: FormGroup) {
    const windowRef = this.windowService.open({
      title: `sửa công đoạn`,
      content: RoutingLineCuDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.item = item.value;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (!(result instanceof WindowCloseResult)) {
        var a = result as RoutingLineDisplay;
        item.patchValue(a);
      }
    });
  }

  deleteLine(index) {
    this.lines.removeAt(index);
  }
}
