import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-compute-price-input-popover',
  templateUrl: './compute-price-input-popover.component.html',
  styleUrls: ['./compute-price-input-popover.component.css']
})
export class ComputePriceInputPopoverComponent implements OnInit {
  @Output() onApply = new EventEmitter();
  @Input() priceObj: any = {
    computePrice: 'percentage',
    percentPrice: 0,
    fixedAmountPrice: 0
  };
  @ViewChild('popOver', { static: false }) public popover: NgbPopover;

  formGroup: FormGroup;
  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.formGroup = this.fb.group({
      computePrice: 'percentage',
      percentPrice: 0,
      fixedAmountPrice: 0
    });
    console.log(this.priceObj);
    
  }

  toggleWithGreeting(popover) {   
    if (popover.isOpen()) {       
      popover.close();
    } else {       
      popover.open();   
    }
  }

  apply() {
    var formValue = this.formGroup.value;
    this.onApply.emit(formValue);
    this.popover.close();
  }

}
