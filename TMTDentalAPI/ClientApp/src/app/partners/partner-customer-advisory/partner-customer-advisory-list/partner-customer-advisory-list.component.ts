import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';
import { PartnerCustomerAdvisoryCuDialogComponent } from '../partner-customer-advisory-cu-dialog/partner-customer-advisory-cu-dialog.component';

@Component({
  selector: 'app-partner-customer-advisory-list',
  templateUrl: './partner-customer-advisory-list.component.html',
  styleUrls: ['./partner-customer-advisory-list.component.css']
})
export class PartnerCustomerAdvisoryListComponent implements OnInit {

  searchUpdate = new Subject<string>();
  search: string;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  dateFrom: Date;
  dateTo: Date;
  constructor(
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
  }
  createAdvisory(){
    const modalRef = this.modalService.open(PartnerCustomerAdvisoryCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm thông tin tư vấn';
    modalRef.result.then(() => {
    }, er => { })
  }
}
