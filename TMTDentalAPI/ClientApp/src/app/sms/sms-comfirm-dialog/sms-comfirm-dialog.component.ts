import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-sms-comfirm-dialog',
  templateUrl: './sms-comfirm-dialog.component.html',
  styleUrls: ['./sms-comfirm-dialog.component.css']
})
export class SmsComfirmDialogComponent implements OnInit {
  title: string;
  campaign: any;
  brandName: string;
  body: string;
  timeSendSms: string;
  numberSms: number;
  bodyContent: string;
  bodyNote: string;
  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {    
  }

  onConfirm() {
    this.activeModal.close(true);
  }

  onCancel() {
    this.activeModal.dismiss();
  }

}
