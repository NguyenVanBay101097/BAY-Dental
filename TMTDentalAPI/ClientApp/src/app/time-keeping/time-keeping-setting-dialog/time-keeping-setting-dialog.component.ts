import { Component, OnInit } from '@angular/core';
import { TimeKeepingService } from '../time-keeping.service';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-time-keeping-setting-dialog',
  templateUrl: './time-keeping-setting-dialog.component.html',
  styleUrls: ['./time-keeping-setting-dialog.component.css']
})
export class TimeKeepingSettingDialogComponent implements OnInit {

  constructor(public timeKeepingService: TimeKeepingService, public fb: FormBuilder, private activeModal: NgbActiveModal,) { }

  settingForm: FormGroup;
  id: string;

  ngOnInit() {
    this.settingForm = this.fb.group({
      oneStandardWorkHour: 0,
      halfStandardWorkHour: 0,
      differenceTime: 0
    });

    setTimeout(() => {
      this.LoadDefaultData();

    });
  }

  get form() {
    return this.settingForm;
  }
  LoadDefaultData() {

    this.timeKeepingService.GetsetupTimeKeeping().subscribe(
      (result: any) => {
        if (result) {
          this.settingForm.patchValue(result);
          this.id = result.id;
        }
      }
    );
  }

  onCancel() {
    this.activeModal.dismiss();
  }

  onSave() {
    const val = this.settingForm.value;
    if (!this.id) {
      this.timeKeepingService.CreateSetupChamcong(val).subscribe(
        res => {
          this.activeModal.dismiss();
        }
      );
    } else {
      this.timeKeepingService.UpdateSetupChamcong(this.id, val).subscribe(
        res => {
          this.activeModal.dismiss();
        }
      );
    }
  }
}

