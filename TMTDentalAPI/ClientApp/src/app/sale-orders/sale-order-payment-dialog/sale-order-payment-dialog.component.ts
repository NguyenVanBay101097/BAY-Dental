import { Component, OnInit, ViewChild } from "@angular/core";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import {
  AccountJournalSimple,
  AccountJournalService,
  AccountJournalFilter,
} from "src/app/account-journals/account-journal.service";
import { AccountRegisterPaymentDisplay } from "src/app/account-payments/account-register-payment.service";
import { FormGroup, FormBuilder, Validators, FormArray } from "@angular/forms";
import { AccountPaymentService } from "src/app/account-payments/account-payment.service";
import { IntlService } from "@progress/kendo-angular-intl";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { NotificationService } from "@progress/kendo-angular-notification";
import { AppSharedShowErrorService } from "src/app/shared/shared-show-error.service";
import { AuthService } from "src/app/auth/auth.service";
import { debounceTime, tap, switchMap } from "rxjs/operators";
import { AccountPaymentsOdataService } from "src/app/shared/services/account-payments-odata.service";
import { SaleOrderPaymentService } from "src/app/core/services/sale-order-payment.service";
import { validator } from "fast-json-patch";

@Component({
  selector: "app-sale-order-payment-dialog",
  templateUrl: "./sale-order-payment-dialog.component.html",
  styleUrls: ["./sale-order-payment-dialog.component.css"],
})
export class SaleOrderPaymentDialogComponent implements OnInit {
  paymentForm: FormGroup;
  defaultVal: any;
  filteredJournals: AccountJournalSimple[];
  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;
  loading = false;
  title: string;
  maxAmount: number = 0;
  submitted: boolean = false;
  showPrint = false;
  showError_1 = false;
  showError_2 = false;
  step: number = 1;
  advanceAmount: number = 0;
  cashSuggestions: number[] = [];
  partnerCash = 0;

  constructor(
    private fb: FormBuilder,
    private intlService: IntlService,
    public activeModal: NgbActiveModal,
    private notificationService: NotificationService,
    private accountJournalService: AccountJournalService,
    private errorService: AppSharedShowErrorService,
    private authService: AuthService,
    private accountPaymenetOdataService: AccountPaymentsOdataService,
    private saleOrderPaymentService: SaleOrderPaymentService
  ) {}

  ngOnInit() {
    this.paymentForm = this.fb.group({
      amount: 0,
      date: new Date(),
      orderId: [null, Validators.required],
      companyId: [null, Validators.required],
      journalLines: this.fb.array([]),
      lines: this.fb.array([]),
      note: "",
      state: "draft",
    });

    setTimeout(() => {
      if (this.defaultVal) {
        this.defaultVal.date = new Date(this.defaultVal.date);
        this.paymentForm.patchValue(this.defaultVal);
        this.maxAmount = this.getValueForm("amount");
        this.paymentForm.get("amount").setValue(0);

        this.linesFC.clear();
        this.defaultVal.lines.forEach((line) => {
          var g = this.fb.group(line);
          this.linesFC.push(g);
        });

        this.journalLinesFC.clear();
        this.defaultVal.journalLines.forEach((line) => {
          var g = this.fb.group(line);
          this.journalLinesFC.push(g);
        });

        this.paymentForm.markAsPristine();
      }

      this.loadFilteredJournals();
      // this.journalCbx.filterChange.asObservable().pipe(
      //   debounceTime(300),
      //   tap(() => (this.journalCbx.loading = true)),
      //   switchMap(value => this.searchJournals(value))
      // ).subscribe(result => {
      //   this.filteredJournals = result;
      //   this.journalCbx.loading = false;
      // });
    });
  }

  get f() {
    return this.paymentForm.controls;
  }

  get journalLinesFC() {
    return this.f.journalLines as FormArray;
  }

  get linesFC() {
    return this.f.lines as FormArray;
  }

