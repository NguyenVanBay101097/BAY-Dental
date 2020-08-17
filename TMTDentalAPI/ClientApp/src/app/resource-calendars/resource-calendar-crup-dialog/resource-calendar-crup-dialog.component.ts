import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ResourceCalendarService, ResourceCalendarDisplay, ResourceCalendarAttendanceDisplay } from '../resource-calendar.service';

@Component({
  selector: 'app-resource-calendar-crup-dialog',
  templateUrl: './resource-calendar-crup-dialog.component.html',
  styleUrls: ['./resource-calendar-crup-dialog.component.css']
})
export class ResourceCalendarCrupDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  listResourceCalendarAttendanceDisplay: ResourceCalendarAttendanceDisplay[] = [];
  resourceCalendarDisplay: ResourceCalendarDisplay = new ResourceCalendarDisplay();
  id: string;
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private resourceCalendarService: ResourceCalendarService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      hoursPerDay: ['', Validators.required]
    });
  }

  loadFormApi() {
    this.resourceCalendarService.get(this.id).subscribe(
      result => {
        this.resourceCalendarDisplay = result;
        this.formGroup.patchValue(this.resourceCalendarDisplay);
        if (this.resourceCalendarDisplay.resourceCalendarAttendances)
          this.listResourceCalendarAttendanceDisplay = this.resourceCalendarDisplay.resourceCalendarAttendances;
      }
    )
  }

  onSave() {

  }

}
