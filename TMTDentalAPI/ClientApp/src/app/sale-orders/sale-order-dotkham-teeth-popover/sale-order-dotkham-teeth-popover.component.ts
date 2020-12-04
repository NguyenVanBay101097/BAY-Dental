import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-sale-order-dotkham-teeth-popover',
  templateUrl: './sale-order-dotkham-teeth-popover.component.html',
  styleUrls: ['./sale-order-dotkham-teeth-popover.component.css'],
  host: {'class': 'ml-auto'}
})
export class SaleOrderDotkhamTeethPopoverComponent implements OnInit {
  formGroup: FormGroup;
  teethSelected: any[] = [];
  allTeeth: any[];
  @Input() line: any;
  @Output() eventTeeth = new EventEmitter<any>();
  @ViewChild('popOver', { static: true }) public popover: any;

  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
     Note: null
    });
    this.reLoad();
  }

  reLoad() {
    this.allTeeth = this.line.SaleOrderLine.Teeth;
    this.formGroup.patchValue(this.line);
    if (this.line.Teeth) {
      this.teethSelected = Object.assign([], this.line.Teeth);
    }
  }

  togglePopover(popOver, lineFG) {
    if (popOver.isOpen()) {
      popOver.close();
    } else {
      this.formGroup.reset();
      this.reLoad();
      popOver.open();
    }
  }

  isSelected(tooth: any) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].Id === tooth.Id) {
        return true;
      }
    }
    return false;
  }

  onSelected(tooth) {
    if (this.isSelected(tooth)) {
      const index = this.teethSelected.indexOf(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
    }
  }

  onSave() {
    const val = this.formGroup.value;
    this.line.Note = val.Note;
    this.line.ToothIds = this.teethSelected.map(x => x.Id);
    this.line.Teeth = this.teethSelected;
    this.eventTeeth.emit(this.line);
    this.popover.close();
  }
}
