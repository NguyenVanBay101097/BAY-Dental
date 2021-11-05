import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import * as _ from 'lodash';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { EmployeeBasic } from 'src/app/employees/employee';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { WorkEntryType, WorkEntryTypePage, WorkEntryTypeService } from 'src/app/work-entry-types/work-entry-type.service';
import { ChamCongBasic, TimeKeepingService } from '../time-keeping.service';

@Component({
  selector: 'app-time-keeping-setup-dialog',
  templateUrl: './time-keeping-setup-dialog.component.html',
  styleUrls: ['./time-keeping-setup-dialog.component.css']
})
export class TimeKeepingSetupDialogComponent implements OnInit {

  @ViewChild('workCbx', { static: true }) workCbx: ComboBoxComponent;
  @ViewChild('timeOutRef') timeOutRef: ElementRef;
  formGroup: FormGroup;
  id: string;
  dateTime: Date;
  timeIn: any;
  today: Date = new Date();
  timeOut: any;
  title: string="";
  employee: EmployeeBasic;
  filterdWorks: WorkEntryType[] = [];
  chamCong: ChamCongBasic = new ChamCongBasic();
  error = false;
  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private timeKeepingServive: TimeKeepingService,
    private intl: IntlService,
    private workEntryTypeService: WorkEntryTypeService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      timeInObj: [null, Validators.required],
      timeOutObj: null,
      workEntryTypeId: [null, Validators.required],
      companyId: null,
      employeeId: null
    });

    setTimeout(() => {
      if (this.id) {
        this.loadFormApi();
      } else {
        this.loadDefault();
      }
  
      this.loadWorkEntryType();

      this.workCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.workCbx.loading = true)),
        switchMap(value => this.searchWorkEntryType(value))
      ).subscribe(result => {
        this.filterdWorks = result.items;
        this.workCbx.loading = false;
      });
    });
  }

  loadDefault() {
    var val = { employeeId: this.employee.id };
    this.timeKeepingServive.defaultGet(val).subscribe((result: any) => {
      this.formGroup.patchValue(result);
      if (result.workEntryType) {
        this.filterdWorks = _.unionBy(this.filterdWorks, [result.workEntryType], 'id');
      }
    });
  }

  loadWorkEntryType() {
    this.searchWorkEntryType().subscribe(
      result => {
        this.filterdWorks = _.unionBy(this.filterdWorks, result.items, 'id');
      }
    )
  }

  searchWorkEntryType(search?: string) {
    var page = new WorkEntryTypePage();
    page.limit = 20;
    page.offset = 0;
    page.filter = search ? search : '';
    return this.workEntryTypeService.getPaged(page);
  }

  loadFormApi() {
    this.timeKeepingServive.get(this.id).subscribe(
      result => {
        this.chamCong = result;
        this.formGroup.patchValue(result);
        this.formGroup.get('timeInObj').setValue(result.timeIn ? new Date(result.timeIn) : null);
        this.formGroup.get('timeOutObj').setValue(result.timeOut ? new Date(result.timeOut) : null);
        this.dateTime = new Date(this.intl.formatDate(new Date(result.timeIn), "yyyy-MM-dd"));
      }
    )
  }

  delete(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.body = `Bạn chắc chắn muốn xóa chấm công vào ${this.intl.formatDate(new Date(this.chamCong.timeIn), "EEEE dd/MM/yyyy")} của nhân viên ${this.employee.name}`;
    modalRef.componentInstance.title = "Xóa chấm công";
    modalRef.result.then(() => {
      this.timeKeepingServive.deleteChamCong(item.id).subscribe(
        () => {
          this.activeModal.close(this.employee);
        }
      )
    });
  }

  onSave() {
    if (this.formGroup.invalid) {
      return false;
    }
    debugger;
    var val = Object.assign({}, this.formGroup.value);
    var timeIn = new Date(`${this.intl.formatDate(this.dateTime, "yyyy-MM-dd")} ${this.intl.formatDate(val.timeInObj, "HH:mm:ss")}`);
    var timeOut = val.timeOutObj ? new Date(`${this.intl.formatDate(this.dateTime, "yyyy-MM-dd")} ${this.intl.formatDate(val.timeOutObj, "HH:mm")}`) : null;

    val.timeIn = this.intl.formatDate(timeIn, "yyyy-MM-ddTHH:mm");
    val.timeOut = timeOut ? this.intl.formatDate(timeOut, "yyyy-MM-ddTHH:mm") : null;

    if (this.id) {
      this.timeKeepingServive.update(this.id, val).subscribe(
        x => {
          this.activeModal.close(this.employee);
        }, err => {
        }
      )
    }
    else {
      this.timeKeepingServive.create(val).subscribe(
        result => {
          this.activeModal.close(this.employee);
        }, err => {
        }
      )
    }
  }

}
