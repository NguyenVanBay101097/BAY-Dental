import { Component, OnInit, ViewChild, ɵConsole } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { IntlService } from "@progress/kendo-angular-intl";
import { NotificationService } from "@progress/kendo-angular-notification";
import * as _ from "lodash";
import {
  AccountJournalFilter,
  AccountJournalService,
} from "src/app/account-journals/account-journal.service";
import { AccountPaymentService } from "src/app/account-payments/account-payment.service";
import { AuthService } from "src/app/auth/auth.service";
import { LoaiThuChiService } from "src/app/loai-thu-chi/loai-thu-chi.service";
import { PhieuThuChiService } from "src/app/phieu-thu-chi/phieu-thu-chi.service";
import { LoaiThuChiFormComponent } from "src/app/shared/loai-thu-chi-form/loai-thu-chi-form.component";
import { PrintService } from "src/app/shared/services/print.service";
import { SalaryPaymentService } from "src/app/shared/services/salary-payment.service";
import { CashBookService } from "../cash-book.service";

@Component({
  selector: "app-cash-book-cu-dialog",
  templateUrl: "./cash-book-cu-dialog.component.html",
  styleUrls: ["./cash-book-cu-dialog.component.css"],
})
export class CashBookCuDialogComponent implements OnInit {
  title: string = null;
  type: string = null;
  type2: string = null;
  state: string = null;
  itemId: string = null;
  resModel: string = null;
  formGroup: FormGroup;
  submitted: boolean = false;
  loaiThuChiList: any = [];
  loaiJournalList: any = [];
  filteredJournals: any = [];
  seeForm: boolean = false;

