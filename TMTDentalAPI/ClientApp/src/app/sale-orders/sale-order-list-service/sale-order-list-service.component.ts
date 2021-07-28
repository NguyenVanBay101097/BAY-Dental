import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormArray, FormBuilder } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { fromEvent, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { Product } from 'src/app/products/product';
import { ProductServiceCuDialogComponent } from 'src/app/products/product-service-cu-dialog/product-service-cu-dialog.component';
import { ProductBasic2, ProductPaged, ProductService } from 'src/app/products/product.service';
import { ProductsOdataService } from 'src/app/shared/services/ProductsOdata.service';
import { ToothDisplay } from 'src/app/teeth/tooth.service';

@Component({
  selector: 'app-sale-order-list-service',
  templateUrl: './sale-order-list-service.component.html',
  styleUrls: ['./sale-order-list-service.component.css']
})
export class SaleOrderListServiceComponent implements OnInit {

  @ViewChild("searchInput", { static: true }) searchInput: ElementRef;
  @Output() newEventEmiter = new EventEmitter<any>()
  limit: number = 20;
  skip: number = 0;
  toothDisplays: ToothDisplay[] = []
  search: string;
  partnerId: string;
  totalListProducts: any[] = [];
  listProducts: any[] = [];
  searchUpdate = new Subject<string>();

  thTable_services = [
    { name: 'Dịch vụ' },
    { name: 'Đơn giá' }
  ]

  constructor(
    private productOdataService: ProductsOdataService,
    private modalService: NgbModal,
    private fb: FormBuilder,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {

    this.searchInit();
    this.loadDataDefault();
  }

  searchInit() {
    fromEvent(document, 'keyup').pipe(
      filter((s: any) => s.keyCode === 113)
    ).subscribe(s => {     
      this.searchInput.nativeElement.focus();
    });

    this.searchUpdate
      .pipe(distinctUntilChanged())
      .subscribe((value) => {
        this.onSearch(value);
      });
  }

  onKeyUp(s) {
    if (s.key === 'Enter' || s.keyCode === 13) {
      if (this.listProducts) { this.addServiceToSaleOrder(this.listProducts[0]); }
    }
  }

  loadDataDefault() {
    // var val = new ProductPaged();
    // val.limit = this.limit;
    // val.offset = this.skip;
    // val.search = this.search || "";
    // val.type2 = "service";

    const state = {
      filter: {
        logic: 'and',
        filters: [
          { field: 'Type2', operator: 'eq', value: 'service' },
          { field: 'Active', operator: 'eq', value: true }
        ]
      }
    };

    const options = {
      select: 'Id,Name,NameNoSign,DefaultCode,ListPrice',
      orderby: 'DateCreated desc'
    };

    this.productOdataService
      .getFetch(state, options).subscribe(
        (res: any) => {
          this.listProducts = res.data;
          this.totalListProducts = res.data.map(x => ({
            ...x, searchString: x.Name.toLowerCase() + ' ' + x.NameNoSign.toLowerCase()
              + ' ' + x.DefaultCode.toLowerCase()
          }));

        },
        (err) => {
          console.log(err);
        }
      );
  }

  onSearch(val) {
    val = val.trim().toLowerCase();
    if (val === '') {
      this.listProducts = this.totalListProducts;
      return;
    }
    this.listProducts = this.totalListProducts.filter(x => x.searchString.includes(val));
    return;
  }

  addServiceToSaleOrder(item) {
    var value = {
      AmountPaid: 0,
      AmountResidual: 0,
      Diagnostic: '',
      Discount: 0,
      DiscountFixed: 0,
      DiscountType: 'percentage',
      Employee: null,
      EmployeeId: '',
      Assistant: null,
      AssistantId: '',
      Name: item.Name,
      PriceSubTotal: 0,
      PriceUnit: item.ListPrice,
      ProductId: item.Id,
      Product: {
        Id: item.Id,
        Name: item.Name
      },
      ProductUOMQty: 1,
      State: 'draft',
      Teeth: this.fb.array([]),
      ToothCategory: null,
      ToothCategoryId: '',
    };
    this.newEventEmiter.emit(value);
  }
  
  notify(type, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: type, icon: true }
    });
  }

  createProductService() {
    let modalRef = this.modalService.open(ProductServiceCuDialogComponent, {
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm: Dịch vụ";
    modalRef.result.then(
      () => {
        this.notify('success','tạo dịch vụ thành công');
        this.loadDataDefault();
      },
      () => { }
    );
  }

}
