import { Component, OnInit } from '@angular/core';
import { ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { ToothService, ToothDisplay } from '../tooth.service';

@Component({
  selector: 'app-tooth-select-dialog',
  templateUrl: './tooth-select-dialog.component.html',
  styleUrls: ['./tooth-select-dialog.component.css']
})
export class ToothSelectDialogComponent implements OnInit {

  hamList: { [key: string]: {} } = {
    '0_up': { '0_right': [], '1_left': [] },
    '1_down': { '0_right': [], '1_left': [] }
  };
  teethSelected: ToothDisplay[] = [];
  constructor(private toothCategoryService: ToothCategoryService, private toothService: ToothService) { }

  ngOnInit() {
    this.loadTeeth().subscribe(teeth => {
      this.processTeeth(teeth);
    });
  }

  processTeeth(teeth: ToothDisplay[]) {
    console.log('teeth: ', teeth);
    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === '1_left') {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
    console.log(this.hamList);
  }

  loadTeeth() {
    return this.toothService.getAllBasic(null);
  }

  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
    }
  }

  getSelectedIndex(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return i;
      }
    }

    return null;
  }

  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
  }
}
