import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { of } from 'rxjs';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { CardTypeBasic, CardTypePaged, CardTypeService } from 'src/app/card-types/card-type.service';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { CheckPermissionService } from 'src/app/shared/check-permission.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { CardCardDisplay, CardCardService } from '../card-card.service';

@Component({
  selector: 'app-card-card-cu-dialog',
  templateUrl: './card-card-cu-dialog.component.html',
  styleUrls: ['./card-card-cu-dialog.component.css']
})
export class CardCardCuDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  partner: PartnerSimple;
  card: CardCardDisplay;
  title = 'Thẻ thành viên';
  submitted = false;
  filteredPartners: PartnerSimple[];
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;

  filteredTypes: CardTypeBasic[];
  @ViewChild('typeCbx', { static: true }) typeCbx: ComboBoxComponent;

  isChanged = false;

  canConfirm = false;
  canActive = false;
  canCancel = false;
  canReset = false;
  canRenew = false;
  canUpgrade = false;
  canLock = false;
  canUnLock = false;
  constructor(private fb: FormBuilder, private cardCardService: CardCardService, private partnerService: PartnerService,
    private cardTypeService: CardTypeService, public activeModal: NgbActiveModal, private errorService: AppSharedShowErrorService,
    private checkPermissionService: CheckPermissionService
    ) { }

  ngOnInit() {
    this.card = new CardCardDisplay();
    this.card.state = "draft";

    this.checkRole();
    this.formGroup = this.fb.group({
      barcode: null,
      partner: [this.partner, Validators.required],
      type: [null, Validators.required],
    });

    setTimeout(() => {
      if (this.id) {
        this.loadRecord();
      } else {
      }


      this.partnerCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.partnerCbx.loading = true)),
        switchMap(value => this.searchPartners(value))
      ).subscribe(result => {
        this.filteredPartners = result;
        this.partnerCbx.loading = false;
      });

      this.typeCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.typeCbx.loading = true)),
        switchMap(value => this.searchTypes(value))
      ).subscribe(result => {
        this.filteredTypes = result.items;
        this.typeCbx.loading = false;
      });

      this.loadFilteredPartners();
      this.loadFilteredTypes();
    });
  }

  get f() {
    return this.formGroup.controls;
  }

  getTitle() {
    if (this.id) {
      return 'Cập nhật: ' + this.title;
    }
    return 'Thêm: ' + this.title;
  }

  buttonConfirm() {
    if (this.id) {
      this.saveIfDirty().subscribe(() => {
        this.cardCardService.buttonConfirm([this.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
          this.isChanged = true;
        });
      });
    } else {
      this.saveRecord().subscribe((result: CardCardDisplay) => {
        this.id = result.id;
        this.cardCardService.buttonConfirm([result.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
          this.isChanged = true;
        });
      });
    }
  }

  saveIfDirty() {
    if (!this.formGroup.dirty) {
      return of(null);
    }

    return this.saveRecord().pipe(finalize(() => this.isChanged = true));
  }

  buttonActive() {
    if (this.id) {
      this.saveIfDirty().subscribe(() => {
        this.cardCardService.buttonActive([this.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
          this.isChanged = true;
        }, err => {
          this.errorService.show(err);
        });
      });
    } else {
      this.saveRecord().subscribe((result: CardCardDisplay) => {
        this.id = result.id;
        this.cardCardService.buttonActive([result.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
          this.isChanged = true;
        }, err => {
          this.errorService.show(err);
        });
      });
    }
  }

  buttonCancel() {
    if (this.id) {
      this.saveIfDirty().subscribe(() => {
        this.cardCardService.buttonCancel([this.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
          this.isChanged = true;
        });
      });
    }
  }

  buttonReset() {
    if (this.id) {
      this.saveIfDirty().subscribe(() => {
        this.cardCardService.buttonReset([this.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
          this.isChanged = true;
        });
      });
    }
  }

  buttonRenew() {
    if (this.id) {
      this.cardCardService.buttonRenew([this.id]).subscribe(() => {
        this.loadRecord();
      });
    }
  }

  buttonLock() {
    if (this.id) {
      this.saveIfDirty().subscribe(() => {
        this.cardCardService.buttonLock([this.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
          this.isChanged = true;
        });
      });
    }
  }

  buttonUpgrade() {
    if (this.id) {
      this.cardCardService.buttonUpgrade([this.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
        this.isChanged = true;
      });
    }
  }

  buttonUnlock() {
    if (this.id) {
      this.saveIfDirty().subscribe(() => {
        this.cardCardService.buttonUnlock([this.id]).pipe(finalize(() => this.loadRecord())).subscribe(() => {
          this.isChanged = true;
        });
      });
    }
  }

  saveRecord() {
    var value = this.formGroup.value;
    value.partnerId = value.partner ? value.partner.id : null;
    value.typeId = value.type.id;
    if (this.id) {
      return this.cardCardService.update(this.id, value).pipe(finalize(() => this.isChanged = true));
    } else {
      return this.cardCardService.create(value).pipe(finalize(() => this.isChanged = true));
    }
  }

  loadRecord() {
    this.cardCardService.get(this.id).subscribe(result => {
      this.card = result;
      this.formGroup.patchValue(result);
      if (result.partner) {
        this.filteredPartners = _.unionBy(this.filteredPartners, [result.partner], 'id');
      }
      if (result.type) {
        this.filteredTypes = _.unionBy(this.filteredTypes, [result.type], 'id');
      }
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.customer = true;
    val.search = filter;
    return this.partnerService.getAutocompleteSimple(val);
  }

  searchTypes(filter?: string) {
    var val = new CardTypePaged();
    val.search = filter || '';
    return this.cardTypeService.getPaged(val);
  }

  loadFilteredPartners() {
    this.searchPartners().subscribe(result => {
      this.filteredPartners = _.unionBy(this.filteredPartners, result, 'id');
    });
  }

  loadFilteredTypes() {
    this.searchTypes().subscribe(result => {
      this.filteredTypes = _.unionBy(this.filteredTypes, result.items, 'id');
    });
  }

  onSave() {
    this.submitted = true;
    if (this.formGroup.invalid) {
      return false;
    }

    this.saveRecord().subscribe(result => {
      if (result) {
        this.activeModal.close(result);
      } else {
        this.activeModal.close(true);
      }
    });
  }

  onClose() {
    if (this.isChanged) {
      this.activeModal.close(true);
    } else {
      this.activeModal.dismiss();
    }
  }

  checkRole(){
    this.canUpgrade=this.canRenew=this.canActive=this.canCancel=this.canLock=this.canUnLock=this.canReset=this.canConfirm = this.checkPermissionService.check(["LoyaltyCard.CardCard.Update"]);

  }
}