  get cashLineFG(){
    return this.journalLinesFC.controls.find(x=> x.value.journal.type == 'cash');
  }

  loadFilteredJournals() {
    this.searchJournals().subscribe((result) => {
      this.filteredJournals = result;
      if (this.filteredJournals && this.filteredJournals.length > 1) {
        this.journalLinesFC.clear();
        // add default tiền mặt
        var cashJournal = this.filteredJournals.find((x) => x.type == "cash");
        var g = this.fb.group({
          journalId: cashJournal.id,
          journal: cashJournal,
          amount: this.maxAmount,
        });
        this.journalLinesFC.push(g);
        this.addCashSuggest();
      }
    });
  }

  getValueForm(key) {
    return this.paymentForm.get(key).value;
  }

  searchJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = "bank,cash,advance";
    val.search = search || "";
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }

  save() {
    this.submitted = true;
    if (!this.paymentForm.valid) {
      return;
    }

    var val = this.getValueFormSave();
    if (val == null) return;

    this.saleOrderPaymentService.create(val).subscribe(
      (result: any) => {
        this.saleOrderPaymentService.actionPayment([result.id]).subscribe(
          () => {
            this.activeModal.close(true);
          },
          (err) => {
            this.errorService.show(err);
          }
        );
      },
      (err) => {
        this.errorService.show(err);
      }
    );
  }

  saveAndPrint() {
    this.submitted = true;
    if (!this.paymentForm.valid) {
      return;
    }

    var val = this.getValueFormSave();
    if (val == null) return;

    this.saleOrderPaymentService.create(val).subscribe(
      (result: any) => {
        this.saleOrderPaymentService.actionPayment([result.id]).subscribe(
          () => {
            this.activeModal.close({
              print: true,
              paymentId: result.id,
            });
          },
          (err) => {
            this.errorService.show(err);
          }
        );
      },
      (err) => {
        this.errorService.show(err);
      }
    );
  }

  getValueFormSave() {
    var val = this.paymentForm.value;
    val.date = this.intlService.formatDate(
      val.date,
      "d",
      "en-US"
    );
    return val;
  }

  cancel() {
    this.activeModal.dismiss();
  }

  changeMoneyLine(line: FormGroup) {
    var sumAmountPrepaid = 0;
    this.getValueForm("lines").forEach(function (v) {
      sumAmountPrepaid += v.amount;
    });
    this.paymentForm.get("amount").setValue(sumAmountPrepaid);
  }

  payOff() {
    var total = 0;
    this.linesFC.controls.forEach((control) => {
      var residual = control.get("saleOrderLine").value.amountResidual || 0;
      control.get("amount").setValue(residual);

      total += residual;
    });

    this.paymentForm.get("amount").setValue(total);
  }

  enterMoney() {
    var amount = this.paymentForm.get("amount").value || 0;

    var lines = this.paymentForm.get("lines") as FormArray;
    //tìm những line có số tiền âm sẽ cộng thêm vào amount;
    var negativeLineControls = lines.controls.filter((value, index, array) => {
      return value.get("saleOrderLine").value.amountResidual < 0;
    });

    negativeLineControls.forEach((control) => {
      var amountResidual =
        control.get("saleOrderLine").value.amountResidual || 0;
      control.get("amount").setValue(amountResidual);
      amount += amountResidual * -1;
    });

    lines.controls.forEach((control) => {
      var amountResidual =
        control.get("saleOrderLine").value.amountResidual || 0;
      if (amountResidual > 0) {
        var amountPaid = Math.min(amount, amountResidual);
        control.get("amount").setValue(amountPaid);

        amount -= amountPaid;
      }
    });
  }

  getReturnOverPaid() {
    var amount = this.getValueForm("amount");
    return this.partnerCash - amount > 0 ? this.partnerCash - amount : 0;
  }

  nextStep() {
    this.step++;
    //gán lại default
    this.partnerCash = this.getValueForm("amount");
    this.journalLinesFC.controls[0].get("amount").setValue(this.partnerCash);
    this.addCashSuggest();
  }

  backPreviousStep() {
    this.step--;
  }

  addCashSuggest() {
    this.cashSuggestions = [];
    var init = this.cashLineFG.value.amount;
    // if (init == 0) {
    //   this.cashSuggestions.push(init);
    //   return;
    // }

    // var plushArr = [0, 30000, 50000, 500000, 700000];
    // for (var i = 0; i < 5; i++) {
    //   this.cashSuggestions.push(init + plushArr[i]);
    // }

    this.cashSuggestions = this.getCalcAmountSuggestions(init);
  }

  addJournalLine(journal: AccountJournalSimple) {
    var index = this.journalLinesFC.value.findIndex(
      (x) => x.journalId == journal.id
    );
    if (index == -1) {
      var g = this.fb.group({
        journalId: journal.id,
        journal: journal,
        amount: 0,
      });
      this.journalLinesFC.push(g);
    }
  }

  getCalcAmountSuggestions(amount: number) {
    var coins = [500, 1e3, 2e3, 5e3, 1e4, 2e4, 5e4, 1e5, 2e5, 5e5];

    function greedy(amount, max) {
      for (var temp = amount, result = []; temp > 0; ) {
        temp -= max;
        result.push(max);
      }
      return result.reduce(function (a, b) {
        return a + b;
      }, 0);
    }
    //Tính suggest
    var results = [],
      preNumber = 0;
    amount >= 1e7 && ((preNumber = Math.floor(amount / 1e7)), (amount %= 1e7));

    for (var i = 0; i < coins.length; i++) {
      var coin = coins[i],
        sum = greedy(amount, coin);
      //Trường hợp lớp hơn 1e7
      if (preNumber > 0) sum = 1e7 * preNumber + sum;
      ///Không có thì thêm vào
      if (results.indexOf(sum) === -1) results.push(sum);
      //trường hợp số phần lẻ
    }

    //Phần lẻ
    var amountStr = amount.toString();
    if (amountStr.length > 4) {
      var le = amountStr.substring(amountStr.length - 4, amountStr.length);
      var amountLe = Number.parseInt(le);
      results.forEach((element) => {
        if (element % 10000 == 0) {
          var rs = element + amountLe;
          if (results.indexOf(rs) === -1) results.push(rs);
        }
      });
    }
    return results.sort(function (a, b) {
      return a > b ? 1 : a < b ? -1 : 0;
    });
  }

  onCashSuggestionClick(amount: number){
    this.partnerCash = amount;
  }

  isJournalActiveClass(item: AccountJournalSimple) {
    return this.journalLinesFC.value.findIndex(x=> x.journalId == item.id) != -1;
  }

  onDeleteJournalLine(index) {
    this.journalLinesFC.removeAt(index);
  }

  onJournalLineAmountChange(val = null, journalLine = null){
    if(!val && journalLine){
      this.journalLinesFC.controls.find(x=> x.value.journalId == journalLine.journalId).get('amount').setValue(0);
    }

    var cashLineFG = this.cashLineFG;
    var otherLineAmount = this.journalLinesFC.value.reduce((total, cur)=>{ return total+ cur.amount}, 0) - cashLineFG.value.amount;
    
    cashLineFG.get('amount').setValue(this.maxAmount - otherLineAmount);
  }

  getJournalLineAmountMax(journalLine) {
    var cash = this.cashLineFG.value.amount;
    var otherLineAmount = this.journalLinesFC.value.reduce((total, cur)=>{ return total+ cur.amount}, 0) - cash - journalLine.amount;
    if(journalLine.journal.type != 'advance') {
      return this.maxAmount - otherLineAmount;
    } else {
      var advanceMax = this.maxAmount - otherLineAmount > this.advanceAmount ? this.advanceAmount : this.maxAmount - otherLineAmount;
      return  advanceMax > 0 ? advanceMax : 0;
    }
  }

}
