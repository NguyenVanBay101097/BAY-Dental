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
  oldColor: string;
  workEntryType: WorkEntryType = new WorkEntryType();
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private timeKeepingService: TimeKeepingService
  ) { }

  public itemColors: string[] = ['#DB003B', '#DB022B', '#EB4F0D', '#FA8F0E', '#F45375', '#D23C88', '#BA0A80', '#911C7E', '#FEC121', '#CDCF00', '#06A2A8', '#52BDD3']

  ngOnInit() {
    this.formGroup = this.fb.group({
      name: '',
    })
    if (this.id)
      this.loadData();

    console.log(this.itemColors);

  }

  loadData() {
    this.timeKeepingService.getWorkEntryType(this.id).subscribe(
      result => {
        this.workEntryType = result;
        this.formGroup.patchValue(this.workEntryType);
        this.oldColor = this.workEntryType.color;
      }
    )
  }

  chooseColor(color) {
    this.oldColor = color;
  }

  onSave() {
    if (this.formGroup.invalid)
      return;
    var val = new WorkEntryType();
    val.name = this.formGroup.get('name').value;
    val.color = this.oldColor;
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
