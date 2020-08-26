import { Component, OnInit, ViewChild } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Subject } from 'rxjs';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HrPayrollStructureTypeService, HrPayrollStructureTypeDisplay, HrPayrollStructureTypeSave } from '../hr-payroll-structure-type.service';
import { ResourceCalendarService, ResourceCalendarPaged, ResourceCalendarBasic } from 'src/app/resource-calendars/resource-calendar.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { TimeKeepingService } from 'src/app/time-keeping/time-keeping.service';
import { NotificationService } from '@progress/kendo-angular-notification';
import { WorkEntryTypeBasic, WorkEntryTypeService } from 'src/app/work-entry-types/work-entry-type.service';

@Component({
  selector: 'app-hr-payroll-structure-type-create',
  templateUrl: './hr-payroll-structure-type-create.component.html',
  styleUrls: ['./hr-payroll-structure-type-create.component.css']
})
export class HrPayrollStructureTypeCreateComponent implements OnInit {

  @ViewChild('calendarCbx', { static: true }) private CalendarCbx: ComboBoxComponent;
  @ViewChild('workEntryTypeCbx', { static: true }) private workEntryTypeCbx: ComboBoxComponent;
  formGroup: FormGroup;
  title: string;
  id: string;
  filterResourceCalendars: ResourceCalendarBasic[] = [];
  filterResourceWorkEntryTypes: WorkEntryTypeBasic[] = [];
  hrPayrollStructureDisplay: HrPayrollStructureTypeDisplay = new HrPayrollStructureTypeDisplay();
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private hrPayrollStructureTypeService: HrPayrollStructureTypeService,
    private resourceCalendarService: ResourceCalendarService,
    private timeKeepingService: TimeKeepingService,
    private notificationService: NotificationService,
    private workEntryTypeService : WorkEntryTypeService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      wageType: ['monthly', Validators.required],
      defaultSchedulePay: ['monthly', Validators.required],
      defaultResourceCalendarId: [null, Validators.required],
      defaultWorkEntryTypeId: [null, Validators.required]
    });

    this.filterCbx();
    this.loadWorkEntryType();
    this.loadCalendar();
    if (this.id) {
      this.loadData();
    }
  }

  filterCbx() {
    this.CalendarCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.CalendarCbx.loading = true)),
      switchMap(value => this.searchCalendar(value))
    ).subscribe(result => {
      this.filterResourceCalendars = result.items;
      this.CalendarCbx.loading = false;
    });

    this.workEntryTypeCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.workEntryTypeCbx.loading = true)),
      switchMap(value => this.searchWorkEntryType(value))
    ).subscribe(result => {
      this.filterResourceWorkEntryTypes = result.items;
      this.workEntryTypeCbx.loading = false;
    });
  }

  loadData() {
    this.hrPayrollStructureTypeService.getId(this.id).subscribe(
      result => {
        this.hrPayrollStructureDisplay = result;
        this.formGroup.patchValue(this.hrPayrollStructureDisplay);
      }
    )
  }

  loadCalendar() {
    this.searchCalendar().subscribe(
      result => {
        this.filterResourceCalendars = result.items;
      }
    )
  }

  searchCalendar(q?: string) {
    var val = new ResourceCalendarPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = q ? q : '';
    return this.resourceCalendarService.getPage(val);
  }

  loadWorkEntryType() {
    this.searchWorkEntryType().subscribe(
      result => {
        this.filterResourceWorkEntryTypes = result.items;
      }
    )
  }

  searchWorkEntryType(q?: string) {
    var val = new ResourceCalendarPaged();
    val.limit = 20;
    val.offset = 0;
    val.search = q ? q : '';
    return this.workEntryTypeService.getPaged(val);
  }

  onSave() {
    if (this.formGroup.invalid)
      return false;

    var val = new HrPayrollStructureTypeSave();
    val = this.formGroup.value;
    if (this.id) {
      this.hrPayrollStructureTypeService.update(this.id, val).subscribe(
        () => {
          this.notificationService.show({
            content: "Cập nhật thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          this.activeModal.close(true);
        }
      )
    } else {
      this.hrPayrollStructureTypeService.create(val).subscribe(
        () => {
          this.notificationService.show({
            content: "Thêm mới thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          this.activeModal.close(true);
        }
      )
    }
  }
}
