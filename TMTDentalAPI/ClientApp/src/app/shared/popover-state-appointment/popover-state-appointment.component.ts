import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Validators } from '@angular/forms';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { AppointmentService } from 'src/app/appointment/appointment.service';

@Component({
  selector: 'app-popover-state-appointment',
  templateUrl: './popover-state-appointment.component.html',
  styleUrls: ['./popover-state-appointment.component.css']
})
export class PopoverStateAppointmentComponent implements OnInit {
  @Output() stateFormGroup = new EventEmitter<any>();
  @Input() item: any;
  submitted = false;
  @ViewChild('popOver', { static: true }) public popover: NgbPopover;
  formGroup: FormGroup;
  stateFilterOptions: any[] = [
    {text:'Đang hẹn' , value:'confirmed'},
    {text:'Chờ khám' , value:'wait'},
    {text:'Đang khám' , value:'examination'},
    {text:'Hoàn thành' , value:'done'},
    {text:'Hủy hẹn' , value:'cancel'}
  ];
  constructor(private fb: FormBuilder,private appointmentService: AppointmentService) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      state: null,
      reason: null
    });
    this.reLoad();
  }

  reLoad() {
    if (this.item) {
      this.formGroup.get('state').setValue(this.item.state);
      if(this.item.state == 'cancel'){
        this.formGroup.get('reason').setValue(this.item.reason);
      }
    }
  }

  get stateControl() {
    return this.formGroup.get('state').value;
  }

  get reasonControl() {
    return this.formGroup.get('reason').value;
  }

  get f() { return this.formGroup.controls; }

  onChangeState(){
    if(this.stateControl == 'cancel'){
      this.formGroup.get("reason").setValidators([Validators.minLength(0), Validators.required]);
      this.formGroup.get("reason").updateValueAndValidity();
    }else{
      this.formGroup.get('reason').clearValidators();
      this.formGroup.get('reason').updateValueAndValidity();
    }
  }

  togglePopover(popOver) {
    if (popOver.isOpen()) {
      popOver.close();
    } else {
      this.formGroup.reset();
      this.reLoad();
      popOver.open();
    }
  }

  onSave() {
    this.submitted = true;
    if (!this.formGroup.valid) {
      return;
    }

    const val = this.formGroup.value;  
    this.stateFormGroup.emit(val);
    this.popover.close();
  }

}
