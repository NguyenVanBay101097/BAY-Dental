import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { ResInsuranceService } from 'src/app/res-insurance/res-insurance.service';

@Component({
  selector: 'app-res-insurance-cu-dialog',
  templateUrl: './res-insurance-cu-dialog.component.html',
  styleUrls: ['./res-insurance-cu-dialog.component.css']
})
export class ResInsuranceCuDialogComponent implements OnInit {
  @Input() title: string;
  @Input() id: string;
  formGroup: FormGroup;
  submitted: boolean = false;
  get f() { return this.formGroup.controls; }

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private resInsuranceService: ResInsuranceService
  ) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      dateObj: null,
      avatar: '',
      representative: '',
      phone: '',
      email: '',
      address: '',
      note: '',
    });
    // this.formGroup.get('dateObj').patchValue(new Date());

    if (this.id) {
      this.loadRecord(this.id);
    }
  }

  loadRecord(id: string) {
    this.resInsuranceService.getById(id).subscribe((res: any) => {
      this.formGroup.patchValue(res);
      if(res.date){
        let date = new Date(res.date);
        this.formGroup.get('dateObj').patchValue(date);
      }
     
    })
  }

  onSave() {
    this.submitted = true;
    
    if (!this.formGroup.valid) {
      return false;
    }

    let val = this.formGroup.value;
    val.date = val.dateObj ? moment(val.dateObj).format('YYYY-MM-DD') : null;
    if (this.id) {
      this.resInsuranceService.update(this.id, val).subscribe((res: any) => {
        this.activeModal.close(res);
      });
    } else {
      this.resInsuranceService.create(val).subscribe((res: any) => {
        this.activeModal.close(res);
      });
    }
  }

  onAvatarUploaded(data) {
    this.f.avatar.setValue(data ? data.fileUrl : null);
  }

  onCancel(): void {
    this.activeModal.dismiss()
  }

}
