import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-time-keeping-setup-dialog',
  templateUrl: './time-keeping-setup-dialog.component.html',
  styleUrls: ['./time-keeping-setup-dialog.component.css']
})
export class TimeKeepingSetupDialogComponent implements OnInit {

  formGroup: FormGroup;
  employeeId: string;
  dataTime: Date;
  constructor(
    private activeModal: NgbActiveModal,
    private fb: FormBuilder,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({

    })
  }

  loadFormApi() {
    
  }

  onSave() {

  }

}
