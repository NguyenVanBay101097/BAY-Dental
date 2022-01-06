import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { CommentService } from '../comment.service';

@Component({
  selector: 'app-comment-cu-dialog',
  templateUrl: './comment-cu-dialog.component.html',
  styleUrls: ['./comment-cu-dialog.component.css']
})
export class CommentCuDialogComponent implements OnInit {
  title = "Thêm ghi chú";
  @Input() value: any;
  formGroup: FormGroup
  submitted = false;
  constructor(
    private commentService: CommentService,
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private notifyService: NotifyService
  ) { }

  ngOnInit(): void {
    this.formGroup =  this.fb.group({
      body:[null, Validators.required],
      threadId:null,
      threadModel:null
    });
    if(this.value){
      this.formGroup.patchValue(this.value);
    }
  }

  Fcontrol(val): FormControl{
    return this.formGroup.get(val) as FormControl;
  }

  onSave(){
    var val = this.formGroup.value;
    this.commentService.create(val).subscribe((res:any) => {
      this.notifyService.notify("success", "Lưu thành công");
      this.activeModal.close(res);
    });
  }

  onCancel(){
    this.activeModal.dismiss();
  }

}
