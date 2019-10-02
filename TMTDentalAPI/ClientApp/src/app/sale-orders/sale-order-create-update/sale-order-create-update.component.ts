import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { debounceTime, switchMap, tap, map } from 'rxjs/operators';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { UserService } from 'src/app/users/user.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { SaleOrderService } from '../sale-order.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { Product } from 'src/app/products/product';
import { ProductService, ProductFilter } from 'src/app/products/product.service';
import { SaleOrderLineDefaultGet } from '../sale-order-line-default-get';
import { IntlService } from '@progress/kendo-angular-intl';
import { stringToKeyValue } from '@angular/flex-layout/extended/typings/style/style-transforms';
import { Observable } from 'rxjs';
import { WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { SaleOrderLineDialogComponent } from '../sale-order-line-dialog/sale-order-line-dialog.component';
import { SaleOrderLineDisplay } from '../sale-order-line-display';

@Component({
  selector: 'app-sale-order-create-update',
  templateUrl: './sale-order-create-update.component.html',
  styleUrls: ['./sale-order-create-update.component.css']
})
export class SaleOrderCreateUpdateComponent implements OnInit {
  orderForm: FormGroup;
  id: string;
  filteredPartners: PartnerSimple[];
  filteredUsers: PartnerSimple[];
  filteredProducts: Product[];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild('userCbx', { static: true }) userCbx: ComboBoxComponent;
  @ViewChild('productCbx', { static: true }) productCbx: ComboBoxComponent;

  opened = false;

  constructor(private fb: FormBuilder, private partnerService: PartnerService,
    private userService: UserService, private route: ActivatedRoute, private saleOrderService: SaleOrderService,
    private productService: ProductService, private intlService: IntlService, private windowService: WindowService) {
  }

  ngOnInit() {
    this.orderForm = this.fb.group({
      partner: null,
      user: null,
      dateOrderD: new Date(10, 10, 2010),
      dateOrder: '',
      orderLines: this.fb.array([]),
      companyId: '',
      userId: '',
      partnerId: ''
    });

    this.route.paramMap.pipe(
      switchMap((params: ParamMap) => {
        this.id = params.get("id");
        if (this.id) {
          return this.saleOrderService.get(this.id);
        } else {
          return this.saleOrderService.defaultGet();
        }
      })).subscribe(result => {
        console.log(result);
        this.orderForm.patchValue(result);

        let dateOrder = this.intlService.parseDate(result.dateOrder);
        this.orderForm.get('dateOrderD').patchValue(dateOrder);
      });

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.filteredPartners = result;
      this.partnerCbx.loading = false;
    });

    this.userCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.userCbx.loading = true)),
      switchMap(value => this.searchUsers(value))
    ).subscribe(result => {
      this.filteredUsers = result;
      this.userCbx.loading = false;
    });

    // this.productCbx.filterChange.asObservable().pipe(
    //   debounceTime(300),
    //   tap(() => (this.productCbx.loading = true)),
    //   switchMap(value => this.searchProducts(value))
    // ).subscribe(result => {
    //   this.filteredProducts = result.items;
    //   this.productCbx.loading = false;
    // });

    this.searchUsers('').subscribe(result => {
      this.filteredUsers = result;
    });

    this.searchPartners('').subscribe(result => {
      this.filteredPartners = result;
    });
  }

  searchPartners(filter: string) {
    return this.partnerService.autocomplete(filter, true);
  }

  searchProducts(search: string) {
    let filter = new ProductFilter();
    filter.search = search;
    return this.productService.getPaged(filter);
  }

  searchUsers(filter: string) {
    return this.userService.autocomplete(filter);
  }

  userValueNormalizer(text: Observable<string>) {
    return text.pipe(
      map((a: string) => {
        return {
          value: null,
          text: a
        };
      })
    )
  };

  showAddLineModal() {
    const windowRef = this.windowService.open({
      title: 'Thêm dịch vụ điều trị',
      content: SaleOrderLineDialogComponent,
      resizable: false,
      autoFocusedElement: '[name="name"]',
    });

    this.opened = true;

    windowRef.result.subscribe((result) => {
      this.opened = false;
      if (result instanceof WindowCloseResult) {
      } else {
        var a = result as SaleOrderLineDisplay;
        this.orderLines.push(this.fb.group(a));
      }
    });

    // var product = this.productCbx.value;
    // if (product) {
    //   var val = new SaleOrderLineDefaultGet();
    //   val.productId = product.id;
    //   this.saleOrderService.defaultLineGet(val).subscribe(result => {
    //     console.log(result);
    //     this.orderLines.push(this.fb.group({
    //       listPrice: result.priceUnit
    //     }));
    //   });
    // }
  }

  lineProduct(line: FormGroup) {
    var product = line.get('product').value;
    return product ? product.name : '';
  }

  lineSalesman(line: FormGroup) {
    var salesman = line.get('salesman').value;
    return salesman ? salesman.name : '';
  }

  get orderLines() {
    return this.orderForm.get('orderLines') as FormArray;
  }

  onSave() {
    console.log(this.orderForm.value);
    var val = this.orderForm.value;
    val.dateOrder = this.intlService.formatDate(val.dateInvoice, 'g', 'en-US');
    //this.saleOrderService.create(val).subscribe(() => console.log('success'));
  }

  partnerDisplayFn(partner) {
    if (partner) {
      return partner.name;
    }
  }

  userDisplayFn(user) {
    if (user) {
      return user.name;
    }
  }
}
