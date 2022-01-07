import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-comment-cu-dialog',
  templateUrl: './comment-cu-dialog.component.html',
  styleUrls: ['./comment-cu-dialog.component.css']
})
export class CommentCuDialogComponent implements OnInit {
  title = "Thêm ghi chú";
  formGroup: FormGroup
  submitted = false;
  onSaveSubj = new Subject();
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    this.formGroup =  this.fb.group({
      body:[null, Validators.required],
    });
   
  }

  Fcontrol(val): FormControl{
    return this.formGroup.get(val) as FormControl;
  }

  onSave(){
    var val = this.formGroup.value;
    this.submitted = true;
    if(this.formGroup.invalid) {
      return;
    }
    this.onSaveSubj.next(val);
  }

  onCancel(){
    this.activeModal.dismiss();
  }

}
