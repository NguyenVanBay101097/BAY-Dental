import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-facebook-page-marketing-activity-dialog',
  templateUrl: './facebook-page-marketing-activity-dialog.component.html',
  styleUrls: ['./facebook-page-marketing-activity-dialog.component.css']
})
export class FacebookPageMarketingActivityDialogComponent implements OnInit {
  formGroup: FormGroup;
  title: string;
  activity: any;
  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: ['', Validators.required],
      content: null,
      intervalNumber: 1,
      intervalType: 'days'
    });

    if (this.activity) {
      this.formGroup.patchValue(this.activity);
    }
  }

  onSave() {
    if (!this.formGroup.valid) {
      return false;
    }

    var value = this.formGroup.value;
    this.activeModal.close(value);
  }
}
