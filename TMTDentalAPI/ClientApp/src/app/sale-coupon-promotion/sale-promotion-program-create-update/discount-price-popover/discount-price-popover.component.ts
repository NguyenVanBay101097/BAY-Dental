import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { HistoryPromotionRequest, SaleOrderPromotionService } from 'src/app/sale-orders/sale-order-promotion.service';
import { SaleCouponProgramService } from '../../sale-coupon-program.service';

@Component({
  selector: 'app-discount-price-popover',
  templateUrl: './discount-price-popover.component.html',
  styleUrls: ['./discount-price-popover.component.css']
})
export class DiscountPricePopoverComponent implements OnInit {

  id: string;
  title: string;
  typeApply: string;
  search: string ='';
  searchUpdate = new Subject<string>();
  gridView: GridDataResult;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  dateFrom: Date;
  dateTo: Date;
  skip = 0;
  pageSize = 20;
  amountTotal: number;
  constructor(public activeModal: NgbActiveModal,private saleOrderPromotionService: SaleOrderPromotionService,private intlService: IntlService,
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadGridData();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.skip = 0;
        this.loadGridData();
      });
  }


  onCancel(){
    this.activeModal.dismiss();
  }

  searchChangeDate(value) {
    console.log(value);
    
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.skip = 0;
    this.loadGridData();
  }

  loadGridData(){
    var val = new HistoryPromotionRequest();
    val.dateFrom = this.dateFrom ? this.intlService.formatDate(this.dateFrom, 'yyyy-MM-dd') : '';
    val.dateTo = this.dateTo ? this.intlService.formatDate(this.dateTo, 'yyyy-MM-dd') : '';
    val.offSet = this.skip;
    val.searchOrder = this.search || '';
    val.saleCouponProgramId = this.id;
    this.saleOrderPromotionService.GetHistoryPromotionByCouponProgram(val).pipe(
      map(rs1 => (<GridDataResult>{
        data: rs1.items,
        total: rs1.totalItems
      }))
    ).subscribe(rs2=>{
      this.gridView = rs2;
    }, er => {
      console.log(er);
    })
  }

  onExportExcel(){
    var val = new HistoryPromotionRequest();
    val.limit = 0;
    this.saleOrderPromotionService.ExportExcelFile(val).subscribe((rs) => {
      let newBlob = new Blob([rs], {
        type:
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      let data = window.URL.createObjectURL(newBlob);
      let link = document.createElement("a");
      link.href = data;
      link.click();
      setTimeout(() => {
        // For Firefox it is necessary to delay revoking the ObjectURL
        window.URL.revokeObjectURL(data);
      }, 100);
    });
  }
  
  pageChange(event: PageChangeEvent){
    this.skip = event.skip;
    this.loadGridData();
  }
}
