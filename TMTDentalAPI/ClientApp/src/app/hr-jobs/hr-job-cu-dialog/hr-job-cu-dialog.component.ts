import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from 'src/app/auth/auth.service';
import { NotifyService } from 'src/app/shared/services/notify.service';
import { HrJobService } from '../hr-job.service';

@Component({
  selector: 'app-hr-job-cu-dialog',
  templateUrl: './hr-job-cu-dialog.component.html',
  styleUrls: ['./hr-job-cu-dialog.component.css']
})
export class HrJobCuDialogComponent implements OnInit {
  @Input() title: string;
  @Input() id: string;
  formGroup: FormGroup;
  submitted: boolean = false;
  invalid: boolean = false;

  get f() { return this.formGroup.controls; }

  constructor(
    private authService: AuthService,
    private hrJobService: HrJobService,
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private notifyService: NotifyService,
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
    });
    if (this.id) {
      this.hrJobService.get(this.id).subscribe((result: any) => {
        this.formGroup.patchValue(result);
      });
    }

    this.formGroup.controls['name'].valueChanges.subscribe(
      (selectedValue) => {
        this.invalid = selectedValue ? false : true;
      }
    );
  }

  getValueFC(key: string) {
    return this.formGroup.get(key).value;
  }

  onSave() {
    this.submitted = true;
    this.invalid = this.getValueFC('name') ? false : true;

    if (!this.formGroup.valid) {
      return;
    }

    let val = this.formGroup.value;
    val.companyId = this.authService.userInfo.companyId;

    if (this.id) {
      this.hrJobService.update(this.id, val).subscribe(() => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.activeModal.close(true);
      });
    } else {
      this.hrJobService.create(val).subscribe(result => {
        this.notifyService.notify('success', 'Lưu thành công');
        this.activeModal.close(result);
      });
    }
  }

}
