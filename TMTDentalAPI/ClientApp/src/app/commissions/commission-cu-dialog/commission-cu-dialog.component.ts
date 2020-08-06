import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommissionService } from '../commission.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { result } from 'lodash';

@Component({
  selector: 'app-commission-cu-dialog',
  templateUrl: './commission-cu-dialog.component.html',
  styleUrls: ['./commission-cu-dialog.component.css']
})
export class CommissionCuDialogComponent implements OnInit {
  formGroup: FormGroup;

  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;

  title: string;
  submitted = false;
  
  constructor(private fb: FormBuilder, 
    private commissionService: CommissionService,
    public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required]
    });
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    var val = this.formGroup.value;
    this.commissionService.create(val)
    .subscribe(result => {
      this.activeModal.close(result);
    }, err => {
      console.log(err);
    });
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.close();
  }

  get f() {
    return this.formGroup.controls;
  }
}
