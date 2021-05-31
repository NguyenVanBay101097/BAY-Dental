import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService, load } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { AccountJournalFilter, AccountJournalService } from 'src/app/account-journals/account-journal.service';
import { AuthService } from 'src/app/auth/auth.service';
import { PartnerService } from 'src/app/partners/partner.service';
import { PrintService } from 'src/app/shared/services/print.service';
import { PartnerAdvanceService } from '../partner-advance.service';

@Component({
  selector: 'app-partner-advance-create-update-dialog',
  templateUrl: './partner-advance-create-update-dialog.component.html',
  styleUrls: ['./partner-advance-create-update-dialog.component.css']
})
export class PartnerAdvanceCreateUpdateDialogComponent implements OnInit {
  title: string;
  type: string;
  id: string;
  partnerId: string;
  formGroup: FormGroup;
  submitted = false;
  journalList: any = [];
  filteredJournals: any = [];
  partnerAdvance: any;
  amountBalance: number;
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder,
    private partnerAdvanceService: PartnerAdvanceService,
    private route: ActivatedRoute, private modalService: NgbModal,
    public activeModal: NgbActiveModal,
    private partnerService: PartnerService,
    private accountJournalService: AccountJournalService,
    private notificationService: NotificationService,
    private router: Router,
    private intlService: IntlService,
    private authService: AuthService,
    private printService: PrintService) { }

  ngOnInit() {

    this.formGroup = this.fb.group({
      dateObj: [null, Validators.required],
      journal: [null, Validators.required],
      type: null,
      amount: 0,
      note: null,
    });

    if (!this.id) {
      this.loadDefault();
    } else {
      this.loadRecord();
    }

    this.loadFilteredJournals();
    this.loadAmountAdvanceBalance();

  }

  loadDefault() {
    var val = { type: this.type, partnerId: this.partnerId };
    this.partnerAdvanceService.getDefault(val).subscribe((result: any) => {
      this.partnerAdvance = result;
      this.formGroup.patchValue(result);

      var date = new Date(result.date);
      this.formGroup.get('dateObj').setValue(date);

      if (result.journal) {
        this.filteredJournals = _.unionBy(this.filteredJournals, [result.journal], "id");
      }

    });
  }

  loadRecord() {
    this.partnerAdvanceService.get(this.id).subscribe((result: any) => {
      this.partnerAdvance = result;
      this.formGroup.patchValue(result);

      var date = new Date(result.date);
      this.formGroup.get('dateObj').setValue(date);

      if (result.journal) {
        this.filteredJournals = _.unionBy(this.filteredJournals, [result.journal], "id");
      }

    });
  }

  get f() { return this.formGroup.controls; }

  loadFilteredJournals() {
    this.searchJournals().subscribe((result) => {
      this.filteredJournals = _.unionBy(this.filteredJournals, result, "id");
    });
  }

  loadAmountAdvanceBalance() {
    if (this.partnerId) {
      this.partnerService.getAmountAdvanceBalance(this.partnerId).subscribe((res: number) => {
        this.amountBalance = Math.abs(res);
      });
    }
  }

  onClose() {
    this.activeModal.dismiss();
  }

  searchJournals(q?: string) {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    val.search = q || '';
    val.companyId = this.authService.userInfo.companyId;
    return this.accountJournalService.autocomplete(val);
  }

  computeForm(val) {
    val.date = this.intlService.formatDate(val.dateObj, "yyyy-MM-ddTHH:mm");
    val.journalId = val.journal ? val.journal.id : null;
    val.companyId = this.authService.userInfo.companyId;
    val.PartnerId = this.partnerId;
    val.type = this.type;
    val.state = this.partnerAdvance.state;
    return val;
  }


  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) {
      return false;
    }
    var val = this.formGroup.value;
    val = this.computeForm(val);

    if (this.id) {
      this.partnerAdvanceService.update(this.id, val).subscribe(
        () => {
          this.notificationService.show({
            content: 'Cập nhật thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.onPrint(this.id);
          this.activeModal.close();

        }
      );
    } else {
      this.partnerAdvanceService.create(val).subscribe(
        (result:any) => {
          this.notificationService.show({
            content: 'Lưu thành công',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'success', icon: true }
          });

          this.onPrint(result.id);
          this.activeModal.close();

        }
      );
    }

  }

  onConfirmPrint(print) {
    this.submitted = true;
    if (this.formGroup.invalid) {
      return false;
    }

    var val = this.formGroup.value;
    val = this.computeForm(val);

    if (this.id) {
      this.partnerAdvanceService.update(this.id, val).subscribe(
        () => {
          this.partnerAdvanceService.actionConfirm([this.id]).subscribe(
            () => {
              this.notificationService.show({
                content: "Xác nhận thành công",
                hideAfter: 3000,
                position: { horizontal: "center", vertical: "top" },
                animation: { type: "fade", duration: 400 },
                type: { style: "success", icon: true },
              });
              if (print) {
                this.onPrint(this.id);
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
    } else {
      this.partnerAdvanceService.create(val).subscribe(
        (result: any) => {
          this.partnerAdvanceService.actionConfirm([result.id]).subscribe(
            () => {
              this.notificationService.show({
                content: "Xác nhận thành công",
                hideAfter: 3000,
                position: { horizontal: "center", vertical: "top" },
                animation: { type: "fade", duration: 400 },
                type: { style: "success", icon: true },
              });
              if (print) {
                this.onPrint(result.id);
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

  onPrint(id) {
    if (!id) {
      return;
    }
    this.partnerAdvanceService.getPrint(id).subscribe((result: any) => {
      this.printService.printHtml(result.html);
    });
  }

}
