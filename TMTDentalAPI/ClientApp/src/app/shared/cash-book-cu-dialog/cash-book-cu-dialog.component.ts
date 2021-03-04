import { Component, OnInit, ViewChild, ɵConsole } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { IntlService } from "@progress/kendo-angular-intl";
import { NotificationService } from "@progress/kendo-angular-notification";
import * as _ from "lodash";
import { debounceTime, switchMap, tap } from "rxjs/operators";
import {
  AccountJournalFilter,
  AccountJournalService,
} from "src/app/account-journals/account-journal.service";
import { AccountPaymentDisplay, AccountPaymentService } from "src/app/account-payments/account-payment.service";
import { AuthService } from "src/app/auth/auth.service";
import { LoaiThuChiService } from "src/app/loai-thu-chi/loai-thu-chi.service";
import { PartnerPaged, PartnerSimple } from "src/app/partners/partner-simple";
import { PartnerFilter, PartnerService } from "src/app/partners/partner.service";
import { PhieuThuChiDisplay, PhieuThuChiService } from "src/app/phieu-thu-chi/phieu-thu-chi.service";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { LoaiThuChiFormComponent } from "src/app/shared/loai-thu-chi-form/loai-thu-chi-form.component";
import { PrintService } from "src/app/shared/services/print.service";

@Component({
  selector: "app-cash-book-cu-dialog",
  templateUrl: "./cash-book-cu-dialog.component.html",
  styleUrls: ["./cash-book-cu-dialog.component.css"],
})
export class CashBookCuDialogComponent implements OnInit {
  title: string;
  type: string;
  id: string;
  formGroup: FormGroup;
  submitted: boolean = false;
  loaiThuChiList: any = [];
  filteredJournals: any = [];
  phieuThuChiDisplay: PhieuThuChiDisplay = new PhieuThuChiDisplay();
  reload = false;

