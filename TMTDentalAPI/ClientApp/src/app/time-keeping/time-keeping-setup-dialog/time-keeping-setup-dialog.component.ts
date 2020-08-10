import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder } from '@angular/forms';
import { TimeKeepingService, ChamCongBasic, ChamCongSave } from '../time-keeping.service';
import { EmployeeSimple, EmployeeBasic } from 'src/app/employees/employee';

@Component({
  selector: 'app-time-keeping-setup-dialog',
  templateUrl: './time-keeping-setup-dialog.component.html',
  styleUrls: ['./time-keeping-setup-dialog.component.css']
})
export class TimeKeepingSetupDialogComponent implements OnInit {

  formGroup: FormGroup;
  id: string;
  dateTime: Date;
  timeIn: any;
  today: Date = new Date();
  timeOut: any;
  employee: EmployeeBasic;
  chamCong: ChamCongBasic = new ChamCongBasic();
  constructor(
    private activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private timeKeepingServive: TimeKeepingService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      timeIn: false,
      timeOut: false
    })

    if (this.id) {
      this.loadFormApi();
    }
    if (this.today.getDate() > this.dateTime.getDate()) {
      this.formGroup.disable();
    }
  }

  loadFormApi() {
    this.timeKeepingServive.get(this.id).subscribe(
      result => {
        this.chamCong = result;
        if (this.chamCong && this.chamCong.timeIn) {
          this.formGroup.get('timeIn').setValue(true);
          this.timeIn = this.chamCong.timeIn
        }
        if (this.chamCong && this.chamCong.timeOut) {
          this.formGroup.get('timeOut').setValue(true);
          this.timeOut = this.chamCong.timeOut
        }
      }
    )
  }

  checkTimeIn(evt) {
    if (evt)
      this.timeIn = new Date(new Date);
    else
      this.timeIn = null;
  }

  checkTimeOut(evt) {
    if (evt)
      this.timeOut = new Date(new Date);
    else
      this.timeOut = null;
  }

  onSave() {
    if (!this.employee)
      return false;

    var val = new ChamCongSave();
    val.timeIn = this.timeIn ? this.timeIn : null;
    val.timeOut = this.timeOut ? this.timeOut : null
    val.employeeId = this.employee.id;
    if (!val.timeOut && !val.timeIn) {
      return false;
    }
    if (this.id) {
      this.timeKeepingServive.update(this.id, val).subscribe(
        x => {
          this.activeModal.close();
        }
      )
    } else if (this.today.getDate() == this.dateTime.getDate()) {
      this.timeKeepingServive.create(val).subscribe(
        result => {
          this.activeModal.close();
        }
      )
    } else {
      this.activeModal.close();
    }
  }

}
