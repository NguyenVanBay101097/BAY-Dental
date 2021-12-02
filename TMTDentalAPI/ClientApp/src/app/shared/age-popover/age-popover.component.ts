import { Component, OnInit, Output, EventEmitter, ViewChild, Input } from '@angular/core';

@Component({
  selector: 'app-age-popover',
  templateUrl: './age-popover.component.html',
  styleUrls: ['./age-popover.component.css']
})
export class AgePopoverComponent implements OnInit {
  @Output() onAgeEmit: EventEmitter<number> = new EventEmitter();
  @ViewChild('popOver', { static: true }) public popover: any;
  age: number = 0;
  
  constructor() { }

  ngOnInit(): void {
  }

  onSave(): void {
    this.onAgeEmit.emit(this.age);
    this.age = 0;
    this.popover.close();
  }

  onCancle(): void {
    this.age = 0;
    this.popover.close();
  }
}
