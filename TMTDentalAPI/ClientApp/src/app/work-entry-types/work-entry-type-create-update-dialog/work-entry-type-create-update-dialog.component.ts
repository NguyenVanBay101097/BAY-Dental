import { WorkEntryTypeSave } from './../work-entry-type.service';
import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { WorkEntryType, WorkEntryTypeService } from '../work-entry-type.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-work-entry-type-create-update-dialog',
  templateUrl: './work-entry-type-create-update-dialog.component.html',
  styleUrls: ['./work-entry-type-create-update-dialog.component.css']
})
export class WorkEntryTypeCreateUpdateDialogComponent implements OnInit {
  formGroup: FormGroup;
  @Input() public id: string;
  title: string;
  oldColor: string;
  

  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private workEntryTypeService: WorkEntryTypeService
  ) { }

  public itemColors: string[] = ['#DB003B', '#DB022B', '#EB4F0D', '#FA8F0E', '#F45375', '#D23C88', '#BA0A80', '#911C7E', '#FEC121', '#CDCF00', '#06A2A8', '#52BDD3']

  ngOnInit() {

    this.formGroup = this.fb.group({
      name: '',
      isHasTimeKeeping: false,
      code: null,
      sequence: null
    })

   if (this.id) {
      setTimeout(() => {
       this.loadData();
        });
    }
  }

  loadData() {
    this.workEntryTypeService.get(this.id).subscribe(
      result => {       
        this.formGroup.patchValue(result);
      }
    )
  }

  chooseColor(color) {
    this.oldColor = color;
  }

  onSave() {
    if (this.formGroup.invalid)
      return;
    var val = new WorkEntryTypeSave();
    val = this.formGroup.value;
    val.color = this.oldColor;
    if (this.id) {
      this.workEntryTypeService.update(this.id, val).subscribe(
        () => {
          this.activeModal.close(true);
        }
      )
    } else {
      this.workEntryTypeService.create(val).subscribe(
        () => {
          this.activeModal.close(true);
        }
      )
    }
  }

 


}
