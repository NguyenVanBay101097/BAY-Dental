import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SmsCampaignCrUpComponent } from '../sms-campaign-cr-up/sms-campaign-cr-up.component';

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

  createItem() {
    const modalRef = this.modalService.open(SmsCampaignCrUpComponent, { size: 'lg', windowClass: 'o_technical_modal' });
    modalRef.componentInstance.title = 'Thêm chiến dịch';
    modalRef.result.then((val) => {
      this.loadDataFromApi();
    })
  }
}
