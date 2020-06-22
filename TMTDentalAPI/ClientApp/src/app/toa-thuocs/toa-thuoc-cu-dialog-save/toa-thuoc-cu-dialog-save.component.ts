import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ToaThuocService, ToaThuocDefaultGet, ToaThuocLineDisplay } from '../toa-thuoc.service';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { UserSimple } from 'src/app/users/user-simple';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';

@Component({
  selector: 'app-toa-thuoc-cu-dialog-save',
  templateUrl: './toa-thuoc-cu-dialog-save.component.html',
  styleUrls: ['./toa-thuoc-cu-dialog-save.component.css']
})
export class ToaThuocCuDialogSaveComponent implements OnInit {
  toaThuocForm: FormGroup;
  id: string;
  partnerId: string;
  limit: number;
  skip: number;
  userSimpleFilter: UserSimple[] = [];
  lines: any[] = [];
  
  constructor(private fb: FormBuilder, private toaThuocService: ToaThuocService, 
    private userService: UserService, public activeModal: NgbActiveModal, 
    private intlService: IntlService, private errorService: AppSharedShowErrorService) { }

  ngOnInit() {
    this.toaThuocForm = this.fb.group({
      name: null, 
      dateObj: null, 
      note: null, 
      advice: null, 
      user: null,
      partnerId: null,
      companyId: null,
      dotKhamId: null,
      lines: this.fb.array([]),
    })

    this.getUserList();

    if (this.id) {
      setTimeout(() => {
        this.loadRecord();
      });
    } else {
      setTimeout(() => {
        this.loadDefault();
      });
    }
  }

  getUserList() {
    var val = new UserPaged;
    val.limit = this.limit;
    val.offset = this.skip;
    this.userService.autocompleteSimple(val).subscribe(
      result => {
        this.userSimpleFilter = result;
      });
  }

  loadRecord() {
    this.toaThuocService.get(this.id).subscribe(
      result => {
        console.log("result", result);
        this.toaThuocForm.patchValue(result);
        let date = new Date(result.date);
        this.toaThuocForm.get('dateObj').patchValue(date);
        this.lines = result.lines;
      });
  }

  loadDefault() {
    var val = new ToaThuocDefaultGet();
    // val.dotKhamId = this.dotKhamId;
    this.toaThuocService.defaultGet(val).subscribe(result => {
      this.toaThuocForm.patchValue(result);
      let date = new Date(result.date);
      this.toaThuocForm.get('dateObj').patchValue(date);
    });
  }

  onSave() {
    if (!this.toaThuocForm.valid) {
      return;
    }

    var val = this.toaThuocForm.value;
    val.partnerId = this.partnerId;
    val.date = this.intlService.formatDate(val.dateObj, 'yyyy-MM-ddTHH:mm:ss');
    val.lines = this.lines;
    console.log(val);
    if (this.id) {
      this.toaThuocService.update(this.id, val).subscribe(() => {
        this.activeModal.close(true);
      }, err => {
        this.errorService.show(err);
      });
    } else {
      this.toaThuocService.create(val).subscribe(result => {
        this.activeModal.close(result);
      }, err => {
        this.errorService.show(err);
      });
    }
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  updateLines(value) {

  }
}
