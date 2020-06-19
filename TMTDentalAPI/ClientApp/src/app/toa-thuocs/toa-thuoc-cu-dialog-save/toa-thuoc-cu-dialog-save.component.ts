import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ToaThuocService, ToaThuocDefaultGet } from '../toa-thuoc.service';
import { UserPaged, UserService } from 'src/app/users/user.service';
import { UserSimple } from 'src/app/users/user-simple';

@Component({
  selector: 'app-toa-thuoc-cu-dialog-save',
  templateUrl: './toa-thuoc-cu-dialog-save.component.html',
  styleUrls: ['./toa-thuoc-cu-dialog-save.component.css']
})
export class ToaThuocCuDialogSaveComponent implements OnInit {
  toaThuocForm: FormGroup;
  id: string;
  limit: number;
  skip: number;
  userSimpleFilter: UserSimple[] = [];
  
  constructor(private fb: FormBuilder, private toaThuocService: ToaThuocService, 
    private userService: UserService) { }

  ngOnInit() {
    this.toaThuocForm = this.fb.group({
      name: null, 
      dateObj: null, 
      note: null, 
      advice: null, 
      userId: null,
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
        console.log(result);
      });
  }

  loadRecord() {
    if (this.id) {
      this.toaThuocService.get(this.id).subscribe(
        result => {
          this.toaThuocForm.patchValue(result);
          let date = new Date(result.date);
          this.toaThuocForm.get('dateObj').patchValue(date);
          // this.lines = result.lines;
        });
    }
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
}
