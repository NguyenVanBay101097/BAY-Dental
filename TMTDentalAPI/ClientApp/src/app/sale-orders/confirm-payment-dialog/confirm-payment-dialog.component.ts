import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirm-payment-dialog',
  templateUrl: './confirm-payment-dialog.component.html',
  styleUrls: ['./confirm-payment-dialog.component.css']
})
export class ConfirmPaymentDialogComponent implements OnInit {
  title: string;
  amountPayment: number = 0;
  name: string = '';
  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit(): void {
  }

  onPrint(){
    this.activeModal.close(true);
  }

}
