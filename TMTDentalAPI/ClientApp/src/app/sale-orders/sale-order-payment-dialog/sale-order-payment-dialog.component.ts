import { Component, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, Validators, FormControl, AbstractControl } from "@angular/forms";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { IntlService } from "@progress/kendo-angular-intl";
import {
  AccountJournalFilter, AccountJournalService, AccountJournalSimple
} from "src/app/account-journals/account-journal.service";
import { AuthService } from "src/app/auth/auth.service";
import { CustomerDebtReportService } from "src/app/core/services/customer-debt-report.service";
import { PaymentService } from "src/app/core/services/payment.service";
import { SaleOrderPaymentService } from "src/app/core/services/sale-order-payment.service";
import { PartnerService } from "src/app/partners/partner.service";
import { PrintService } from "src/app/shared/services/print.service";
import { AppSharedShowErrorService } from "src/app/shared/shared-show-error.service";
import { ConfirmPaymentDialogComponent } from "../confirm-payment-dialog/confirm-payment-dialog.component";
import { NotifyService } from 'src/app/shared/services/notify.service';

@Component({
  selector: "app-sale-order-payment-dialog",
  templateUrl: "./sale-order-payment-dialog.component.html",
  styleUrls: ["./sale-order-payment-dialog.component.css"]
})
export class SaleOrderPaymentDialogComponent implements OnInit {
  paymentForm: FormGroup;
  defaultVal: any;// data from api saleorderpaymentbysaleorderid
  partnerDebt = 0; // số tiền công nợ
  userAmountPaymentMax = 0; // số tiền tối đa mà người dùng được nhập
  // advanced payment
  userAmountPayment = 0; // số tiền khách nhập
  filteredJournals: any[] = [];
  cashBankJournals: any[] = [];

  paymentMethods: any[] = [
    { type: 'cash', name: 'Tiền mặt' },
    { type: 'bank', name: 'Ngân hàng' },
    { type: 'debt', name: 'Ghi công nợ' },
    { type: 'advance', name: 'Tạm ứng' }
  ];

  advanceAmount: number = 0; // số tiền tạm ứng
  selectedJournals = []; // danh sách journal filter box được chọn

  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;
  loading = false;
  title: string;
  submitted: boolean = false;
  showPrint = false;
  showError_1 = false;
  showError_2 = false;
  step: number = 1;
  cashSuggestions: number[] = [];
  partnerCash = 0;
  returnCash = 0;
  maxAmount: number = 0;
  partnerId: string;
  partnerName: string;

  isPaymentForService = false;

  constructor(
    private fb: FormBuilder,
    private intlService: IntlService,
    public activeModal: NgbActiveModal,
    private accountJournalService: AccountJournalService,
    private errorService: AppSharedShowErrorService,
    private authService: AuthService,
    private saleOrderPaymentService: SaleOrderPaymentService,
    private partnerService: PartnerService,
    private customerDebtReportService: CustomerDebtReportService,
    private paymentService: PaymentService,
    private modalService: NgbModal,
    private printService: PrintService,
    private notifyService: NotifyService
  ) { }

  ngOnInit() {
    this.paymentForm = this.fb.group({
      amount: [0, [Validators.required, Validators.min(0)]],
      date: [new Date(), Validators.required],
      orderId: [null, Validators.required],
      companyId: [null, Validators.required],
      journalLines: this.fb.array([]),
      lines: this.fb.array([]),
      note: "",
      state: "draft",
      isDebtPayment: false,
      debtJournalId: null,
      debtJournal: null,
      debtAmount: 0,
      debtNote: null,
      partnerId: this.partnerId || '',
      isPaymentForService: false
    });

    setTimeout(() => {
      if (this.defaultVal) {
        this.defaultVal.date = new Date(this.defaultVal.date);
        this.maxAmount = this.defaultVal.amount;
        this.paymentForm.patchValue(this.defaultVal);
      }

      this.getAmountAdvanceBalance();
    });

    // advance payment
    this.getJounals();
  }

  get journalLines() {
    return this.paymentForm.get('journalLines') as FormArray;
  }

