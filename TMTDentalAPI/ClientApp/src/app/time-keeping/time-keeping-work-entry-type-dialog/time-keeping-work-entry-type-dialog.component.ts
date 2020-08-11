import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TimeKeepingService, WorkEntryType } from '../time-keeping.service';

@Component({
  selector: 'app-time-keeping-work-entry-type-dialog',
  templateUrl: './time-keeping-work-entry-type-dialog.component.html',
  styleUrls: ['./time-keeping-work-entry-type-dialog.component.css']
})
export class TimeKeepingWorkEntryTypeDialogComponent implements OnInit {
  formGroup: FormGroup;
  id: string;
  workEntryType: WorkEntryType = new WorkEntryType();
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private timeKeepingService: TimeKeepingService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: '',
    })
    if (this.id)
      this.loadData();
  }

  loadData() {
    this.timeKeepingService.getWorkEntryType(this.id).subscribe(
      result => {
        this.workEntryType = result;
        this.formGroup.patchValue(this.workEntryType);
      }
    )
  }

  onSave() {
    if (this.formGroup.invalid)
      return;
    var val = new WorkEntryType();
    val.name = this.formGroup.get('name').value;
    val.color = "red";
    if (this.id) {
      this.timeKeepingService.updateWorkEntryType(this.id, val).subscribe(
        () => {
          this.activeModal.close(true);
        }
      )
    } else {
      this.timeKeepingService.createWorkEntryType(val).subscribe(
        () => {
          this.activeModal.close(true);
        }
      )
    }
  }

}
