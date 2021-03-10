import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { debounceTime, mergeMap, switchMap, tap } from 'rxjs/operators';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AccountPaymentDisplay, AccountPaymentSave, AccountPaymentService } from 'src/app/account-payments/account-payment.service';
import { AuthService } from 'src/app/auth/auth.service';
import { EmployeeService } from 'src/app/employees/employee.service';
import { PartnerBasic, PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';

@Component({
  selector: 'app-salary-payment-dialog-v2',
  templateUrl: './salary-payment-dialog-v2.component.html',
  styleUrls: ['./salary-payment-dialog-v2.component.css']
})
export class SalaryPaymentDialogV2Component implements OnInit {
  formGroup: FormGroup;
  title: string;
  id: string;
  filteredJournals: any = [];
  filteredPartners: PartnerBasic[] = [];
  public minDateTime: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  public maxDateTime: Date = new Date(new Date(this.monthEnd.setHours(23, 59, 59)).toString());
  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;
  @ViewChild("partnerCbx", { static: true }) partnerCbx: ComboBoxComponent;
  submitted = false;
  paymentDisplay: AccountPaymentDisplay = new AccountPaymentDisplay();
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private accountPaymentService: AccountPaymentService,
    private authService: AuthService,
    private partnerService: PartnerService,
    private accountJournalService: AccountJournalService,
    private intlService: IntlService,
    private printService: PrintService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      paymentDateObj: [null, Validators.required],
      journal: [null, Validators.required],
      partner: [null, Validators.required],
      amount: [0, Validators.required],
      communication: null,
      partnerType: [null],
      paymentType: [null, Validators.required]
    });

    this.partnerCbx.filterChange
      .asObservable()
      .pipe(
        debounceTime(300),
        tap(() => (this.partnerCbx.loading = true)),
        switchMap((val) => this.searchEmployees(val.toString().toLowerCase()))
      )
      .subscribe((result) => {
        this.filteredPartners = result.items;
        this.partnerCbx.loading = false;
      });

    setTimeout(() => {
      if (this.id) {
        this.loadData();
      } else {
        this.defaultGet();
      }

      this.loadFilteredJournals();
      this.loadEmployees();
    });
  }

  defaultGet() {
    this.accountPaymentService.salaryPaymentDefaultGet().subscribe((res: any) => {
      this.paymentDisplay = res;
      this.formGroup.patchValue(res);
      var paymentDate = new Date(res.paymentDate);
      this.formGroup.get('paymentDateObj').setValue(paymentDate);
    });
  }

  loadData() {
    this.accountPaymentService.get(this.id).subscribe((result: any) => {
      this.paymentDisplay = result;
      this.formGroup.patchValue(result);
      var paymentDate = new Date(result.paymentDate);
      this.formGroup.get("paymentDateObj").patchValue(paymentDate);
    });
  }

  loadFilteredJournals() {
    var val = new AccountJournalFilter();
    val.type = "bank,cash";
    val.companyId = this.authService.userInfo.companyId;
    this.accountJournalService.autocomplete(val).subscribe(
      (result) => {
        this.filteredJournals = result;
        if (this.filteredJournals.length) {
          this.formGroup.get('journal').patchValue(this.filteredJournals[0]);
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  loadEmployees() {
    this.searchEmployees().subscribe((result) => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result.items, "id");
    });
  }

  searchEmployees(filter?: string) {
    var val = new PartnerPaged();
    val.employee = true;
    val.active = true;
    val.search = filter || "";
    return this.partnerService.getPaged(val);
  }

  getValueForm() {
    var value = this.formGroup.value;
    value.journalId = value.journal.id;
    value.partnerId = value.partner.id;
    value.paymentDate = this.intlService.formatDate(value.paymentDateObj, "yyyy-MM-ddTHH:mm:ss");
    return value;
  }

  actionConfirm(print: boolean) {
    var value = this.getValueForm();
    if (this.id) {
      this.accountPaymentService.update(this.id, value)
        .pipe(
          mergeMap(() => this.accountPaymentService.post([this.id]))
        ).subscribe(
        () => {
          this.activeModal.close({
            print: print
          });
        }
      )
    } else {
      this.accountPaymentService.create(value).pipe(
        mergeMap((result) => {
          this.id = result.id;
          return this.accountPaymentService.post([result.id]);
        })
      ).subscribe(
        () => {
          this.activeModal.close({
            id: this.id,
            print: print
          })
        }
      )
    }
  }

  actionCancel() {
    if (this.id) {
      let modalRef = this.modalService.open(ConfirmDialogComponent, { size: "sm", windowClass: "o_technical_modal", keyboard: false, backdrop: "static" });
      modalRef.componentInstance.title = "Hủy phiếu";
      modalRef.componentInstance.body = "Bạn có chắc chắn muốn hủy phiếu?";
      modalRef.result.then(
        () => {
          this.accountPaymentService.actionCancel([this.id]).subscribe(
            result => {
              this.notify('success', 'Hủy phiếu thành công');
              this.activeModal.close();
            }
          )
        }, () => {});
    }
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  checkIsDisable() {
    return this.paymentDisplay.state == 'posted' || this.paymentDisplay.state == 'cancel';
  }

  get f() {
    return this.formGroup.controls;
  }
}
