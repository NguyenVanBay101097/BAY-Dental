import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SmsCampaignCrUpComponent } from '../sms-campaign-cr-up/sms-campaign-cr-up.component';
import { SmsMessageDialogComponent } from '../sms-message-dialog/sms-message-dialog.component';

@Component({
  selector: 'app-sms-campaign-list',
  templateUrl: './sms-campaign-list.component.html',
  styleUrls: ['./sms-campaign-list.component.css']
})
export class SmsCampaignListComponent implements OnInit {

  constructor(private modalService: NgbModal) { }

  ngOnInit() {
  }

  loadDataFromApi() {

  }

  createCampaign() {
    const modalRef = this.modalService.open(SmsCampaignCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Thêm chiến dịch';
    modalRef.result.then((val) => {
      this.loadDataFromApi();
    })
  }

  createMessage() {
    const modalRef = this.modalService.open(SmsMessageDialogComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Tạo tin nhắn';
    modalRef.result.then((val) => {
      this.loadDataFromApi();
    })
  }
}