  get isPaymentForServiceValue() {
    return this.paymentForm.get('isPaymentForService').value;
  }

  get userAmountPaymentValue() {
    return Number(this.userAmountPayment);
  }

  getAmountAdvanceBalance() {
    this.partnerService.getAmountAdvanceBalance(this.partnerId).subscribe(result => {
      this.advanceAmount = result;
    })
  }

  changeDebtPayment(checked) {
    if (checked) {
      const validators = [Validators.required, Validators.min(0)];
      this.paymentFC.debtAmount.setValidators(validators);
      this.paymentFC.debtAmount.setValue(this.partnerDebt);

      this.paymentFC.debtJournal.setValidators([Validators.required]);
      this.paymentFC.debtJournal.setValue(this.filteredJournals.find(x => x.type == 'cash'));
    } else {
      this.paymentFC.debtAmount.clearValidators();
      this.paymentFC.debtJournal.clearValidators();
    }

    this.paymentFC.debtAmount.updateValueAndValidity();
    this.paymentFC.debtJournal.updateValueAndValidity();
  }

  getJounals() {
    var val = new AccountJournalFilter();
    val.companyId = this.authService.userInfo.companyId;
    val.type = "bank,cash,advance,debt";
    this.accountJournalService.autocomplete(val).subscribe((result: any[]) => {
      this.filteredJournals = result;
      this.cashBankJournals = result.filter(x => x.type == 'cash' || x.type == 'bank');
    })
  };

  get amountResidual() {
    return this.defaultVal.lines.reduce((total, line) => {
      return total + line?.saleOrderLine?.amountResidual;
    }, 0);
  }

  get amount() {
    return Number(this.paymentForm.get('amount').value);
  }

  get paymentFC() {
    return this.paymentForm.controls;
  }

  get journalLinesFC() {
    return this.paymentFC.journalLines as FormArray;
  }

  get linesFC() {
    return this.paymentFC.lines as FormArray;
  }

  get cashLineFG() {
    return this.journalLinesFC.controls.find(x => x.value.journal.type == 'cash');
  }

  get debtAmount() {
    return Number(this.paymentForm.get('debtAmount').value);
  }

  get isDebtPaymentValue() {
    return this.paymentForm.get('isDebtPayment').value;
  }

  getPaymentFormControl(key) {
    return this.paymentForm.get(key);
  }

  filteredJournalsByType(type) {
    return JSON.parse(JSON.stringify(this.filteredJournals.filter(x => type.includes(x.type))));
  }

  getAmountTotalPayment() {
    let amount = this.amount | 0;
    let debtAmount = this.debtAmount | 0;
    if (this.paymentFC.isDebtPayment.value)
      return amount + debtAmount;
    return amount;
  }

  getValueForm(key) {
    return this.paymentForm.get(key).value;
  }

  searchJournals(search?: string) {
    var val = new AccountJournalFilter();
    val.type = "bank,cash,advance,debt";
    val.search = search || "";
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }

  isThanhToanDu() {
    var paymentLines = this.journalLinesFC.value;
    var totalAmountPaymentLines = paymentLines.reduce((a, b) => {
      return a + b.amount;
    }, 0);

    return totalAmountPaymentLines == this.amount;
  }

  save() {
    this.submitted = true;
    if (!this.paymentForm.valid) {
      return;
    }

    if (this.amount <= 0) {
      this.notifyService.notify('error', 'Số tiền thanh toán phải lớn hơn 0');
      return false;
    }

    //nếu số tiền còn thiếu > 0 thì ko cho thanh toán
    if (this.step == 2 && this.amountTotalJournalPayment < this.amount) {
      this.notifyService.notify('error', 'Vui lòng chọn phương thức cho số tiền còn thiếu');
      return false;
    }

    var val = this.getValueFormSave();
    this.paymentService.paymentSaleOrderAndDebt(val).subscribe((result: any) => {
      this.activeModal.close(false);
      let modalRef = this.modalService.open(ConfirmPaymentDialogComponent, {
        size: 'sm',
        windowClass: "o_technical_modal",
        keyboard: false,
        backdrop: "static",
      });
      modalRef.componentInstance.title = "Thanh toán thành công";
      modalRef.componentInstance.name = this.partnerName || '';
      modalRef.componentInstance.amountPayment = this.paymentFC.isDebtPayment.value ? this.getAmountTotalPayment() : this.paymentForm.get('amount').value;
      modalRef.result.then(
        res => {
          this.saleOrderPaymentService.getPrint(result.id).subscribe((result: any) => {
            this.printService.printHtml(result.html);
          });
        },
        () => { }
      );
    }, (err) => {
    })
  }