  get f() {
    return this.formGroup.controls;
  }

  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private phieuThuChiService: PhieuThuChiService,
    private loaiThuChiService: LoaiThuChiService,
    private accountJournalService: AccountJournalService,
    private authService: AuthService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private printService: PrintService,
    private paymentService: AccountPaymentService,
    private salaryPaymentService: SalaryPaymentService, 
  ) {}

  ngOnInit() {
    this.setTitle();

    this.formGroup = this.fb.group({
      id: null,
      address: null,
      amount: 0,
      communication: null,
      companyId: null,
      dateObj: [null, Validators.required],
      journal: [null, Validators.required],
      loaiThuChi: [null, Validators.required],
      name: null,
      payerReceiver: null,
      reason: null,
      state: null,
      type: null,
    });

    setTimeout(() => {
      this.loadLoaiThuChiList();
      this.loadFilteredJournals();
      if (!this.itemId) {
        this.loadDefault();
      } else {
        this.loadRecord();
      }
    });
  }

  getType() {
    if (this.type == "inbound") {
      return "thu";
    } else {
      return "chi";
    }
  }

  setTitle() {
    if (!this.resModel || this.resModel == "phieu.thu.chi") {
      if (!this.itemId) {
        this.title = "Tạo phiếu " + this.getType();
      } else {
        this.title = "Chỉnh sửa phiếu " + this.getType();
      }
    } else {
      this.title = this.type2;
    }
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  loadDefault() {
    var val = { type: this.getType() };
    this.phieuThuChiService.defaultGet(val).subscribe((result: any) => {
      this.formGroup.patchValue(result);

      var date = new Date(result.date);
      this.formGroup.get("dateObj").setValue(date);
    });
  }

  loadRecord() {
    if (this.resModel) {
      if (this.resModel == "phieu.thu.chi") {
        this.phieuThuChiService.get(this.itemId).subscribe((result: any) => {
          this.formGroup.patchValue(result);
          var date = new Date(result.date);
          this.formGroup.get("dateObj").setValue(date);
        });
      } else if (this.resModel == "account.payment") {
        this.paymentService.get(this.itemId).subscribe((result) => {
          this.formGroup.patchValue(result);
          this.formGroup.get("payerReceiver").setValue(result.partner['name']);
          var date = new Date(result.paymentDate);
          this.formGroup.get("dateObj").setValue(date);
        });
      } else if (this.resModel == "salary.payment") {
        this.salaryPaymentService.getIdSP(this.itemId).subscribe((result) => {
          // Convert object keys to lower case
          var result_keyLowCase = {};
          for (var [key, value] of Object.entries(result)) {
            if (value && typeof(value) == "object") {
              var r_keyLowCase = {};
              for (var [k, v] of Object.entries(value)) {
                r_keyLowCase[k.toLowerCase()] = v;
              }
              value = r_keyLowCase;
            }
            result_keyLowCase[key.toLowerCase()] = value;
          }
          result = result_keyLowCase;
          this.formGroup.patchValue(result);
          this.formGroup.get("payerReceiver").setValue(result.employee['name']);
          let date = new Date(result.date);
          this.formGroup.get("dateObj").patchValue(date);
        });
      }
    } else {
      this.phieuThuChiService.get(this.itemId).subscribe((result: any) => {
        this.formGroup.patchValue(result);
        var date = new Date(result.date);
        this.formGroup.get("dateObj").setValue(date);
      });
    }
  }

  loadLoaiThuChiList() {
    var val = new AccountJournalFilter();
    val.type = this.getType();
    this.loaiThuChiService.getPaged(val).subscribe(
      (res) => {
        this.loaiThuChiList = res.items;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  loadFilteredJournals() {
    var val = new AccountJournalFilter();
    val.type = "bank,cash";
    val.companyId = this.authService.userInfo.companyId;
    this.accountJournalService.autocomplete(val).subscribe(
      (res) => {
        this.filteredJournals = res;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  loadItem() {
    if (this.itemId) {
      this.phieuThuChiService.get(this.itemId).subscribe(
        (result: any) => {
          this.formGroup.patchValue(result);
          this.formGroup.get("dateObj").patchValue(new Date(result.date));
        },
        (error) => {
          console.log(error);
        }
      );
    }
  }

  quickCreateLoaiThuChi() {
    const modalRef = this.modalService.open(LoaiThuChiFormComponent, {
      scrollable: true,
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm loại " + this.type;
    modalRef.componentInstance.type = this.getType();
    modalRef.result.then(
      (result: any) => {
        this.formGroup.get("loaiThuChi").patchValue(result);
        this.loaiThuChiList = _.unionBy(this.loaiThuChiList, [result], "id");
      },
      () => {}
    );
  }

  onSave() {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.loaiThuChiId = val.loaiThuChi.id;
    val.journalId = val.journal.id;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");

    if (!this.itemId) {
      this.phieuThuChiService.create(val).subscribe(
        (result: any) => {
          this.notificationService.show({
            content: "Lưu thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          this.itemId = result.id;
          this.setTitle();
        },
        (error) => {
          console.log(error);
        }
      );
    } else {
      this.phieuThuChiService.update(this.itemId, val).subscribe(
        (result: any) => {
          this.notificationService.show({
            content: "Lưu thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          this.activeModal.close();
        },
        (error) => {
          console.log(error);
        }
      );
    }
  }

  onConfirmPrint(print) {
    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.loaiThuChiId = val.loaiThuChi.id;
    val.journalId = val.journal.id;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");

    if (!this.itemId) {
      this.phieuThuChiService.create(val).subscribe(
        (result: any) => {
          this.itemId = result.id;
          this.setTitle();
          this.phieuThuChiService.actionConfirm([this.itemId]).subscribe(
            (result) => {
              this.notificationService.show({
                content: "Xác nhận thành công",
                hideAfter: 3000,
                position: { horizontal: "center", vertical: "top" },
                animation: { type: "fade", duration: 400 },
                type: { style: "success", icon: true },
              });
              this.formGroup.get("state").setValue("posted");
              if (print) {
                this.printPhieu(this.itemId);
              }
            },
            (error) => {
              console.log(error);
            }
          );
        },
        (error) => {
          console.log(error);
        }
      );
    } else {
      this.phieuThuChiService.actionConfirm([this.itemId]).subscribe(
        (result) => {
          this.notificationService.show({
            content: "Xác nhận thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          this.formGroup.get("state").setValue("posted");
          if (print) {
            this.printPhieu(this.itemId);
          }
        },
        (error) => {
          console.log(error);
        }
      );
    }
  }

  onPrint() {
    this.printPhieu(this.itemId);
  }

  onCreate() {
    this.loadDefault();
    this.itemId = null;
    this.setTitle();
  }

  printPhieu(id: string) {
    this.phieuThuChiService.getPrint(id).subscribe((data: any) => {
      this.printService.printHtml(data.html);
    });
  }
}
