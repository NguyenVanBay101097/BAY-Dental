import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { IntlService } from '@progress/kendo-angular-intl';
import { ProductSimple } from 'src/app/products/product-simple';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { RoutingSimple, RoutingService, RoutingPaged } from 'src/app/routings/routing.service';
import { UserSimple } from 'src/app/users/user-simple';
import { DotKhamLineService } from 'src/app/dot-khams/dot-kham-line.service';
import { UserService } from 'src/app/users/user.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import * as _ from 'lodash';
import { NotificationService } from '@progress/kendo-angular-notification';
import { DotKhamLineOperationService } from 'src/app/dot-khams/dot-kham-line-operation.service';
import { DotKhamLineChangeRoutingDialogComponent } from '../dot-kham-line-change-routing-dialog/dot-kham-line-change-routing-dialog.component';

@Component({
  selector: 'app-dot-kham-line-cu-dialog',
  templateUrl: './dot-kham-line-cu-dialog.component.html',
  styleUrls: ['./dot-kham-line-cu-dialog.component.css']
})
export class DotKhamLineCuDialogComponent implements OnInit {
  dklForm: FormGroup;
  id: string;
  filteredProducts: ProductSimple[];
  filteredRoutings: RoutingSimple[];
  filteredUsers: UserSimple[];
  isDirty = false;
  opened = false;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;
  @ViewChild('routingCbx', { static: true }) routingCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private dotKhamLineService: DotKhamLineService, private intlService: IntlService,
    private productService: ProductService, private windowRef: WindowRef, private windowService: WindowService,
    private userService: UserService, private routingService: RoutingService,
    private notificationService: NotificationService, private operationService: DotKhamLineOperationService) { }

  ngOnInit() {
    this.dklForm = this.fb.group({
      name: null,
      product: null,
      productId: null,
      dotKhamId: null,
      user: null,
      userId: null,
      state: null,
      routing: null,
      routingId: null,
      hasOps: null,
      operations: this.fb.array([])
    });

    if (this.id) {
      this.loadRecord();
    } else {
    }

    if (this.productCbx) {
      this.productCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.productCbx.loading = true)),
        switchMap(value => this.searchProducts(value))
      ).subscribe(result => {
        this.filteredProducts = result;
        this.productCbx.loading = false;
      });
    }

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });

    if (this.routingCbx) {
      this.routingCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.routingCbx.loading = true)),
        switchMap(value => this.searchRoutings(value))
      ).subscribe(result => {
        this.filteredRoutings = result;
        this.routingCbx.loading = false;
      });
    }
  }

  get operations() {
    return this.dklForm.get('operations') as FormArray;
  }

  searchProducts(search?: string) {
    var val = new ProductFilter();
    val.search = search;

    return this.productService.autocomplete2(val);
  }

  searchRoutings(search?: string) {
    var val = new RoutingPaged();
    val.search = search;

    return this.routingService.autocompleteSimple(val);
  }

  searchUsers(filter?: string) {
    return this.userService.autocomplete(filter);
  }

  changeRouting() {
    const windowRef = this.windowService.open({
      title: 'Đổi quy trình',
      content: DotKhamLineChangeRoutingDialogComponent,
      resizable: false,

      autoFocusedElement: '[name="name"]',
    });

    const instance = windowRef.content.instance;
    instance.dotKhamLineId = this.id;
    instance.productId = this.dklForm.get('product').value ? this.dklForm.get('product').value.id : null;

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        this.loadRecord();
      }
    });
  }

  loadRecord() {
    if (this.id) {
      this.dotKhamLineService.get(this.id).subscribe(result => {
        console.log(result);
        this.dklForm.patchValue(result);
        if (result.product) {
          this.filteredProducts = _.unionBy(this.filteredProducts, [result.product], 'id');
        }

        if (result.routing) {
          this.filteredRoutings = _.unionBy(this.filteredRoutings, [result.routing], 'id');
        }

        if (result.user) {
          this.filteredUsers = _.unionBy(this.filteredUsers, [result.user], 'id');
        }

        this.operations.clear();
        result.operations.forEach(op => {
          this.operations.push(this.fb.group(op));
        });
      });
    }
  }

  onSave() {
    if (!this.dklForm.valid) {
      return;
    }

    var val = this.dklForm.value;
    this.dotKhamLineService.create(val).subscribe(result => {
      this.windowRef.close(result);
    });
  }

  onUpdate() {
    if (!this.dklForm.valid) {
      return;
    }

    if (this.id) {
      var val = this.dklForm.value;
      // val.productId = val.product ? val.product.id : null;
      // val.routingId = val.routing ? val.routing.id : null;
      val.userId = val.user ? val.user.id : null;

      this.dotKhamLineService.update(this.id, val).subscribe(() => {
        this.isDirty = true;
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }
  }

  showLineState(state: string) {
    switch (state) {
      case 'progress':
        return 'Đang tiến hành';
      case 'done':
        return 'Đã xong';
      default:
        return 'Chưa tiến hành';
    }
  }

  showOperationState(state: string) {
    switch (state) {
      case 'progress':
        return 'Đang tiến hành';
      case 'done':
        return 'Đã xong';
      default:
        return 'Chưa tiến hành';
    }
  }

  startOperation(op: FormGroup) {
    this.operationService.startOperation(op.get('id').value).subscribe(() => {
      this.isDirty = true;
      this.loadRecord(); //reload data
    });
  }

  markDone(op: FormGroup) {
    this.operationService.markDone(op.get('id').value).subscribe(() => {
      this.isDirty = true;
      this.loadRecord(); //reload data
    });
  }

  onCancel() {
    if (this.isDirty) {
      this.windowRef.close(true);
    } else {
      this.windowRef.close();
    }
  }
}