  saveAndPrint() {
    this.submitted = true;
    if (!this.paymentForm.valid) {
      return;
    }


    var val = this.getValueFormSave();
    if (val == null) return;

    this.paymentService.paymentSaleOrderAndDebt(val).subscribe((result: any) => {
      this.activeModal.close({
        print: true,
        paymentId: result.id,
      });
    }, (err) => {
      this.errorService.show(err);
    })
  }

  getValueFormSave() {
    var val = this.paymentForm.getRawValue();
    val.date = this.intlService.formatDate(val.date, "d", "en-US");

    val.journalLines = val.journalLines.map(x => {
      return {
        journalId: x.journal.id,
        amount: x.amount
      }
    });

    val.debtJournalId = val.debtJournal ? val.debtJournal.id : null;

    return val;
  }

  cancel() {
    this.activeModal.dismiss();
  }

  changeMoneyLine() {
    var sumAmountPrepaid = 0;
    this.linesFC.controls.forEach(function (v) {
      sumAmountPrepaid += Number(v.get('amount').value);
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
    setTimeout(() => {
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
    }, 0);


  }

  updateReturnCash() {
    var returnAmount = this.partnerCash - this.getAmountCashLine();
    this.returnCash = returnAmount > 0 ? returnAmount : 0;
  }

  nextStep() {

    this.submitted = true;
    if (this.paymentForm.invalid) {
      return;
    }

    this.step++;
    //gán lại default
    this.userAmountPaymentMax = this.amount;

    setTimeout(() => {
      this.userAmountPayment = this.amount;
    }, 200);

    // this.debtJournalSelected = {type:'cash',name: 'Tiền mặt', journals: [], journal:null};

    this.selectedJournals = [];

    //Nếu chưa có dòng phương thức tiền mặt thì add, có thì update tiền mặt mặc định
    // var paymentAmount = this.getValueForm("amount");
    // var cashJournal = this.filteredJournals.find(x => x.type == 'cash');
    // var cashPaymentLine = this.journalLinesFC.controls.find(x => x.get('journalId').value == cashJournal.id);
    // if (cashPaymentLine) {
    //   cashPaymentLine.get('amount').setValue(paymentAmount);
    // } else {
    //   this.journalLinesFC.push(this.fb.group({
    //     journalId: cashJournal.id,
    //     journal: cashJournal,
    //     amount: paymentAmount,
    //   }));
    // }

    // this.partnerCash = paymentAmount;
    // this.updateReturnCash();
    // this.cashSuggestions = this.getCalcAmountSuggestions(paymentAmount);

    // this.partnerCash = this.getValueForm("amount");
    // this.journalLinesFC.controls[0].get("amount").setValue(this.partnerCash);
    //this.addCashSuggest();
  }

  backPreviousStep() {
    this.step--;
    this.userAmountPayment = 0;
    this.journalLines.clear();
    if (this.isDebtPaymentValue) {
      this.paymentFC.debtJournal.setValue(this.filteredJournals.find(x => x.type == 'cash'));
    }
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
    //nếu trường hợp là tạm ứng là load số tiền tạm ứng còn lại và input nhập không cho nhập vượt
    var index = this.journalLinesFC.value.findIndex(
      (x) => x.journalId == journal.id
    );
    if (index == -1) {
      if (journal.type == 'advance') {
        this.partnerService.getAmountAdvanceBalance(this.partnerId).subscribe(result => {
          var soTienConLai = this.amount - this.laySoTienThanhToanExceptCashAndIndex(index);
          var g = this.fb.group({
            journalId: journal.id,
            journal: journal,
            amount: [soTienConLai, [Validators.required, Validators.min(0)]],
            amountAdvanceBalance: result
          });
          this.journalLinesFC.push(g);
          this.onChangeJournalLineAmount(soTienConLai, this.journalLinesFC.controls.indexOf(g));
          this.onBlurJournalLineAmount();
        })
      } else {
        var soTienConLai = this.amount - this.laySoTienThanhToanExceptCashAndIndex(index);
        var g = this.fb.group({
          journalId: journal.id,
          journal: journal,
          amount: [soTienConLai, [Validators.required, Validators.min(0)]],
        });
        this.journalLinesFC.push(g);

        this.onChangeJournalLineAmount(soTienConLai, this.journalLinesFC.controls.indexOf(g));
        this.onBlurJournalLineAmount();
      }
    }
  }

  getCalcAmountSuggestions(amount: number) {
    var coins = [500, 1e3, 2e3, 5e3, 1e4, 2e4, 5e4, 1e5, 2e5, 5e5];

    function greedy(amount, max) {
      for (var temp = amount, result = []; temp > 0;) {
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

  onCashSuggestionClick(amount: number) {
    this.partnerCash = amount;
    this.updateReturnCash();
  }

  isJournalActiveClass(item: AccountJournalSimple) {
    return this.journalLinesFC.value.findIndex(x => x.journalId == item.id) != -1;
  }

  onDeleteJournalLine(index) {
    this.journalLinesFC.removeAt(index);
    this.onJournalLineAmountChange();
  }

  onBlurJournalLineAmount() {
    this.partnerCash = this.getAmountCashLine();
    this.cashSuggestions = this.getCalcAmountSuggestions(this.partnerCash);
    this.updateReturnCash();
  }

  onValuePartnerCashChange(e) {
    this.updateReturnCash();
  }

  getAmountCashLine() {
    var cashLine = this.getCashLineControl();
    return cashLine.value.amount;
  }

  laySoTienThanhToanExceptCashAndIndex(index) {
    var paymentLines = this.journalLinesFC.value;
    var total = 0;
    for (var i = 0; i < paymentLines.length; i++) {
      var line = paymentLines[i];
      if (i != index && line.journal.type != 'cash') {
        total += line.amount;
      }
    }

    return total;
  }

  getCashLineControl() {
    for (var i = 0; i < this.journalLinesFC.controls.length; i++) {
      var control = this.journalLinesFC.controls[i];
      var controlValue = control.value;
      if (controlValue.journal.type == 'cash') {
        return control;
      }
    }

    return null;
  }

  onChangeJournalLineAmount(e, index) {
    var soTienThanhToan = this.laySoTienThanhToanExceptCashAndIndex(index);
    var soTienConLai = this.amount - soTienThanhToan;
    var currentLine = this.journalLinesFC.controls[index];
    var currentLineValue = currentLine.value;

    var cashLine = this.getCashLineControl();

    //kiểm tra số tiền của index, nếu là type advance thì ko cho phép vượt quá số tối đa
    if (currentLineValue.journal.type == 'advance') {
      if (currentLineValue.amount > currentLineValue.amountAdvanceBalance) {
        currentLine.get('amount').setValue(currentLineValue.amountAdvanceBalance);
      } else {
        currentLine.get('amount').setValue(currentLineValue.amount);
      }
      currentLineValue = currentLine.value;
    }

    if (currentLineValue.amount > soTienConLai) {
      currentLine.get('amount').setValue(soTienConLai);
      cashLine.get('amount').setValue(0);
    } else {
      var soTienCashConLai = soTienConLai - currentLineValue.amount;
      cashLine.get('amount').setValue(soTienCashConLai);
    }
  }

  onJournalLineAmountChange(val = null, journalLine = null) {
    if (!val && journalLine) {
      this.journalLinesFC.controls.find(x => x.value.journalId == journalLine.journalId).get('amount').setValue(0);
    }

    var cashLineFG = this.cashLineFG;
    var otherLineAmount = this.journalLinesFC.value.reduce((total, cur) => { return total + cur.amount }, 0) - cashLineFG.value.amount;

    cashLineFG.get('amount').setValue(this.amount - otherLineAmount);
    this.addCashSuggest();
    this.partnerCash = this.cashSuggestions[0];
  }

  getJournalLineAmountMax(journalLine) {
    var cash = this.cashLineFG.value.amount;
    var otherLineAmount = this.journalLinesFC.value.reduce((total, cur) => { return total + cur.amount }, 0) - cash - journalLine.amount;
    if (journalLine.journal.type != 'advance') {
      return this.amount - otherLineAmount;
    } else {
      var advanceMax = this.amount - otherLineAmount > this.advanceAmount ? this.advanceAmount : this.amount - otherLineAmount;
      return advanceMax > 0 ? advanceMax : 0;
    }
  }

  changePaymentForService(e) {
    if (e.target.checked) {
      this.linesFC.clear();
      this.defaultVal.lines.forEach((line) => {
        var g = this.fb.group(line);
        this.linesFC.push(g);
      });

      this.paymentForm.get('amount').setValue(0);
    } else {
      this.linesFC.clear();
      this.paymentForm.get('amount').setValue(this.defaultVal.amount);
    }
    // var total = this.isPaymentForService ? 0 : this.amountResidual;
    // this.paymentForm.get('amount').setValue(total);
    // this.enterMoney();
  }

  // advance payment
  isHasJournalFilter(journalType) {
    var index = this.journalLines.controls.findIndex(x => x.get('type').value == journalType);
    return index !== -1;
  }

  selectJournalFilter(journalFilter) {
    var index = this.journalLines.controls.findIndex(x => x.get('type').value == journalFilter.type);
    if (index !== -1 || this.userAmountPaymentValue <= 0) {
      return;
    }

    if (journalFilter.type == 'advance' && this.advanceAmount <= 0) {
      return false;
    }

    var amount = this.userAmountPaymentValue;
    if (journalFilter.type == 'advance') {
      amount = this.advanceAmount > amount ? amount : this.advanceAmount;
    }

    var journals = this.filteredJournals.filter(x => x.type == journalFilter.type);

    this.journalLines.push(this.fb.group({
      type: journalFilter.type,
      journals: this.fb.array(journals),
      amount: amount,
      journal: [journals[0], Validators.required]
    }));

    let computeAmount = this.amount - this.amountTotalJournalPayment;
    this.userAmountPayment =  computeAmount;
   
    setTimeout(() => {
      this.userAmountPaymentMax = computeAmount;
    }, 200);
  }

  removeJounalSelected(i) {
    var journalLine = this.journalLines.at(i);
    this.userAmountPaymentMax = this.userAmountPaymentMax + journalLine.get('amount').value;

    this.journalLines.removeAt(i);

    setTimeout(() => {
      this.userAmountPayment = this.userAmountPayment + journalLine.get('amount').value;
    }, 200);
  }

  getPaymentMethod(type: string) {
    return this.paymentMethods.find(x => x.type == type);
  }

  onChangeLine(index) {
    var line = this.linesFC.controls[index];
    var amount = line.get('amount').value;
    let max = line.get('saleOrderLine').value.amountResidual;
    if (amount > max) {
      line.get('amount').setValue(max);
    }
  }

  onChangeUserAmountPayment() {
    if (this.userAmountPayment > this.amount) {
      this.userAmountPayment = this.amount;
    }
  }

  get amountTotalJournalPayment() {
    return this.journalLines.controls.reduce((x, v) => {
      return v.get('amount').value + x;
    }, 0);
  }

  get CombineAllJournalSelected() {
    var list = [];
    this.journalLines.controls.forEach(JN => {
      var journal = JN.get('journal').value;
      if (journal) {
        var item = list.find(x => x.type == journal.type);
        if (item) {
          item.amount += JN.get('amount').value;
        } else {
          list.push({type: journal.type, amount: JN.get('amount').value});
        }
      }
    });

    var debtJournal = this.paymentForm.get('debtJournal').value;
    if (debtJournal) {
      var existJN = list.find(x => x.type == debtJournal.type);
      if (existJN) {
        existJN.amount += this.debtAmount;
      } else {
        list.push({type: debtJournal.type, amount: this.debtAmount});
      }
    }

    return list;
  }
}
