import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SaleOrderLineOnChangeProduct, SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { ProductBasic2, ProductPaged, ProductService } from 'src/app/products/product.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent } from '../partner-customer-treatment-history-form-add-service-dialog/partner-customer-treatment-history-form-add-service-dialog.component';

@Component({
  selector: 'app-partner-customer-treatment-history-form-service-list',
  templateUrl: './partner-customer-treatment-history-form-service-list.component.html',
  styleUrls: ['./partner-customer-treatment-history-form-service-list.component.css']
})
export class PartnerCustomerTreatmentHistoryFormServiceListComponent implements OnInit {
  @Output() newEventEmiter = new EventEmitter<any>()
  limit: number = 20;
  skip: number = 0;
  search: string;
  partnerId: string;
  saleLineForm: FormGroup;
  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  listProductServices: ProductBasic2[] = [];
  searchUpdate = new Subject<string>();

  thTable_services = [
    { name: 'Dịch vụ' },
    { name: 'Đơn giá' }
  ]

  services: any[] = [
    {
      service: "this is service name",
      money: "this is money",
    }, {
      service: "this is service name aaaaaaaaaaaaaa",
      money: "this is money",
    }
  ];

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private modalService: NgbModal,
    private toothService: ToothService,
    private saleLineService: SaleOrderLineService,
    private toothCategoryService: ToothCategoryService,
  ) { }

  ngOnInit() {
    this.saleLineForm = this.fb.group({
      name: '',
      product: null,
      productId: null,
      priceUnit: 0,
      productUOMQty: 1,
      discount: 0,
      discountType: 'percentage',
      discountFixed: 0,
      priceSubTotal: 1,
      amountPaid: 0,
      amountResidual: 0,
      diagnostic: null,
      toothCategory: null,
      state: 'draft',
      employee: null
    });

    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadDataDefault();
      });

    setTimeout(() => {
      this.loadDefaultToothCategory().subscribe(result => {
        this.saleLineForm.get('toothCategory').patchValue(result);
        this.loadTeethMap(result);
      })
    });

    this.loadDataDefault();
  }

  loadDataDefault() {
    var val = new ProductPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || "";
    val.type2 = "service";

    this.productService
      .getPaged(val).subscribe(
        (res) => {
          this.listProductServices = res.items;
        },
        (err) => {
          console.log(err);
        }
      );
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
  }

  processTeeth(teeth: ToothDisplay[]) {
    this.hamList = {
      '0_up': { '0_right': [], '1_left': [] },
      '1_down': { '0_right': [], '1_left': [] }
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === '1_left') {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  getPriceSubTotal() {
    var discountType = this.discountTypeValue;
    var price = discountType == 'percentage' ? this.getPriceUnit() * (1 - this.getDiscount() / 100) :
      Math.max(0, this.getPriceUnit() - this.discountFixedValue);
    var subtotal = price * this.getQuantity();
    return subtotal;
  }

  getPriceUnit() {
    return this.saleLineForm.get('priceUnit').value;
  }

  getQuantity() {
    return this.saleLineForm.get('productUOMQty').value;
  }

  getDiscount() {
    return this.saleLineForm.get('discount').value;
  }

  get discountFixedValue() {
    return this.saleLineForm.get('discountFixed').value;
  }

  get discountTypeValue() {
    return this.saleLineForm.get('discountType').value;
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }


  loadData(productId) {
    if (productId) {
      var val = new SaleOrderLineOnChangeProduct();
      val.productId = productId;
      this.saleLineService.onChangeProduct(val).subscribe(result => {
        console.log(result);
        this.saleLineForm.patchValue(result);
      });
    }
  }

  addServiceToSaleOrder(item) {
    item.priceSubTotal = 0;
    item.amountResidual = 0;
    item.priceUnit = item.listPrice;
    item.diagnostic = "";
    item.discountType = "percentage";
    item.amountPaid = 0;
    item.productUOMQty = 1;
    item.discount = 0;
    item.discountFixed = 0;
    this.newEventEmiter.emit(item);
  }
}