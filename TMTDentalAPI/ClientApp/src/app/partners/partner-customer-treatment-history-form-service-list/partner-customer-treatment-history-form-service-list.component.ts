import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductBasic2, ProductPaged, ProductService } from 'src/app/products/product.service';
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
    private productService: ProductService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadDataDefault();
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

  addServiceToSaleOrder(item) {
    let modalRef = this.modalService.open(PartnerCustomerTreatmentHistoryFormAddServiceDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm dịch vụ điều trị';
    modalRef.componentInstance.partnerId = this.partnerId;
    modalRef.componentInstance.productService = item;
    modalRef.result.then(result => {
      console.log(result);
      if(result){
        this.newEventEmiter.emit(result);
      }
    }, () => {
    });
  }
}