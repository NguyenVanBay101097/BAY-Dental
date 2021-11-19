import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountJournalSimple } from 'src/app/account-journals/account-journal.service';

@Component({
  selector: 'app-sale-order-payment-advanced-dialog',
  templateUrl: './sale-order-payment-advanced-dialog.component.html',
  styleUrls: ['./sale-order-payment-advanced-dialog.component.css']
})
export class SaleOrderPaymentAdvancedDialogComponent implements OnInit {
  title: string;
  paymentForm: FormGroup;
  filteredJournals: AccountJournalSimple[] = [
    {id:'',type:'',name: 'Tiền mặt'},
    {id:'',type:'',name: 'Ngân hàng'},
    {id:'',type:'',name: 'Ghi công nợ'},
    {id:'',type:'',name: 'Tạm ứng'}

  ];

  constructor(
    public activeModal: NgbActiveModal,

  ) { }

  ngOnInit(): void {
  }

}
