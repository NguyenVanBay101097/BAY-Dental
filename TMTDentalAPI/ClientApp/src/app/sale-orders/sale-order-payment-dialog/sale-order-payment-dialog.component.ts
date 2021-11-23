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
import { AppSharedShowErrorService } from "src/app/shared/shared-show-error.service";
import { SaleOrderPaymentAdvancedDialogComponent } from "../sale-order-payment-advanced-dialog/sale-order-payment-advanced-dialog.component";

@Component({
  selector: "app-sale-order-payment-dialog",
  templateUrl: "./sale-order-payment-dialog.component.html",
  styleUrls: ["./sale-order-payment-dialog.component.css"]
})
export class SaleOrderPaymentDialogComponent implements OnInit {
  paymentForm: FormGroup;
  defaultVal: any;// data from api saleorderpaymentbysaleorderid
  partnerDebt = 0; // số tiền công nợ
  // advanced payment
  userAmountPayment = 0; // số tiền khách nhập
  filteredJournals: any[] = [// danh sách journal filter box
    {type:'cash',name: 'Tiền mặt', journals: [], journal:null},
    {type:'bank',name: 'Ngân hàng', journals: [], journal:null},
    {type:'debt',name: 'Ghi công nợ', journals: [], journal:null},
    {type:'advance',name: 'Tạm ứng', journals: [], journal:null}
  ];
  advanceAmount: number = 0; // số tiền tạm ứng
  selectedJournals = []; // danh sách journal filter box được chọn
  debtJournalSelected: any; // phương thức thanh toán công nợ được chọn
  

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
  partner: any;

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

  ) { }

  ngOnInit() {
    this.paymentForm = this.fb.group({
      amount: [0, [Validators.required, Validators.min(1)]],
      date: [new Date(), Validators.required],
      orderId: [null, Validators.required],
      companyId: [null, Validators.required],
      journalLines: this.fb.array([]),
      lines: this.fb.array([]),
      note: "",
      state: "draft",
      isDebtPayment: false,
      debtJournalId: null,
      debtAmount: [0],
      debNote: null,
      partnerId: this.partner.id
    });

    this.paymentFC.isDebtPayment.valueChanges.subscribe(checked => {
      if (checked) {
        const validators = [ Validators.required, Validators.min(1) ];
        this.paymentFC.debtAmount.setValidators(validators);
        this.paymentFC.debtAmount.setValue(this.partnerDebt);
      } else {
        this.paymentFC.debtAmount.clearValidators();
      }
      this.paymentFC.debtAmount.updateValueAndValidity();
    });

    setTimeout(() => {
      if (this.defaultVal) {
        this.defaultVal.date = new Date(this.defaultVal.date);
        this.paymentForm.patchValue(this.defaultVal);
        this.maxAmount = this.getValueForm("amount");
       
        this.linesFC.clear();
        this.defaultVal.lines.forEach((line) => {
          var g = this.fb.group(line);
          this.linesFC.push(g);
        });

        this.paymentForm.markAsPristine();
      }
      this.changePaymentForService();
     
    });

    // advance payment
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
          filterJN.journal = jn;
        }
      });
    })
  }; 

  get amountResidual() {
   return this.defaultVal.lines.reduce((total,line) => {
      return total + line?.saleOrderLine?.amountResidual;
    },0);
  }

  get amount() {
    return this.paymentForm.get('amount').value;
  }

  get paymentFC() {
    return this.paymentForm.controls ;
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
    return this.paymentForm.get('debtAmount').value;
  }

  filteredJournalsByType(type){
    return this.filteredJournals.filter(x=> type.includes(x.type));
  }

  getAmountTotalPayment() {
    let amount = this.amount | 0;
    let debtAmount = this.debtAmount | 0;
    return amount + debtAmount;
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
    var val = this.getValueFormSave();
    if (val == null) return;
    this.paymentService.paymentSaleOrderAndDebt(val).subscribe(result => {
        this.activeModal.close(true);
    },(err) => {
        this.errorService.show(err);
    })
  }

  saveAndPrint() {
    // if (!this.isThanhToanDu()) {
    //   alert('Vui lòng phân bổ đủ số tiền cần thanh toán');
    //   return false;
    // }

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
    },(err) => {
        this.errorService.show(err);
    })
  }

  getValueFormSave() {
    var val = this.paymentForm.value;
    val.date = this.intlService.formatDate(
      val.date,
      "d",
      "en-US"
    );
    if(this.userAmountPayment > 0){
      var cashJournalFilter = this.filteredJournals.find(x=> x.type == 'cash');
      var cashSelected = this.selectedJournals.find(x=> x.type == 'cash');
      if(cashSelected){
        cashSelected.amount = cashSelected.amount + this.userAmountPayment;
      } else {
        this.selectJournalFilter(cashJournalFilter);
      }
    }

    val.journalLines = this.selectedJournals.map(x=> {
     return {
      journalId: x.journal.id,
      amount: x.amount
     }
    });

    val.debtJournalId = this.debtJournalSelected.journal.id;
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
    this.userAmountPayment = this.amount;
    this.debtJournalSelected = Object.assign({},this.filteredJournals.find(x=> x.type == 'cash'));
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
    this.selectedJournals = [];
    this.debtJournalSelected = null;
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
        this.partnerService.getAmountAdvanceBalance(this.partner.id).subscribe(result => {
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

  changePaymentForService() {
    let total = this.isPaymentForService ? 0 : this.amountResidual;
    this.paymentForm.get('amount').setValue(total);
    this.enterMoney();
  }

  // advance payment
  isHasJournalFilter(journalType) {
    var types = this.selectedJournals.map(x => x.type);
    return types.includes(journalType);
  }

  selectJournalFilter(journalFilter) {
    var types = this.selectedJournals.map(x => x.type);
    if (types.findIndex(x => x == journalFilter.type) != -1) {
      return;
    }
    journalFilter.amount = this.userAmountPayment;
    journalFilter.journal = journalFilter.journals[0];
    this.selectedJournals.push(journalFilter);
    this.userAmountPayment = this.amount - this.amountTotalJournalPayment;
  }

  removeJounalSelected(journal) {
    let index = this.selectedJournals.findIndex(x => x.type == journal.type);
    if (index != -1) {
      let jnItem = this.selectedJournals[index];
      this.userAmountPayment = this.amount - this.amountTotalJournalPayment;
      this.selectedJournals.splice(index,1);
    }
  }

  get amountTotalJournalPayment(){
    return this.selectedJournals.reduce((x, v)=> {
      return v.amount + x;
    },0);
  }

  get CombineAllJournalSelected(){
    var newSelected = [];
    this.selectedJournals.forEach(JN => {
      newSelected.push(Object.assign({}, JN));
    });
   if(this.debtJournalSelected) {
    var existJN = newSelected.find(x=> this.debtJournalSelected.type == x.type);
    if(existJN) {
      existJN.amount = existJN.amount + this.getValueForm('debtAmount');
    }
   }
    return newSelected;
  }



}
