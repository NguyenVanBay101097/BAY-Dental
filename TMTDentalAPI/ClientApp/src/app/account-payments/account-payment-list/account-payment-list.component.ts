import { Component, ViewChild, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AccountPaymentService, AccountPaymentPaged, AccountPaymentBasic } from '../account-payment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';


@Component({
  selector: 'app-account-payment-list',
  templateUrl: './account-payment-list.component.html',
  styleUrls: ['./account-payment-list.component.css']
})
export class AccountPaymentListComponent implements OnInit {

  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  search: string;
  searchUpdate = new Subject<string>();
  partnerType: string;

  selectedIds: string[] = [];

  constructor(private paymentService: AccountPaymentService, private route: ActivatedRoute, private modalService: NgbModal,
    private router: Router) {
  }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.partnerType = params.get('partner_type');
      this.loadDataFromApi();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new AccountPaymentPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    if (this.search) {
      val.search = this.search;
    }
    if (this.partnerType) {
      val.partnerType = this.partnerType;
    }

    this.paymentService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  getTitle() {
    if (this.partnerType === 'customer') {
      return 'Thanh toán điều trị';
    } else {
      return 'Thanh toán mua hàng'
    }
  }

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  showState(state) {
    switch (state) {
      case 'posted':
        return 'Đã vào sổ';
      case 'reconciled':
        return 'Đã đối soát';
      default:
        return 'Nháp';
    }
  }

  unlinkSelected() {
    if (this.selectedIds.length == 0) {
      return false;
    }

    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Xóa thanh toán';
    modalRef.componentInstance.body = 'Bạn chắc chắn muốn xóa?';
    modalRef.result.then(() => {
      this.paymentService.unlink(this.selectedIds).subscribe(() => {
        this.selectedIds = [];
        this.loadDataFromApi();
      });
    });
  }

  cancelSelected() {
    if (this.selectedIds.length) {
      this.paymentService.actionCancel(this.selectedIds).subscribe(() => {
        this.loadDataFromApi();
      });
    }
  }

  // createItem() {
  //   let modalRef = this.modalService.open(ProductCategoryDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
  //   modalRef.componentInstance.title = 'Thêm nhóm sản phẩm';

  //   modalRef.result.then(result => {
  //     this.loadDataFromApi();
  //   }, () => {
  //   });
  // }

  editItem(item: AccountPaymentBasic) {
    this.router.navigate(['/accountpayments/edit/' + item.id]);
  }

  // deleteItem(item) {
  //   const dialog: DialogRef = this.dialogService.open({
  //     title: "Xóa nhóm sản phẩm",
  //     content: 'Bạn có chắc chắn muốn xóa?',
  //     actions: [
  //       { text: 'Hủy bỏ', value: false },
  //       { text: 'Đồng ý', primary: true, value: true }
  //     ],
  //     width: 450,
  //     height: 200,
  //     minWidth: 250
  //   });

  //   dialog.result.subscribe((result) => {
  //     if (result instanceof DialogCloseResult) {
  //       console.log('close');
  //     } else {
  //       console.log('action', result);
  //       if (result['value']) {
  //         this.productCategoryService.delete(item.id).subscribe(() => {
  //           this.loadDataFromApi();
  //         }, err => {
  //           console.log(err);
  //         });
  //       }
  //     }
  //   });
  // }
}

