import { Component, OnInit, ViewChild, ɵConsole } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { NgbActiveModal, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ComboBoxComponent } from "@progress/kendo-angular-dropdowns";
import { IntlService } from "@progress/kendo-angular-intl";
import { NotificationService } from "@progress/kendo-angular-notification";
import * as _ from "lodash";
import { of } from "rxjs";
import { debounceTime, switchMap, tap } from "rxjs/operators";
import {
  AccountJournalFilter,
  AccountJournalService,
} from "src/app/account-journals/account-journal.service";
import { AccountPaymentDisplay, AccountPaymentService } from "src/app/account-payments/account-payment.service";
import { AuthService } from "src/app/auth/auth.service";
import { loaiThuChiPaged, LoaiThuChiService } from "src/app/loai-thu-chi/loai-thu-chi.service";
import { PartnerPaged, PartnerSimple } from "src/app/partners/partner-simple";
import { PartnerFilter, PartnerService } from "src/app/partners/partner.service";
import { PhieuThuChiDisplay, PhieuThuChiService } from "src/app/phieu-thu-chi/phieu-thu-chi.service";
import { ConfirmDialogComponent } from "src/app/shared/confirm-dialog/confirm-dialog.component";
import { LoaiThuChiFormComponent } from "src/app/shared/loai-thu-chi-form/loai-thu-chi-form.component";
import { PrintService } from "src/app/shared/services/print.service";
import { CheckPermissionService } from "../check-permission.service";

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
  filteredPartners: any = [];
  filteredPartnerTypes: any = [
    { value: 'customer', text: 'Khách hàng' },
    { value: 'supplier', text: 'Nhà cung cấp' },
    { value: 'employee', text: 'Nhân viên' },
  ];

  get f() { return this.formGroup.controls; }

  @ViewChild("journalCbx", { static: true }) journalCbx: ComboBoxComponent;
  @ViewChild("partnerCbx", { static: true }) partnerCbx: ComboBoxComponent;
  @ViewChild("loaiThuChiCbx", { static: true }) loaiThuChiCbx: ComboBoxComponent;

  // permission 
  canPhieuThuChiUpdate = this.checkPermissionService.check(["Account.PhieuThuChi.Update"]);

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
    private checkPermissionService: CheckPermissionService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      amount: 0,
      dateObj: [null, Validators.required],
      journal: [null, Validators.required],
      loaiThuChi: [null, Validators.required],
      reason: null,
      partnerType: null,
      partner: null,
      isAccounting: false
    });

    setTimeout(() => {
      this.loadLoaiThuChiList();
      this.loadFilteredJournals();
      if (!this.id) {
        this.loadDefault();
      } else {
        this.loadRecord();
      }

      this.partnerCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.partnerCbx.loading = true)),
        switchMap(value => this.searchPartners(value))
      ).subscribe(result => {
        this.filteredPartners = result.items;
        this.partnerCbx.loading = false;
      });

      this.loaiThuChiCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.loaiThuChiCbx.loading = true)),
        switchMap(value => this.searchLoaiThuChis(value))
      ).subscribe(result => {
        this.loaiThuChiList = result.items;
        this.loaiThuChiCbx.loading = false;
      });
    });
  }

  getType() {
    if (this.type == "thu") {
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

  getLoaiThuChiTitle() {
    if (this.type == "thu") {
      return "Loại thu";
    } else {
      return "Loại chi";
    }
  }

  get formReadonly() {
    if (!this.canPhieuThuChiUpdate) {
      return true;
    }
    return this.phieuThuChiDisplay.state == 'posted' || this.phieuThuChiDisplay.state == 'cancel';
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  loadDefault() {
    this.phieuThuChiService.defaultGet({ type: this.type }).subscribe((res: any) => {
      this.phieuThuChiDisplay = res;
      this.formGroup.patchValue(res);
      var date = new Date(res.date);
      this.formGroup.get('dateObj').setValue(date);
    });
  }

  showPartnerTypes(partnerType) {
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

  loadRecord() {
    this.phieuThuChiService.get(this.id).subscribe((result: any) => {
      this.phieuThuChiDisplay = result;
      this.formGroup.patchValue(result);
      var date = new Date(result.date);
      this.formGroup.get("dateObj").patchValue(date);

      if (result.partner) {
        this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
      }

      if (result.partnerType) {
        this.loadFilteredPartners();
      }
    });
  }

  onChangeLoai(val) {
    debugger
    if (val) {
      this.formGroup.get('isAccounting').setValue(val.isAccounting);
    }
  }

  loadLoaiThuChiList() {
    var val = new loaiThuChiPaged();
    val.type = this.type;
    this.loaiThuChiService.getPaged(val).subscribe(
      (res) => {
        this.loaiThuChiList = res.items;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  searchLoaiThuChis(search?: string) {
    var val = new loaiThuChiPaged();
    val.type = this.type;
    val.search = search || '';
    return this.loaiThuChiService.getPaged(val);
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

  changePartnerType(partnerType: string) {
    this.formGroup.get('partner').setValue(null);
    this.loadFilteredPartners();
  }

  loadFilteredPartners() {
    this.searchPartners().subscribe((result: any) => {
      if (result) {
        this.filteredPartners = result.items;
      }
    });
  }

  searchPartners(search?: string) {
    var partnerType = this.formGroup.get('partnerType').value;
    if (!partnerType) {
      return of(null);
    }

    var paged = new PartnerPaged();
    paged.active = true;
    paged.search = search || '';
    if (partnerType == 'customer') {
      paged.customer = true;
    } else if (partnerType == 'supplier') {
      paged.supplier = true;
    } else if (partnerType == 'employee') {
      paged.employee = true;
    }

    return this.partnerService.getPaged(paged);
  }

  quickCreateLoaiThuChi() {
    const modalRef = this.modalService.open(LoaiThuChiFormComponent, {
      scrollable: true,
      size: 'xl',
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.title = "Thêm: " + this.getLoaiThuChiTitle();
    modalRef.componentInstance.type = this.type;
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
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    val.partnerId = val.partner ? val.partner.id : null;
    val.type = this.type;
    val.accountType = "other";

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
    val.partnerId = val.partner ? val.partner.id : null;
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm:ss");
    val.type = this.type;
    val.accountType = "other";

    if (!this.id) {
      this.phieuThuChiService.create(val).subscribe(
        (result: any) => {
          this.phieuThuChiService.actionConfirm([result.id]).subscribe(
            () => {
              this.notificationService.show({
                content: "Xác nhận thành công",
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
              this.activeModal.close();
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
      this.phieuThuChiService.update(this.id, val).subscribe(
        (result: any) => {
          this.phieuThuChiService.actionConfirm([this.id]).subscribe(
            (result) => {
              this.notificationService.show({
                content: "Xác nhận thành công",
                hideAfter: 3000,
                position: { horizontal: "center", vertical: "top" },
                animation: { type: "fade", duration: 400 },
                type: { style: "success", icon: true },
              });
              
              if (print) {
                this.printPhieu(this.id);
              }

              this.activeModal.close();
            },
            (error) => {
              console.log(error);
              this.submitted = false;
            }
          );
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
      this.printService.printHtml(data);
    });
  }

  onCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = `Hủy ${this.getType().toLowerCase()}`;
    modalRef.componentInstance.body = `Bạn chắc chắn muốn hủy ${this.getType().toLowerCase()}?`;
    modalRef.result.then((result) => {
      this.phieuThuChiService.actionCancel([this.id]).subscribe((result) => {
        this.notificationService.show({
          content: "Hủy thành công",
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
