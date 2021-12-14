import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ToothDisplay } from 'src/app/teeth/tooth.service';

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
  teethSort: ToothDisplay[] = [];
  anchor: number = 0;
  focus: number = 0;
  selection = new Set();
  @Input() isDisabled = false;
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

    //process teethSort
    this.teethSort = [];
    for (const pro in this.hamList) {
      this.teethSort.push(...this.hamList[pro]['0_right']);
      this.teethSort.push(...this.hamList[pro]['1_left']);
    }

  }

  isSelected(tooth: ToothDisplay) {
    return this.selectedKeys.indexOf(tooth.id) !== -1;
  }

  clickTooth(tooth: ToothDisplay, event: PointerEvent) {
    var onlyClick = () => {
      var index = this.selectedKeys.indexOf(tooth.id);
      if (this.isSelected(tooth)) {
        this.selectedKeys.splice(index, 1);
        this.anchor = 0;
        this.focus = 0;
      } else {
        var toothIndex = this.teethSort.findIndex(x => x.id == tooth.id);
        this.anchor = toothIndex;
        this.focus = toothIndex;
        this.selectedKeys.push(tooth.id);
      }
    };

    var shiftClick = () => {
      var toothIndex = this.teethSort.findIndex(x => x.id == tooth.id);
      let start = Math.min(this.anchor, this.focus)
      let end = Math.max(this.anchor, this.focus)
      let removeCount = end - start + 1;
      // remove between anchor and focus
      if (toothIndex >= start && toothIndex <= end)
        // this.selectedKeys.splice(0,removeCount);
        this.selectedKeys.splice(this.selectedKeys.length - removeCount, removeCount);

      this.focus = toothIndex;
      start = Math.min(this.anchor, this.focus)
      end = Math.max(this.anchor, this.focus)
      for (let i = start; i <= end; i++) {
        if (!this.isSelected(this.teethSort[i]))
          this.selectedKeys.push(this.teethSort[i].id)
      }
    }


    if (event.shiftKey) {
      shiftClick();
    } else {
      onlyClick();
    }
    // window.getSelection().removeAllRanges();
    this.selectedKeysChange.emit(this.selectedKeys);
  }
}
