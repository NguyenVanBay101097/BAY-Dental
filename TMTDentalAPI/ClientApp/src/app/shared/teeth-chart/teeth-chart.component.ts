import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ToothBasic, ToothDisplay } from 'src/app/teeth/tooth.service';

@Component({
  selector: 'app-teeth-chart',
  templateUrl: './teeth-chart.component.html',
  styleUrls: ['./teeth-chart.component.css']
})
export class TeethChartComponent implements OnInit, OnChanges {
  @Input() teeth: ToothDisplay[] = [];
  @Input() selectedKeys: string[] = [];
  hamList: { [key: string]: {} };
  @Output() selectedKeysChange = new EventEmitter<string[]>();

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    this.processTeeth();
  }

  ngOnInit() {
    this.processTeeth();
  }

  processTeeth() {
    this.hamList = {
      '0_up': { '0_right': [], '1_left': [] },
      '1_down': { '0_right': [], '1_left': [] }
    };

    for (var i = 0; i < this.teeth.length; i++) {
      var tooth = this.teeth[i];
      if (tooth.position === '1_left') {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  isSelected(tooth: ToothDisplay) {
    return this.selectedKeys.indexOf(tooth.id) !== -1;
  }

  clickTooth(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.selectedKeys.indexOf(tooth.id);
      this.selectedKeys.splice(index, 1);
    } else {
      this.selectedKeys.push(tooth.id);
    }

    this.selectedKeysChange.emit(this.selectedKeys);
  }
}
