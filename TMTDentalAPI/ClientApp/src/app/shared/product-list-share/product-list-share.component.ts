import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { fromEvent, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { ProductServiceCuDialogComponent } from 'src/app/products/product-service-cu-dialog/product-service-cu-dialog.component';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ToothDisplay } from 'src/app/teeth/tooth.service';
import { ProductsOdataService } from '../services/ProductsOdata.service';

@Component({
  selector: 'app-product-list-share',
  templateUrl: './product-list-share.component.html',
  styleUrls: ['./product-list-share.component.css']
})
export class ProductListShareComponent implements OnInit {


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
    private productService: ProductService,
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
      .pipe(
        debounceTime(400),
        distinctUntilChanged())
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
    var val = new ProductPaged();
    val.limit = 0;
    val.offset = 0;
    val.search = this.search ? this.search : '';
    val.type2 = 'service';
    this.productService.getPaged(val).subscribe(res => {
      this.listProducts = res.items;
    })
  }

  onSearch(val) {
    this.loadDataDefault();
  }

  addServiceToSaleOrder(item) {
    var value = {
      name: item.name,
      subPrice: item.listPrice,
      productId: item.id,
      qty: 1,
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
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm: Dịch vụ";
    modalRef.result.then(
      () => {
        this.notify('success', 'tạo dịch vụ thành công');
        this.loadDataDefault();
      },
      () => { }
    );
  }


}
