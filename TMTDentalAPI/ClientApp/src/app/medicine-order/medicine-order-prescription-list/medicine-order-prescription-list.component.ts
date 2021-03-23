import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { ToaThuocPaged, ToaThuocService } from 'src/app/toa-thuocs/toa-thuoc.service';
import { MedicineOrderCreateDialogComponent } from '../medicine-order-create-dialog/medicine-order-create-dialog.component';

@Component({
  selector: 'app-medicine-order-prescription-list',
  templateUrl: './medicine-order-prescription-list.component.html',
  styleUrls: ['./medicine-order-prescription-list.component.css']
})
export class MedicineOrderPrescriptionListComponent implements OnInit {
  gridData: GridDataResult;
  searchUpdate = new Subject<string>();
  search: string;
  loading = false;
  limit = 20;
  dateFrom: Date;
  dateTo: Date;
  skip = 0;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  constructor(
    private toathuocSevice: ToaThuocService,
    private intlService: IntlService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var paged = new ToaThuocPaged();
    paged.limit = this.limit;
    paged.offset = this.skip;
    paged.search = this.search ? this.search : '';
    paged.dateFrom = this.intlService.formatDate(this.dateFrom, "yyyy-MM-dd")
    paged.dateTo = this.intlService.formatDate(this.dateTo, "yyyy-MM-ddT23:59")
    this.toathuocSevice.getPaged(paged).pipe(
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

  pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadDataFromApi();
  }


  createPrescriptionPayment(item) {
    const modalRef = this.modalService.open(MedicineOrderCreateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thanh toán hóa đơn thuốc';
    modalRef.componentInstance.idToaThuoc = item.id;
    modalRef.result.then(res => {
      this.loadDataFromApi();
    }, () => {
    });
  }

  onSearchDateChange(data) {
    this.dateFrom = data.dateFrom;
    this.dateTo = data.dateTo;
    this.skip = 0;
    this.loadDataFromApi();
  }


}
