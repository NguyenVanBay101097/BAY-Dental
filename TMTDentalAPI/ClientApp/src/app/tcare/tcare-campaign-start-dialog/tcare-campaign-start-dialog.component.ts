import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tcare-campaign-start-dialog',
  templateUrl: './tcare-campaign-start-dialog.component.html',
  styleUrls: ['./tcare-campaign-start-dialog.component.css']
})
export class TcareCampaignStartDialogComponent implements OnInit {
  formGroup: FormGroup;
  submited = false;
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      sheduleStart: [null, Validators.required]
    })
  }

  get sheduleStartControl() { return this.formGroup.get('sheduleStart') }

  onSave() {
    this.submited = true;
    if (this.formGroup.invalid)
      return false;

    var value = this.formGroup.value;
    this.activeModal.close(value);
  }

}
