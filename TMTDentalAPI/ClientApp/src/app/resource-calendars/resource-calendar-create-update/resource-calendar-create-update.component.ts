import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { ResourceCalendarService, ResourceCalendarAttendancePaged, ResourceCalendarAttendanceDisplay, ResourceCalendarDisplay } from '../resource-calendar.service';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ActivatedRoute, Router, Params, ParamMap } from '@angular/router';
import { load, IntlService } from '@progress/kendo-angular-intl';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ResourceCalendarAttendanceCreateUpdateDialogComponent } from '../resource-calendar-attendance-create-update-dialog/resource-calendar-attendance-create-update-dialog.component';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-resource-calendar-create-update',
  templateUrl: './resource-calendar-create-update.component.html',
  styleUrls: ['./resource-calendar-create-update.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class ResourceCalendarCreateUpdateComponent implements OnInit {

  id: string;
  title: string;
  formGroup: FormGroup;
  listRessourceCalendarAttendance: ResourceCalendarAttendanceDisplay[] = [];
  resourceCalendar: ResourceCalendarDisplay = new ResourceCalendarDisplay();
  submitted = false;
  today = new Date();

  constructor(
    private fb: FormBuilder,
    private activeRoute: ActivatedRoute,
    private resourceCalendarService: ResourceCalendarService,
    private router: Router,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private intl: IntlService
  ) {

  }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: [null, Validators.required],
      hoursPerDay: ['', Validators.required],
      attendances: this.fb.array([]),
    });

    this.activeRoute.queryParamMap.subscribe((params: ParamMap) => {
      this.id = params.get('id');
      this.loadData();
    });
  }

  TimeToNumber(time) {
    var hoursMinutes = time.split(/[.:]/);
    var hours = parseInt(hoursMinutes[0], 10);
    var minutes = hoursMinutes[1] ? parseInt(hoursMinutes[1], 10) : 0;
    return hours + minutes / 60;
  }

  NumberToTime(minutes) {
    var sign = minutes < 0 ? "-" : "";
    var min = Math.floor(Math.abs(minutes));
    var sec = Math.floor((Math.abs(minutes) * 60) % 60);
    return sign + (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
  }

  loadData() {
    if (this.id) {
      this.resourceCalendarService.get(this.id).subscribe(
        (result: any) => {
          this.resourceCalendar = result;
          this.formGroup.patchValue(result);

          this.attendances.clear();
          result.attendances.forEach(line => {
            this.attendances.push(this.fb.group({
              id: line.id,
              name: line.name,
              dayOfWeek: line.dayOfWeek,
              hourFrom: new Date(`${this.today.getFullYear()}-${this.today.getMonth()}-${this.today.getDate()} ${this.NumberToTime(line.hourFrom)}`),
              hourTo: new Date(`${this.today.getFullYear()}-${this.today.getMonth()}-${this.today.getDate()} ${this.NumberToTime(line.hourTo)}`),
              dayPeriod: line.dayPeriod,
              calendarId: line.calendarId,
            }));
          });
        });
    }
  }

  getValueForm(key) {
    return this.formGroup.get(key).value;
  }

  get attendances() {
    return this.formGroup.get('attendances') as FormArray;
  }

  loadDefault() {
    this.formGroup.get('name').patchValue(null);
    this.formGroup.get('hoursPerDay').patchValue(0);
  }


  deleteLine(index: number) {
    this.attendances.removeAt(index);
  }

  // drop(event: CdkDragDrop<string[]>) {
  //   moveItemInArray(this.listRessourceCalendarAttendance, event.previousIndex, event.currentIndex);
  //   console.log(this.listRessourceCalendarAttendance);
  //   this.resourceCalendarService.setSequence(this.listRessourceCalendarAttendance).subscribe(
  //     () => {
  //       this.notificationService.show({
  //         content: "Thành công",
  //         hideAfter: 3000,
  //         position: { horizontal: "right", vertical: "bottom" },
  //         animation: { type: "fade", duration: 400 },
  //         type: { style: "success", icon: true },
  //       });
  //     }
  //   );
  // }

  onCreate() {
    this.attendances.push(this.fb.group({
      name: ['', Validators.required],
      dayOfWeek: ['1', Validators.required],
      hourFrom: [null, Validators.required],
      hourTo: [null, Validators.required],
      dayPeriod: ['morning', Validators.required],
    }));
  }

  onSave() {
    this.submitted = true;

    if (this.formGroup.invalid)
      return false;


    var val = Object.assign({}, this.formGroup.value);

    val.resourceCalendarAttendances.forEach(line => {
      var dd = this.intl.formatDate(new Date(line.hourFrom), "HH:mm")
      line.hourFrom = this.TimeToNumber(dd);
      var dm = this.intl.formatDate(new Date(line.hourTo), "HH:mm")
      line.hourTo = this.TimeToNumber(dm);
      line.calendarId = this.id ? this.id : null;
    });


    if (!this.id) {
      this.resourceCalendarService.create(val).subscribe(
        result => {
          this.router.navigate(['/resource-calendars/form'], {
            queryParams: {
              id: result['id']
            },
          });
        }
      )
    } else {
      this.resourceCalendarService.update(this.id, val).subscribe(
        () => {
          this.notificationService.show({
            content: "Cập nhật thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
          this.loadData();
        }
      )
    }
  }

  get f() {
    return this.formGroup.controls;
  }

  createNew() {
    this.router.navigate(['/resource-calendars/form']);
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      hoursPerDay: ['', Validators.required],
      resourceCalendarAttendances: this.fb.array([])
    });
  }
}
