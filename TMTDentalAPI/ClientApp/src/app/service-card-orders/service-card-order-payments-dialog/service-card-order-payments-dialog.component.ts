import { formatNumber } from '@angular/common';
import { HostListener, ViewChild } from '@angular/core';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService, parseNumber } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { version } from 'process';
import { AccountJournalFilter, AccountJournalService, AccountJournalSimple } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AccountRegisterPaymentDisplay } from 'src/app/account-payments/account-register-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

export function formatCommaDecimalNumber(value: number, locale = 'en-US', digitsInfo = '.2-2'): string {
  // No need to instantiate Pipes at least since Angular 8
  let formatted = formatNumber(value, locale, digitsInfo);
  return formatted;
}

@Component({
  selector: 'app-service-card-order-payments-dialog',
  templateUrl: './service-card-order-payments-dialog.component.html',
  styleUrls: ['./service-card-order-payments-dialog.component.css']
})



export class ServiceCardOrderPaymentsDialogComponent implements OnInit {
  formGroup: FormGroup;
  defaultVal: any;
  filteredJournals: AccountJournalSimple[];
  JounrnalDedault: AccountJournalSimple;
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  loading = false;
  title: string;
  keyCode: string = '';
  rowClicked;

  constructor(private paymentService: AccountPaymentService, private fb: FormBuilder, private intlService: IntlService,
    public activeModal: NgbActiveModal, private notificationService: NotificationService, private accountJournalService: AccountJournalService,
    private errorService: AppSharedShowErrorService, private authService: AuthService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      cusPayments: this.fb.array([]),
    });

    if (this.defaultVal) {
      setTimeout(() => {
        this.loadFilteredJournals();
      });
    }
  }

  @HostListener('window:keydown', ['$event'])
  keyEvent(event: KeyboardEvent) {
    let charCode = (event.which) ? event.which : event.keyCode;
    if ((charCode >= 48 && charCode <= 57) || (charCode >= 96 && charCode <= 105)) {
      this.keyCode += event.key;
      this.setValueNumber(this.keyCode);
    } else if (charCode == 13) {
      this.setPayments();
    } else if (charCode == 8) {
      this.RemoveLength();
    } else if (charCode == 46) {
      console.log('Delete Key Pressed');
    } 

  }


  get cusPayments() {
    return this.formGroup.get('cusPayments') as FormArray;
  }

  loadRecord() {
    if (this.defaultVal) {

      var res = this.fb.group({
        amountResidual: this.defaultVal.amountTotal,
        amount: this.defaultVal.amountTotal,
        amountRefund: 0,
        paymentDate: this.intlService.formatDate(new Date(), 'd', 'en-US'),
        communication: null,
        journalId: this.getJournalDefault().id,
        journal: this.getJournalDefault(),
        isRefund: false
      });

      if (this.cusPayments.length == 0) {
        this.cusPayments.push(res);
        this.rowClicked = 0;
      }
    }
  }

  getJournalDefault() {
    var jounrnalDedault = this.filteredJournals.find(x => x.name == "Tiền mặt");
    return jounrnalDedault;
  }


  loadFilteredJournals() {
    this.searchJournals().subscribe(result => {
      this.filteredJournals = result;
      this.loadRecord();
    })
  }

 

  setValueNumber(key) {
    var formRes = this.cusPayments.controls[this.rowClicked];
    formRes.get('amount').setValue(this.price_to_number(this.addCommas(key)));
    formRes.patchValue(formRes.value);
    if(this.totalResidual < 0 && this.rowClicked != (this.cusPayments.length -1)){
      var formRes = this.cusPayments.controls[this.cusPayments.length -1];
      formRes.get('amountResidual').setValue(formRes.value.amountResidual - Math.abs(this.totalResidual));
      if(formRes.get('amountResidual').value < 0 ){
        formRes.get('amountResidual').setValue(0);
      }
      formRes.patchValue(formRes.value);
      this.computeRefund(this.cusPayments.length -1);
    }
    this.computeRefund(this.rowClicked);
    
  }

  //compute amount
  addCommas(nStr) {
    nStr += '';
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
      x1 = x1.replace(rgx, '$1' + '.' + '$2');
    }
    return x1 + x2;
  }

  computeRefund(index: number){
    var total = 0;
    var res = this.cusPayments.controls[index];
    if(res.value.amount > res.value.amountResidual){      
      total = res.value.amount - res.value.amountResidual;
    }
    res.value.amountRefund = total;
    res.patchValue(res.value);   
  }

  get totalResidual(){
    var total = this.defaultVal.amountTotal;
    if(this.cusPayments.length === 0){
      return total = 0;
    }

    if(this.defaultVal){
      this.cusPayments.controls.forEach(line => {
        total -= line.get('amount').value;
      });
    }

    return total;
  }

  price_to_number(v){
    if(!v){return 0;}
    v=v.split('.').join('');
    v=v.split(',').join('.');
    return Number(v.replace(/[^0-9.]/g, ""));
  }

  searchJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = search || '';
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }

  changeTableRowColor(index: number) {
      this.rowClicked = index;
      this.keyCode = '';
  }

  addPayment(val) {
    let total = 0;
    this.cusPayments.controls.forEach(line => {
      total += line.get('amount').value;
    });

    var res = this.fb.group({
      amountResidual: this.totalResidual === 0 ? this.defaultVal.amountTotal : this.totalResidual,
      amount: this.totalResidual,
      amountRefund: 0,
      paymentDate: this.intlService.formatDate(new Date(), 'd', 'en-US'),
      communication: null,
      journalId: val.id,
      journal: val,
      isRefund: false
    });

    if (!this.cusPayments.controls.some(x => x.value.journalId === res.value.journalId)) {
      if ((total - this.defaultVal.amountTotal) != 0 && total < this.defaultVal.amountTotal) {
        this.cusPayments.push(res);    
      }
    }else {
      var pay = this.cusPayments.controls.find(x=>x.value.journalId === res.value.journalId);
      if (pay) {
        if (this.totalResidual > 0) {
          pay.value.amount += this.totalResidual;
          pay.patchValue(pay.value);
        }
      }
    }
    this.keyCode = '';
    this.changeTableRowColor(this.cusPayments.controls.length - 1);
  }


  RemoveLength(){
    var pay = this.cusPayments.controls[this.rowClicked];
    var amount = this.addCommas(pay.value.amount);
    pay.get('amount').setValue(this.price_to_number(amount.substring(0, amount.length - 1)));
    pay.patchValue(pay.value);
    if((pay.get('amountResidual').value - pay.get('amount').value) > 0 && this.rowClicked != (this.cusPayments.length -1)){
      var formRes = this.cusPayments.controls[this.cusPayments.length -1];
      if(formRes.get('amountResidual').value >= 0){
        formRes.get('amountResidual').setValue(pay.get('amountResidual').value - pay.get('amount').value);
      }
      formRes.patchValue(formRes.value);
      this.computeRefund(this.cusPayments.length -1);
    }
    this.computeRefund(this.rowClicked);
  }


  deletePay(index: number) {
    this.keyCode = '';
    this.cusPayments.removeAt(index);
    this.changeTableRowColor(this.cusPayments.controls.length - 1);
    this.cusPayments.markAsDirty();
  }

  onChangePayment(pay: FormGroup) {
    var payment = this.cusPayments.controls.find(x => x.value.journalId === pay.value.journalId);
    if (payment) {
      payment.patchValue(pay.value);
    }
  }

  setPayments() {
       
    if (!this.formGroup.valid) {
      return;
    }

    var residual = 0; 
    this.cusPayments.controls.forEach(pay =>{
      residual += pay.get('amountRefund').value;
    })

    if(this.totalResidual > 0){
      return;
    }

    if(residual > 0){
      this.createPaymentResidual(residual)
    }


    this.activeModal.close(this.cusPayments);

  }


  createPaymentResidual(value: number){
    var res = this.fb.group({
      amountResidual: this.defaultVal.amountTotal,
      amount: -value,
      amountRefund: this.defaultVal.amountRefund,
      paymentDate: this.intlService.formatDate(new Date(), 'd', 'en-US'),
      communication: null,
      journalId: this.getJournalDefault().id,
      journal: this.getJournalDefault(),
      isRefund: true
    });
    this.cusPayments.push(res);
  }

  // save() {
  //   if (!this.paymentForm.valid) {
  //     return;
  //   }

  //   this.create().subscribe((result: any) => {
  //     this.paymentService.post([result.id]).subscribe(() => {
  //       this.activeModal.close(true);
  //     }, (err) => {
  //       this.errorService.show(err);
  //     });
  //   }, (err) => {
  //     this.errorService.show(err);
  //   });
  // }


  cancel() {
    this.activeModal.dismiss();
  }

}
