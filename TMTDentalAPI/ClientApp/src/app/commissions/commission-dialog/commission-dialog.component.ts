import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CommissionService } from '../commission.service';

@Component({
  selector: 'app-commission-dialog',
  templateUrl: './commission-dialog.component.html',
  styleUrls: ['./commission-dialog.component.css']
})
export class CommissionDialogComponent implements OnInit {
  myform: FormGroup;
  @Input() id: string;
  title: string;
  submitted = false;

  get f() { return this.myform.controls; }
  
  constructor(
    private fb: FormBuilder,
    private commissionService: CommissionService,
    public activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
    this.myform = this.fb.group({
      name: ["", Validators.required]
    });

    if (this.id) {
      setTimeout(() => {
        this.commissionService.get(this.id).subscribe((result) => {
          this.myform.patchValue(result);
        });
      });
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.myform.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(
      (result) => {
        if (result) {
          this.activeModal.close(result);
        } else {
          this.activeModal.close(true);
        }
      }, err => {
        console.log(err);
        this.submitted = false;
      });
  }

  saveOrUpdate() {
    var val = this.myform.value;
    if (!this.id) {
      return this.commissionService.create(val);
    } else {
      return this.commissionService.update(this.id, val);
    }
  }
}
