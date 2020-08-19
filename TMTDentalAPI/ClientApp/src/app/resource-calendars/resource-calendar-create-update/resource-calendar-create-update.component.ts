import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ResourceCalendarService, ResourceCalendarAttendancePaged, ResourceCalendarAttendanceDisplay, ResourceCalendarDisplay } from '../resource-calendar.service';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NotificationService } from '@progress/kendo-angular-notification';
import { ActivatedRoute, Router } from '@angular/router';
import { load } from '@progress/kendo-angular-intl';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ResourceCalendarAttendanceCreateUpdateDialogComponent } from '../resource-calendar-attendance-create-update-dialog/resource-calendar-attendance-create-update-dialog.component';

@Component({
  selector: 'app-resource-calendar-create-update',
  templateUrl: './resource-calendar-create-update.component.html',
  styleUrls: ['./resource-calendar-create-update.component.css']
})
export class ResourceCalendarCreateUpdateComponent implements OnInit {

  id: string;
  title: string;
  formGroup: FormGroup;
  listRessourceCalendarAttendance: ResourceCalendarAttendanceDisplay[] = [];
  resourceCalendar: ResourceCalendarDisplay = new ResourceCalendarDisplay();

  constructor(
    private fb: FormBuilder,
    private activeRoute: ActivatedRoute,
    private resourceCalendarService: ResourceCalendarService,
    private router: Router,
    private modalService: NgbModal,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      hoursPerDay: [0, Validators.required]
    });
    this.id = this.activeRoute.queryParams['_value'].id;
    if (this.id)
      this.loadData();
    else {
      this.loadDefault();
    }
  }

  minTommss(minutes) {
    var sign = minutes < 0 ? "-" : "";
    var min = Math.floor(Math.abs(minutes));
    var sec = Math.floor((Math.abs(minutes) * 60) % 60);
    return sign + (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
  }

  loadData() {
    this.resourceCalendarService.get(this.id).subscribe(
      result => {
        this.resourceCalendar = result;
        this.formGroup.patchValue(this.resourceCalendar);
        this.listRessourceCalendarAttendance = this.resourceCalendar.resourceCalendarAttendances;
      }
    )
  }

  loadDefault() {
    this.formGroup.get('name').patchValue(null);
    this.formGroup.get('hoursPerDay').patchValue(0);
    this.loadCalendarAttendance();
  }

  loadCalendarAttendance(id?: any) {
    var paged = new ResourceCalendarAttendancePaged();
    paged.limit = 20;
    paged.offset = 0;
    this.resourceCalendarService.GetListResourceCalendadrAtt(paged).subscribe(
      result => {
        this.listRessourceCalendarAttendance = result;
      }
    )
  }

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.listRessourceCalendarAttendance, event.previousIndex, event.currentIndex);
    console.log(this.listRessourceCalendarAttendance);
    this.resourceCalendarService.setSequence(this.listRessourceCalendarAttendance).subscribe(
      () => {
        this.notificationService.show({
          content: "Thành công",
          hideAfter: 3000,
          position: { horizontal: "right", vertical: "bottom" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });
      }
    );
  }

  removeAtt(item) {
    var index = this.listRessourceCalendarAttendance.findIndex(x => x.id == item.id);
    if (index >= 0) {
      this.listRessourceCalendarAttendance.splice(index, 1);
    }
  }

  addAtt() {
    let modalRef = this.modalService.open(ResourceCalendarAttendanceCreateUpdateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm: Giờ làm việc';
    modalRef.componentInstance.calendarId = this.id ? this.id : null;
    modalRef.result.then(result => {
      if (!this.listRessourceCalendarAttendance)
        this.listRessourceCalendarAttendance = [];
      this.listRessourceCalendarAttendance.push(result);
    });
  }

  editAtt(item) {
    let modalRef = this.modalService.open(ResourceCalendarAttendanceCreateUpdateDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: Giờ làm việc';
    modalRef.componentInstance.calendarId = this.id ? this.id : null;
    modalRef.componentInstance.id = item.id;
    modalRef.result.then((result: ResourceCalendarAttendanceDisplay) => {
      this.loadData();
    });
  }

  removeAll() {
    this.listRessourceCalendarAttendance = [];
  }

  onNew() {
    this.router.navigateByUrl('resource-calendars/form')
    this.loadDefault();
  }
  onSave() {
    if (this.formGroup.invalid)
      return;

    var val = this.formGroup.value;
    val.resourceCalendarAttendances = this.listRessourceCalendarAttendance;
    if (!this.id) {
      this.resourceCalendarService.create(val).subscribe(
        result => {
          this.id = result.id;
          this.router.navigateByUrl('resource-calendars');
          this.notificationService.show({
            content: "Thêm mới thành công",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
        }
      )
    } else {
      this.resourceCalendarService.update(this.id, val).subscribe(
        () => {
          this.router.navigateByUrl('resource-calendars');
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
  }



}