  get f() { return this.formGroup.controls; }

  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private loaiThuChiService: LoaiThuChiService,
    private accountJournalService: AccountJournalService,
    private authService: AuthService,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private printService: PrintService,
    private phieuThuChiService: PhieuThuChiService,
    private partnerService: PartnerService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      amount: 0,
      communication: null,
      paymentDateObj: [null, Validators.required],
      journal: [null, Validators.required],
      loaiThuChi: [null, Validators.required],
      name: null,
      state: null,
      partnerType: ['customer', Validators.required],
      partner: [null],
    });

    setTimeout(() => {
      this.loadLoaiThuChiList();
      this.loadFilteredJournals();
      if (!this.id) {
        this.loadDefault();
      } else {
        this.loadRecord();
      }

      this.filterChangeCombobox();
    });
  }

  getType() {
    if (this.type == "inbound") {
      return "Phiếu thu";
    } else {
      return "Phiếu chi";
    }
  }

  getTitle() {
    if (!this.id) {
      return "Tạo: " + this.getType();
    } else {
      return "Chỉnh sửa: " + this.getType();
    }
  }

  get seeForm() {
    var state = this.formGroup.get('state').value;
    var val = state == 'posted' || state == 'cancel';
    return val;
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  loadDefault() {
    this.phieuThuChiService.defaultGet({type: this.type}).subscribe((res: any) => {
      this.phieuThuChiDisplay = res;
      this.formGroup.patchValue(res);
      var paymentDate = new Date(res.paymentDate);
      this.formGroup.get('paymentDateObj').setValue(paymentDate);
      this.loadFilteredPartners(this.getValueForm("partnerType"));
    });
  }

  loadRecord() {
    this.phieuThuChiService.get(this.id).subscribe((result: any) => {
      this.phieuThuChiDisplay = result;
      this.formGroup.patchValue(result);
      var paymentDate = new Date(result.paymentDate);
      this.formGroup.get("paymentDateObj").patchValue(paymentDate);
      this.loadFilteredPartners(this.getValueForm("partnerType"));
    });
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
        this.formGroup.get("journal").patchValue(this.filteredJournals[0]);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  loadPartnerTypes() {
    this.partnerTypes = [
      {
        text: "Khách hàng",
        value: "customer"
      }, {
        text: "Nhà cung cấp",
        value: "supplier"
      }, {
        text: "Nhân viên",
        value: "employee"
      }
    ];
    this.filteredPartnerTypes = this.partnerTypes.slice();
  }

  displayPartnerType(partnerType) {
    switch (partnerType) {
      case 'customer':
        return 'Khách hàng';
      case 'supplier':
        return 'Nhà cung cấp';
      case 'employee':
        return 'Nhân viên';
      default:
        return '';
    }
  }

  loadFilteredPartners(partnerType: string, search?: string) {
    this.searchPartners(partnerType, search).subscribe(result => {
      this.filteredPartners = result;
    });
  }

  change_partnerType(item) {
    this.formGroup.get('partner').patchValue(null);
    this.loadFilteredPartners(item);
  }

  searchPartners(partnerType: string, search?: string) {
    var paged = new PartnerFilter();
    switch (partnerType) {
      case "customer":
        paged.customer = true;
        break;
      case "supplier":
        paged.supplier = true;
        break;
      case "employee":
        paged.employee = true;
        break;
      default:
        break;
    }

    paged.active = true;
    paged.search = search || "";
    return this.partnerService.autocomplete2(paged);
  }

  filterChangeCombobox() {
    this.partnerCbx.filterChange
        .asObservable()
        .pipe(
          debounceTime(300),
          tap(() => (this.partnerCbx.loading = true)),
          switchMap((val) => this.searchPartners(val.toString().toLowerCase()))
        )
        .subscribe((result) => {
          this.filteredPartners = result;
          this.partnerCbx.loading = false;
        });
  }

  filter_partnerType(value) {
    this.filteredPartnerTypes = this.partnerTypes.filter((s) => s.text.toLowerCase().indexOf(value.toLowerCase()) !== -1);
  }

  quickCreateLoaiThuChi() {
    const modalRef = this.modalService.open(LoaiThuChiFormComponent, {
      scrollable: true,
      size: "lg",
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm loại " + this.getType();
    modalRef.componentInstance.type = this.getType();
    modalRef.result.then(
      (result: any) => {
        this.formGroup.get("loaiThuChi").patchValue(result);
        this.loaiThuChiList = _.unionBy(this.loaiThuChiList, [result], "id");
      },
      () => { }
    );
  }

  onSavePrint(print) {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.loaiThuChiId = val.loaiThuChi.id;
    val.journalId = val.journal.id;
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, "yyyy-MM-ddTHH:mm:ss");
    val.partnerId = val.partner ? val.partner.id : null;
    val.paymentType = this.type;

    if (!this.id) {
      this.phieuThuChiService.create(val).subscribe(
        (result: any) => {
          this.notificationService.show({
            content: "Lưu thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          this.id = result.id;
          this.reload = true;
          if (print) {
            this.printPhieu(this.id);
          }
          this.submitted = false;
        },
        (error) => {
          console.log(error);
          this.submitted = false;
        }
      );
    } else {
      this.phieuThuChiService.update(this.id, val).subscribe(
        (result: any) => {
          this.notificationService.show({
            content: "Lưu thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          if (print) {
            this.printPhieu(this.id);
          }
          this.submitted = false;
          this.activeModal.close();
        },
        (error) => {
          console.log(error);
          this.submitted = false;
        }
      );
    }
  }

  onConfirmPrint(print) {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    val.loaiThuChiId = val.loaiThuChi.id;
    val.journalId = val.journal.id;
    val.paymentDate = this.intlService.formatDate(val.paymentDateObj, "yyyy-MM-ddTHH:mm:ss");
    val.partnerId = val.partner ? val.partner.id : null;
    val.paymentType = this.type;

    if (!this.id) {
      this.phieuThuChiService.create(val).subscribe(
        (result: any) => {
          this.phieuThuChiService.post([result.id]).subscribe(
            () => {
              this.notificationService.show({
                content: "Xác nhận thành công",
                hideAfter: 3000,
                position: { horizontal: "center", vertical: "top" },
                animation: { type: "fade", duration: 400 },
                type: { style: "success", icon: true },
              });
              
              this.activeModal.close({
                id: result.id,
                print: print
              });
            },
            (error) => {
              console.log(error);
              this.submitted = false;
            }
          );
        },
        (error) => {
          console.log(error);
          this.submitted = false;
        }
      );
    } else {
      this.phieuThuChiService.post([this.id]).subscribe(
        (result) => {
          this.notificationService.show({
            content: "Xác nhận thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          
          this.activeModal.close({
            print: print
          });
        },
        (error) => {
          console.log(error);
          this.submitted = false;
        }
      );
    }
  }

  onPrint() {
    this.printPhieu(this.id);
  }

  onClose() {
    if (this.reload) {
      this.activeModal.close();
    } else {
      this.activeModal.dismiss();
    }
  }

  printPhieu(id: string) {
    this.phieuThuChiService.getPrint(id).subscribe((data: any) => {
      this.printService.printHtml(data.html);
    });
  }

  onCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = `Hủy phiếu ${this.getType().toLowerCase()}`;
    modalRef.componentInstance.body = `Bạn chắc chắn muốn hủy phiếu ${this.getType().toLowerCase()}?`;
    modalRef.result.then((result) => {
      this.phieuThuChiService.actionCancel([this.id]).subscribe((result) => {
        this.notificationService.show({
          content: "Hủy phiếu thành công",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });
        this.activeModal.close();
      }, (error) => {
      });
    }, (error) => {
    });
  }
}
