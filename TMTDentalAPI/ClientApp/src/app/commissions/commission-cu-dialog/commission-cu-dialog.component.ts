import { Component, OnInit, ViewChild, ElementRef, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { CommissionService } from '../commission.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-commission-cu-dialog',
  templateUrl: './commission-cu-dialog.component.html',
  styleUrls: ['./commission-cu-dialog.component.css']
})
export class CommissionCuDialogComponent implements OnInit {
  formGroup: FormGroup;

  @ViewChild('nameInput', { static: true }) nameInput: ElementRef;

  @Input() public id: string;
  title: string;
  submitted = false;
  
  constructor(private fb: FormBuilder, 
    private commissionService: CommissionService,
    public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
    });

    if (this.id) {
      setTimeout(() => {
        this.commissionService.get(this.id).subscribe((result) => {
          this.formGroup.patchValue(result);
        });
      });
    }
  }

  onSave() {
    this.submitted = true;

    if (!this.formGroup.valid) {
      return;
    }

    this.saveOrUpdate().subscribe(result => {
      if (result) {
        this.activeModal.close(result);
      } else {
        this.activeModal.close(true);
      }
    }, err => {
      console.log(err);
    });
  }

  saveOrUpdate() {
    var val = this.formGroup.value;
    val.parentId = val.parent ? val.parent.id : null;
    if (!this.id) {
      return this.commissionService.create(val);
    } else {
      return this.commissionService.update(this.id, val);
    }
  }

  onCancel() {
    this.submitted = false;
    this.activeModal.close();
  }

  get f() {
    return this.formGroup.controls;
  }
}
