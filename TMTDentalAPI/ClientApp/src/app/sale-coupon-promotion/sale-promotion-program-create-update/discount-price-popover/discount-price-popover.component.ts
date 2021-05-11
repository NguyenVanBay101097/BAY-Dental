import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { SaleCouponProgramService } from '../../sale-coupon-program.service';

@Component({
  selector: 'app-discount-price-popover',
  templateUrl: './discount-price-popover.component.html',
  styleUrls: ['./discount-price-popover.component.css']
})
export class DiscountPricePopoverComponent implements OnInit {

  id: string;
  title: string;
  search: string;
  searchUpdate = new Subject<string>();
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  dateFrom: Date;
  dateTo: Date;
  skip = 0;
  loading = false;
  constructor(public activeModal: NgbActiveModal,private saleCouponProgramService: SaleCouponProgramService) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
  }

  onExportExcel(){

  }

  onCancel(){

  }

  searchChangeDate(value) {
    this.dateFrom = value.dateFrom;
    this.dateTo = value.dateTo;
    this.skip = 0;
    this.loadGridData();
  }

  loadGridData(){
    this.saleCouponProgramService.getHistoryApplyPromotion(this.id).subscribe(result=>{
      console.log(result);
      
    })
  }

  exportExcelFile(){
    
  }

}
