import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';

@Component({
  selector: 'app-treatment-history-teeth-popover',
  templateUrl: './treatment-history-teeth-popover.component.html',
  styleUrls: ['./treatment-history-teeth-popover.component.css']
})
export class TreatmentHistoryTeethPopoverComponent implements OnInit {
  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  filteredToothCategories: ToothCategoryBasic[] = [];
  @Input() teeths = [];
  @Output() teethselected = new EventEmitter<ToothDisplay[]>();

  @ViewChild('popOver', { static: true }) public popover: any;
  
  constructor(
    private fb: FormBuilder,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
  ) { }

  ngOnInit() {

    setTimeout(() => {
      this.loadDefaultToothCategory().subscribe(result => {
        this.loadTeethMap(result);
      })
    });
  }

  toggleWithTags(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      this.loadDefaultToothCategory().subscribe(result => {
        this.loadTeethMap(result);
      })
      popover.open();
    }
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
  }

  getSelectedIndex(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return i;
      }
    }

    return null;
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
    }
  }



  processTeeth(teeth: ToothDisplay[]) {
    this.hamList = {
      '0_up': { '0_right': [], '1_left': [] },
      '1_down': { '0_right': [], '1_left': [] }
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === '1_left') {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  onSelected(tooth: ToothDisplay) {

    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
    }   
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
  }

  onSave(){
    debugger
    this.popover.close();
    this.teethselected.emit(this.teethSelected);
  }

}
