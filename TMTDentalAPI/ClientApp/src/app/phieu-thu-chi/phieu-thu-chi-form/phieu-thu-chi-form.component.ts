import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PhieuThuChiService } from '../phieu-thu-chi.service';
import { LoaiThuChiService, loaiThuChiPaged } from 'src/app/loai-thu-chi/loai-thu-chi.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountJournalService, AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { LoaiThuChiFormComponent } from 'src/app/loai-thu-chi/loai-thu-chi-form/loai-thu-chi-form.component';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import * as _ from 'lodash';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-phieu-thu-chi-form',
  templateUrl: './phieu-thu-chi-form.component.html',
  styleUrls: ['./phieu-thu-chi-form.component.css']
})
export class PhieuThuChiFormComponent implements OnInit {
  title: string;
  type: string;
  itemId: string;
  myForm: FormGroup;
  submitted = false;
  loaiThuChiList: any = [];
  loaiJournalList: any = [];
  filteredJournals: any = [];
  phieuThuChi: any;
  @ViewChild('journalCbx', { static: true }) journalCbx: ComboBoxComponent;
  
  constructor(private fb: FormBuilder, 
    private phieuThuChiService: PhieuThuChiService, 
    private loaiThuChiService: LoaiThuChiService, 
    private route: ActivatedRoute, private modalService: NgbModal,
    private accountJournalService: AccountJournalService, 
    private notificationService: NotificationService, 
    private router: Router, private intlService: IntlService) { }

  ngOnInit() {
    this.phieuThuChi = new Object();

    this.route.queryParamMap.subscribe(params => {
      this.type = params.get('type');
      this.itemId = params.get('id');
      if (!this.itemId) {
        this.loadDefault();
      } else {
        this.loadRecord();
      }
    });

    this.myForm = this.fb.group({
      companyId: null,
      dateObj: [null, Validators.required],
      journal: [null, Validators.required], 
      name: null,
      type: null,
      amount: 0,
      communication: null,
      reason: null,
      payerReceiver: null,
      address: null,
      loaiThuChi: [null, Validators.required] 
    });

    setTimeout(() => {
      this.loadLoaiThuChiList();
      this.loadFilteredJournals();
    });
    
  }

  loadDefault() {
    var val = { type: this.type };
    this.phieuThuChiService.defaultGet(val).subscribe((result: any) => {
      this.phieuThuChi = result;
      this.myForm.patchValue(result);

      var date = new Date(result.date);
      this.myForm.get('dateObj').setValue(date);
    });
  }

  loadRecord() {
    this.phieuThuChiService.get(this.itemId).subscribe((result: any) => {
      this.phieuThuChi = result;
      this.myForm.patchValue(result);

      var date = new Date(result.date);
      this.myForm.get('dateObj').setValue(date);
    });
  }

  converttype() {
    switch (this.type) {
      case 'thu':
        return 'loại thu';
      case 'chi':
        return 'loại chi';
    }
  }

  loadLoaiThuChiList() {
    var val = new AccountJournalFilter();
    val.type = this.type;
    this.loaiThuChiService.getPaged(val)
    .subscribe(res => {
      this.loaiThuChiList = res.items;
    }, err => {
      console.log(err);
    })
  }

  loadFilteredJournals() {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    this.accountJournalService.autocomplete(val).subscribe(res => {
      this.filteredJournals = res;
    }, err => {
      console.log(err);
    });
  }

  loadItem() {
    if (this.itemId) {
      this.phieuThuChiService.get(this.itemId)
      .subscribe((result: any) => {
        this.myForm.patchValue(result);
        this.myForm.get('dateObj').patchValue(new Date(result.date));
      }, err => {
        console.log(err);
      })
    }
  }

  getValueForm(key) {
    return this.myForm.get(key).value;
  }

  onSaveConfirm() {
    if (!this.myForm.valid) {
      return;
    }

    var val = this.myForm.value;
    val.loaiThuChiId = val.loaiThuChi.id;
    val.journalId = val.journal.id;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');

    this.phieuThuChiService.create(val).subscribe((result: any) => {
      this.phieuThuChiService.actionConfirm([result.id]).subscribe(() => {
        this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { id: result.id, type: this.type } });
      }, () => {
        this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { id: result.id, type: this.type } });
      });
    }, err => {
      console.log(err);
    });
  }

  actionNew() {
    this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { type: this.type } });
  }

  actionConfirm() {
    if (!this.myForm.valid) {
      return;
    }

    this.phieuThuChiService.actionConfirm([this.itemId])
    .subscribe(() => {
      this.loadRecord();
    }, err => {
      console.log(err);
    });
  }

  onSave() {
    if (!this.myForm.valid) {
      return;
    }

    var val = this.myForm.value;
    val.loaiThuChiId = val.loaiThuChi.id;
    val.journalId = val.journal.id;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');

    if (!this.itemId) {
      this.phieuThuChiService.create(val)
      .subscribe((result: any) => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.router.navigate(['/phieu-thu-chi/form'], { queryParams: { id: result.id, type: this.type } });
      }, err => {
        console.log(err);
      })
    } else {
      this.phieuThuChiService.update(this.itemId, val)
      .subscribe(() => {
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, err => {
        console.log(err);
      })
    }
  }

  actionCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Hủy phiếu';
    modalRef.componentInstance.body = 'Bạn có chắc chắn muốn hủy?';
    modalRef.result.then(() => {
      this.phieuThuChiService.actionCancel([this.itemId])
      .subscribe(() => {
        this.loadRecord();
      }, err => {
        console.log(err);
      })
    }, () => {
    });
  }

  quickCreateLoaiThuChi() {
    const modalRef = this.modalService.open(LoaiThuChiFormComponent, { scrollable: true, size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm ' + this.converttype();
    modalRef.componentInstance.type = this.type;
    modalRef.result.then((result: any) => {
      this.myForm.get('loaiThuChi').patchValue(result);
      this.loaiThuChiList = _.unionBy(this.loaiThuChiList, [result], 'id'); 
    }, () => {
    });
  }
}
