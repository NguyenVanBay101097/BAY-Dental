import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { TimeKeepingService, ChamCongBasic, ChamCongSave, WorkEntryType, WorkEntryTypePage } from '../time-keeping.service';
import { EmployeeSimple, EmployeeBasic } from 'src/app/employees/employee';
import { IntlService } from '@progress/kendo-angular-intl';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { offset } from '@progress/kendo-date-math';

@Component({
  selector: 'app-time-keeping-setup-dialog',
  templateUrl: './time-keeping-setup-dialog.component.html',
  styleUrls: ['./time-keeping-setup-dialog.component.css']
})
export class TimeKeepingSetupDialogComponent implements OnInit {

  @ViewChild('workCbx', { static: true }) workCbx: ComboBoxComponent
  formGroup: FormGroup;
  id: string;
  dateTime: Date;
  timeIn: any;
  today: Date = new Date();
  timeOut: any;
  employee: EmployeeBasic;
  filterdWorks: WorkEntryType[] = [];
  chamCong: ChamCongBasic = new ChamCongBasic();
  constructor(
    private activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private timeKeepingServive: TimeKeepingService,
    private intl: IntlService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      timeIn: false,
      timeOut: false,
      workEntryTypeId: [null, Validators.required]
    })


    if (this.id) {
      this.loadFormApi();
    }
    // if (this.today.getDate() > this.dateTime.getDate()) {
    //   this.formGroup.disable();
    // }
    this.loadWorkEntryType();
    this.workCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.workCbx.loading = true)),
      switchMap(value => this.searchWorkEntryType(value))
    ).subscribe(result => {
      this.filterdWorks = result.items;
      this.workCbx.loading = false;
    });

  }

  loadWorkEntryType() {
    this.searchWorkEntryType().subscribe(
      result => {
        this.filterdWorks = result.items;
        if (this.filterdWorks && this.filterdWorks.length > 0)
          this.formGroup.get('workEntryTypeId').patchValue(this.filterdWorks[0].id);
      }
    )
  }

  searchWorkEntryType(search?: string) {
    var page = new WorkEntryTypePage();
    page.limit = 20;
    page.offset = 0;
    page.filter = search ? search : '';
    return this.timeKeepingServive.getPagedWorkEntryType(page);
  }

  loadFormApi() {
    this.timeKeepingServive.get(this.id).subscribe(
      result => {
        this.chamCong = result;
        this.formGroup.get('workEntryTypeId').patchValue(this.chamCong.workEntryTypeId);
        if (this.chamCong && this.chamCong.timeIn) {
          this.formGroup.get('timeIn').setValue(new Date(this.chamCong.timeIn));
          this.timeIn = new Date(this.chamCong.timeIn);
        }
        if (this.chamCong && this.chamCong.timeOut) {
          this.formGroup.get('timeOut').setValue(new Date(this.chamCong.timeOut));
          this.timeOut = new Date(this.chamCong.timeOut);
        }
      }
    )
  }

  changeTimeIn(time: Date) {
    if (time)
      this.timeIn = new Date(this.dateTime.getFullYear(), this.dateTime.getMonth(), this.dateTime.getDate(), time.getHours(), time.getMinutes());
    else
      this.timeIn = null;
  }

  changeTimeOut(time: Date) {
    if (time)
      this.timeOut = new Date(this.dateTime.getFullYear(), this.dateTime.getMonth(), this.dateTime.getDate(), time.getHours(), time.getMinutes());
    else
      this.timeOut = null;
  }

  onSave() {
    if (!this.employee)
      return false;

    var val = new ChamCongSave();
    val.timeIn = this.timeIn ? this.intl.formatDate(this.timeIn, "yyyy-MM-ddTHH:mm") : '';
    val.timeOut = this.timeOut ? this.intl.formatDate(this.timeOut, "yyyy-MM-ddTHH:mm") : '';
    val.employeeId = this.employee.id;
    val.date = this.intl.formatDate(this.dateTime, "yyyy-MM-dd");
    val.workEntryTypeId = this.formGroup.get('workEntryTypeId').value;
    if (this.id) {
      this.timeKeepingServive.update(this.id, val).subscribe(
        x => {
          this.activeModal.close();
        }
      )
    }
    else {
      this.timeKeepingServive.create(val).subscribe(
        result => {
          this.activeModal.close();
        }
      )
    }
    // else if (this.today.getDate() == this.dateTime.getDate()) {
    //   this.timeKeepingServive.create(val).subscribe(
    //     result => {
    //       this.activeModal.close();
    //     }
    //   )
    // } else {
    //   this.activeModal.close();
    // }
  }

}
