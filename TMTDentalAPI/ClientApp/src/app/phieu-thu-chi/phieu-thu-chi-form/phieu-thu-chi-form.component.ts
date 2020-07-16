import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PhieuThuChiService } from '../phieu-thu-chi.service';
import { LoaiThuChiService, loaiThuChiPaged } from 'src/app/loai-thu-chi/loai-thu-chi.service';
import { ActivatedRoute } from '@angular/router';
import { AccountJournalService, AccountJournalFilter } from 'src/app/account-journals/account-journal.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-phieu-thu-chi-form',
  templateUrl: './phieu-thu-chi-form.component.html',
  styleUrls: ['./phieu-thu-chi-form.component.css']
})
export class PhieuThuChiFormComponent implements OnInit {
  title: string;
  resultSelection: string;
  itemId: string;
  myForm: FormGroup;
  submitted = false;
  loaiThuChiList: any = [];
  loaiJournalList: any = [];
  
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal, 
    private phieuThuChiService: PhieuThuChiService, 
    private loaiThuChiService: LoaiThuChiService, 
    private route: ActivatedRoute, private modalService: NgbModal,
    private accountJournalService: AccountJournalService, 
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      this.title = params.get('title');
      this.resultSelection = params.get('result_selection');
      this.itemId = params.get('itemId');
      console.log(this.resultSelection);
    });

    this.myForm = this.fb.group({
      companyId: null,
      company: null,
      date: [null, Validators.required],
      journalId: null,
      journal: null, 
      state: null, 
      name: null,
      type: null,
      amount: 0,
      communication: null,
      reason: null,
      payerReceiver: null,
      address: null,
      loaiThuChiId: null,
      loaiThuChi: [null, Validators.required], 
    });

    this.myForm.patchValue({ date: new Date() });

    setTimeout(() => {
      this.loadLoaiThuChiList();
      this.loadJournals();
    });
    
  }

  getTypePayerReceiver() {
    switch (this.resultSelection) {
      case 'thu':
        return 'nộp';
      case 'chi':
        return 'thu';
    }
  }

  loadLoaiThuChiList() {
    var val = new AccountJournalFilter();
    val.type = this.resultSelection;
    this.loaiThuChiService.getPaged(val)
    .subscribe(res => {
      this.loaiThuChiList = res.items;
    }, err => {
      console.log(err);
    })
  }

  loadJournals() {
    var val = new AccountJournalFilter();
    val.type = 'bank,cash';
    this.accountJournalService.autocomplete(val)
    .subscribe(res => {
      this.loaiJournalList = res;
      if (this.loaiJournalList.length)
        this.myForm.patchValue({ journalId: this.loaiJournalList[0].id });
    }, err => {
      console.log(err);
    })
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
    val.type = this.resultSelection; 
    val.journal = this.loaiJournalList.find(el => el.id == val.journalId);
    this.phieuThuChiService.create(val)
    .subscribe(result => {
      this.itemId = result['id'];
      this.actionConfirm();
    }, err => {
      console.log(err);
    })
  }

  actionConfirm() {
    if (!this.myForm.valid) {
      return;
    }

    var ids = [];
    ids.push(this.itemId);
    this.phieuThuChiService.actionConfirm(ids)
    .subscribe(result => {
      this.myForm.patchValue({ state: 'posted' });
      this.notificationService.show({
        content: 'Xác nhận thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
    }, err => {
      console.log(err);
    })
  }

  onSave() {
    if (!this.myForm.valid) {
      return;
    }

    var val = this.myForm.value;
    val.loaiThuChiId = val.loaiThuChi.id; 
    val.type = this.resultSelection; 
    val.journal = this.loaiJournalList.find(el => el.id == val.journalId);
    this.phieuThuChiService.create(val)
    .subscribe(result => {
      console.log(result);
      this.myForm.patchValue({ 
        state: result['state'], 
        name: result['name'] 
      });
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

  actionCancel() {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
        modalRef.componentInstance.title = 'Hủy phiếu ' + this.getTypePayerReceiver();
        modalRef.componentInstance.body = 'Bạn có chắc chắn muốn hủy?';
        modalRef.result.then(() => {
          var ids = [];
          ids.push(this.itemId);
          this.phieuThuChiService.actionCancel(ids)
          .subscribe(result => {
            console.log(result);
            this.myForm.patchValue({ state: 'draft' });
            this.notificationService.show({
              content: 'Hủy phiếu thành công',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'success', icon: true }
            });
          }, err => {
            console.log(err);
          })
        }, () => {
        });
  }
}
