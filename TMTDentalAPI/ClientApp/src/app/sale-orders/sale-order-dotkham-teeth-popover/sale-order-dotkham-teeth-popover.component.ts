import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { SaleOrderLineService } from 'src/app/core/services/sale-order-line.service';
import { SaleOrderLinesOdataService } from 'src/app/shared/services/sale-order-linesOdata.service';

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
    private fb: FormBuilder,
    private saleOrderLineService: SaleOrderLineService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
     note: null
    });
  }

  reLoad() {
    this.saleOrderLineService.getTeethList(this.line.saleOrderLineId).subscribe((res: any) => {
      this.allTeeth = res;
    });
    this.formGroup.patchValue(this.line);
    if (this.line.teeth) {
      this.teethSelected = Object.assign([], this.line.teeth);
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
      if (this.teethSelected[i].id === tooth.id) {
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
    this.line.note = val.note;
    this.line.toothIds = this.teethSelected.map(x => x.id);
    this.line.teeth = this.teethSelected;
    this.eventTeeth.emit(this.line);
    this.popover.close();
  }
}
