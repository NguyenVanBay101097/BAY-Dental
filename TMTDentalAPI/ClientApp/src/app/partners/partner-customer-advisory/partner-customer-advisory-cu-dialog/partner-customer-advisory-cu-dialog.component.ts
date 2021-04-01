import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic } from 'src/app/tooth-categories/tooth-category.service';

@Component({
  selector: 'app-partner-customer-advisory-cu-dialog',
  templateUrl: './partner-customer-advisory-cu-dialog.component.html',
  styleUrls: ['./partner-customer-advisory-cu-dialog.component.css']
})
export class PartnerCustomerAdvisoryCuDialogComponent implements OnInit {

  myForm: FormGroup;
  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private toothService: ToothService
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      advisoryDate: [null, Validators.required]
    })
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
  }

  processTeeth(teeth: ToothDisplay[]) {
    console.log(teeth);
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

  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
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
}
