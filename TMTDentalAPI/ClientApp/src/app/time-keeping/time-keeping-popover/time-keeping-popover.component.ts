import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbModal, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { WorkEntryTypeService } from 'src/app/work-entry-types/work-entry-type.service';
import { TimeKeepingService } from '../time-keeping.service';

@Component({
  selector: 'app-time-keeping-popover',
  templateUrl: './time-keeping-popover.component.html',
  styleUrls: ['./time-keeping-popover.component.css']
})
export class TimeKeepingPopoverComponent implements OnInit {
  formGroup: FormGroup
  title: 'Thêm chấm công';
  @Input() line: any;
  @Output() formDate = new EventEmitter<any>();
  @Output() removeTime = new EventEmitter<any>();

  @ViewChild('popOver', { static: true }) public popover: NgbPopover;

  constructor(
    private fb: FormBuilder,
    private timeKeepingServive: TimeKeepingService,
    private intl: IntlService,
    private workEntryTypeService: WorkEntryTypeService,
    private notificationService: NotificationService,
    private showErrorService: AppSharedShowErrorService,
    private modalService: NgbModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      type: [null, Validators.required],
      overTime: false,
      overTimeHourType: null,
      overTimeHour: null,
      id: null,
      companyId: null,
    });

    this.reLoad();
  }

  reLoad() {
    if (this.line) {
      if (this.line.chamCongs.length > 0) {
        var res = this.line.chamCongs[0];
        this.formGroup = this.fb.group({
          type: res.type,
          overTime: res.overTime,
          overTimeHour: res.overTimeHour,
          overTimeHourType: res.overTimeHour <= 4 ? res.overTimeHour : 'orther',
          id: res.id,
          companyId: res.companyId,
        });
      }
      else {
        this.formGroup = this.fb.group({
          type: 'work',
          overTime: false,
          overTimeHour: null,
          overTimeHourType: null,
        });
      }
    }
  }

  onSave() {
    var res = this.formGroup.value;
    res.type = res.type;
    res.overTime = res.overTime;
    res.overTimeHour = res.overTimeHourType === 'orther' ? res.overTimeHour : parseInt(res.overTimeHourType);
    res.id = res.id ? res.id : null;
    this.formDate.emit(res);
    this.popover.close();
  }

  onRemoveTimeKeeping(){
    debugger
    var res = this.formGroup.value;
    this.removeTime.emit(res);
    this.popover.close();
  }

  onChangeType() {
    var res = this.formGroup.value;
    if (res.type !== 'work') {
      res.overTime = false;
      res.overTimeHour = null;
      res.overTimeHourType = null;
    }

    this.formGroup.patchValue(res);
  }

  onChangeOverTimeHourType() {
    var res = this.formGroup.value;
    if (this.getOverTimeHourType !== 'orther') {
      res.overTimeHour = null;
    }else{
      res.overTimeHour = 0;
    }
    
    this.formGroup.patchValue(res);
  }

  getIntValue(value){
    return parseInt(value);
  }

  changeCheckBox(value) {
    var res = this.formGroup.value;
    res.overTime = value;
    res.overTimeHour = null;
    res.overTimeHourType = null;
    this.formGroup.patchValue(res);
  }

  get getType() {
    if (this.line.chamCongs.length > 0) {
      var res = this.line.chamCongs[0];
      var type = res.type;
    }
    return type;
  }

  get getFormType() {
    return this.formGroup.get('type').value;
  }

  get overtime() {
    return this.formGroup.get('overTime').value;
  }

  get getOverTimeHour() {
    return this.formGroup.get('overTimeHour').value;
  }

  get getOverTimeHourType() {
    return this.formGroup.get('overTimeHourType').value;
  }

  getTimeType(value) {
    switch (value) {
      case "work":
        return "W";
      case "halfaday":
        return "W/2";
      case "off":
        return "O";
    }
  }

  toggleWith(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      this.formGroup.reset();
      this.reLoad();
      popover.open();
    }
  }


}
