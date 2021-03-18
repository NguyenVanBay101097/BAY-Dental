import { Component, Input, OnInit, Type } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { forkJoin, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PartnerAdvanceCreateUpdateDialogComponent } from '../partner-advance-create-update-dialog/partner-advance-create-update-dialog.component';
import { PartnerAdvancePaged, PartnerAdvanceService, PartnerAdvanceSummaryFilter } from '../partner-advance.service';

@Component({
  selector: 'app-partner-advance-list',
  templateUrl: './partner-advance-list.component.html',
  styleUrls: ['./partner-advance-list.component.css']
})
export class PartnerAdvanceListComponent implements OnInit {
  partnerId: string;
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  
  search: string;
  limit = 20;
  offset = 0;
  edit = false;
  dateFrom: Date;
  dateTo: Date;
  loading = false;
  amountBalance: number;

  typeAmount: any = {};
  types: any[] = [
    { value: 'advance', text: 'Tạm ứng đã đóng' },
    { value: 'refund', text: 'Tạm ứng đã hoàn' },
    { value: '', text: 'Tạm ứng còn lại' },
  ]

  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());

  constructor(
    private intlService: IntlService,
    private modalService: NgbModal,
    private router: Router,
    private route: ActivatedRoute,
    private notificationService: NotificationService,
    private partnerAdvanceService: PartnerAdvanceService,
    private fb: FormBuilder,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.partnerId = this.route.parent.snapshot.paramMap.get('id');
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.offset = 0;
        this.loadDataFromApi();
      });


    this.loadDataFromApi();
    this.loadTypeAmountTotal();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new PartnerAdvancePaged();
    paged.limit = this.limit;
    paged.offset = this.offset;
    paged.search = this.search ? this.search : '';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd");
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-dd");
    this.partnerAdvanceService.getPaged(paged).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      console.log(res);

      this.loading = false;
    }, err => {
      this.loading = false;
    })
  }

  public pageChange(event: PageChangeEvent): void {
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  loadTypeAmountTotal() {
    forkJoin(this.types.map(x => {
      var val = new PartnerAdvanceSummaryFilter();
      val.type = x.value;
      val.dateFrom = this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd');
      val.dateTo = this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd');
      return this.partnerAdvanceService.getSumary(val).pipe(
        switchMap(amount => of({ type: x.value, amount: amount }))
      );
    })).subscribe((result) => {
      result.forEach(item => {
        if(item.type == ''){
          debugger
          this.amountBalance = item.amount as number;
        }       
        this.typeAmount[item.type] = item.amount;
      });
    });
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.offset = 0;
    this.loadDataFromApi();
    this.loadTypeAmountTotal();
  }

  createAdvanceModal() {
    const modalRef = this.modalService.open(PartnerAdvanceCreateUpdateDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Đóng tạm ứng';
    modalRef.componentInstance.type = 'advance';
    modalRef.componentInstance.partnerId = this.partnerId;
      modalRef.result.then(res => {
        this.loadDataFromApi();
        this.loadTypeAmountTotal();
      }, () => {
      });
  }

  createRefundModal() {
    const modalRef = this.modalService.open(PartnerAdvanceCreateUpdateDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Hoàn tạm ứng';
    modalRef.componentInstance.type = 'refund';
    modalRef.componentInstance.partnerId = this.partnerId;
      modalRef.result.then(res => {
        this.loadDataFromApi();
        this.loadTypeAmountTotal();
      }, () => {
      });
  }

  editItem(item) {
    const modalRef = this.modalService.open(PartnerAdvanceCreateUpdateDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = item.type == 'advance' ? 'Đóng tạm ứng' : 'Hoàn tạm ứng';
    modalRef.componentInstance.type = item.type;
    modalRef.componentInstance.partnerId = this.partnerId;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then((res) => {
      this.loadDataFromApi();
      this.loadTypeAmountTotal();
      // if (res && res.print) {
      //   this.printPhieu(item.id);
      // }
    }, (err) => { });
  }


  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa ' + (item.type = 'advance' ? 'đóng tạm ứng' : 'hoàn tạm ứng');
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn xóa' + (item.type = 'advance' ? 'đóng tạm ứng ?' : 'hoàn tạm ứng ?');
    modalRef.result.then(() => {
      this.partnerAdvanceService.delete(item.id).subscribe(() => {
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
        this.loadTypeAmountTotal();
      }, () => {
      });
    }, () => {
    });
  }


  getType(type) {
    switch (type) {
      case 'advance':
        return 'Đóng tạm ứng';
      case 'refund':
        return 'Hoàn tạm ứng';
    }
  }

  getState(state) {
    switch (state) {
      case 'draft':
        return 'Nháp';
      case 'confirmed':
        return 'Đã xác nhận';
    }
  }

}
