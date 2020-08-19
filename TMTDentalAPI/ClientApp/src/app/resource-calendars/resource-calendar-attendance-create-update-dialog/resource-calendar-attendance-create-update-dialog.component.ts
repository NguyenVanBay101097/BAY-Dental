import { Component, OnInit } from '@angular/core';
import { FormBuilder, Form, FormGroup, Validators } from '@angular/forms';
import { ResourceCalendarService, ResourceCalendarAttendanceDisplay } from '../resource-calendar.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-resource-calendar-attendance-create-update-dialog',
  templateUrl: './resource-calendar-attendance-create-update-dialog.component.html',
  styleUrls: ['./resource-calendar-attendance-create-update-dialog.component.css']
})
export class ResourceCalendarAttendanceCreateUpdateDialogComponent implements OnInit {

  id: string;
  title: string;
  formGroup: FormGroup;
  today = new Date();
  resourceCalendarAtt: ResourceCalendarAttendanceDisplay = new ResourceCalendarAttendanceDisplay();
  formDefault = {
    dayOfWeek: '0',
    dayPeriod: 'morning'
  }
  calendarId: string;

  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private resourceCalendarService: ResourceCalendarService,
    private notificationService: NotificationService,
    private intl: IntlService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      dayOfWeek: ['', Validators.required],
      hourFrom: [null, Validators.required],
      hourTo: [null, Validators.required],
      dayPeriod: ['', Validators.required]
    });

    if (this.id) {
      this.loadData();
    }
    else {
      this.formGroup.patchValue(this.formDefault);
    }
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
    this.resourceCalendarService.getAtt(this.id).subscribe(
      result => {
        this.resourceCalendarAtt = result;
        this.formGroup.get('name').patchValue(this.resourceCalendarAtt.name);
        this.formGroup.get('dayOfWeek').patchValue(this.resourceCalendarAtt.dayOfWeek);
        this.formGroup.get('dayPeriod').patchValue(this.resourceCalendarAtt.dayPeriod);

        var timeTo = this.NumberToTime(this.resourceCalendarAtt.hourTo);
        timeTo = `${this.today.getFullYear()}-${this.today.getMonth()}-${this.today.getDate()} ${timeTo}`;
        this.formGroup.get('hourTo').patchValue(new Date(timeTo));

        var timeFrom = this.NumberToTime(this.resourceCalendarAtt.hourFrom);
        timeFrom = `${this.today.getFullYear()}-${this.today.getMonth()}-${this.today.getDate()} ${timeFrom}`;
        this.formGroup.get('hourFrom').patchValue(new Date(timeFrom));

      }
    )
  }

  onSave() {
    if (this.formGroup.invalid)
      return false;
    var val = this.formGroup.value;
    var dm = this.intl.formatDate(this.formGroup.get('hourTo').value, "HH:mm")
    val.hourTo = this.TimeToNumber(dm);
    var dd = this.intl.formatDate(this.formGroup.get('hourFrom').value, "HH:mm")
    val.hourFrom = this.TimeToNumber(dd);
    val.calendarId = this.calendarId ? this.calendarId : (this.resourceCalendarAtt.calendarId ? this.resourceCalendarAtt.calendarId : null);
    val.sequence = this.resourceCalendarAtt.sequence;
    if (this.id) {
      this.resourceCalendarService.updateAtt(this.id, val).subscribe(
        () => {
          this.activeModal.close(val);
          this.notificationService.show({
            content: "Cập nhật thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
        }
      )
    }
    else {
      this.activeModal.close(val);
      this.notificationService.show({
        content: "Thêm mới thành công",
        hideAfter: 3000,
        position: { horizontal: "center", vertical: "top" },
        animation: { type: "fade", duration: 400 },
        type: { style: "success", icon: true },
      });
    }
  }

}
