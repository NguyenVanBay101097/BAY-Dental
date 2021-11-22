import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AccountJournalFilter, AccountJournalSave, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-sale-order-payment-advanced-dialog',
  templateUrl: './sale-order-payment-advanced-dialog.component.html',
  styleUrls: ['./sale-order-payment-advanced-dialog.component.css']
})
export class SaleOrderPaymentAdvancedDialogComponent implements OnInit {
  title: string;
  paymentForm: FormGroup;
  amountResidual: number;
  amountPayment: number;
  amountTotalJournalPayment: number = 0;
  debtAmountTotal: number = 0;
  filteredJournals: any[] = [
    {type:'cash',name: 'Tiền mặt', journals: []},
    {type:'bank',name: 'Ngân hàng', journals: []},
    {type:'debt',name: 'Ghi công nợ', journals: []},
    {type:'advance',name: 'Tạm ứng', journals: []}
  ];
  debtPaymentJournals: any = [
    {type:'cash',name: 'Tiền mặt'},
    {type:'bank',name: 'Ngân hàng'},
  ];
  showDebtPaymentJournal = false;
  debtTypeSelected = {type:'cash'};
  debtJournalSelected: string;
  selectedJournals = [];
  constructor(
    public activeModal: NgbActiveModal,
    private authService: AuthService,
    private accountJournalService: AccountJournalService
  ) { }

  ngOnInit(): void {
    this.getJounals();
  }

  getJounals() {
    var val = new AccountJournalFilter();
    val.companyId = this.authService.userInfo.companyId;
    val.type = "bank,cash,advance,debt";
    this.accountJournalService.journalResBankAutoComplete(val).subscribe((result: any[]) => {
      result.forEach((jn: any) => {
        var filterJN = this.filteredJournals.find(x=> x.type == jn.type);
        if(filterJN)
        {
          filterJN.journals.push(jn);
        }
      });
      console.log(this.filteredJournals);
    })
  };

  isDisabled(journalType) {
    var types = this.selectedJournals.map(x => x.type);
    return types.includes(journalType);
  }

  selectJournal(journal) {
    var types = this.selectedJournals.map(x => x.type);
    if (types.findIndex(x => x == journal.type) != -1) {
      return;
    }
    journal.amount = this.amountPayment;
    this.selectedJournals.push(journal);
    this.amountTotalJournalPayment = this.amountTotalJournalPayment + this.amountPayment;
    this.amountPayment = this.amountResidual - this.amountTotalJournalPayment;
  }

  onSelectDebtTypeJounals(journal) {
    if (journal.type == 'bank'){
      let journals = this.filteredJournals['bank'].journals;
      if (journals.length > 0)
        this.showDebtPaymentJournal = true;
    }
  }

  onSelectDebtJounals(journal) {
    this.debtJournalSelected = journal;
  }

}
