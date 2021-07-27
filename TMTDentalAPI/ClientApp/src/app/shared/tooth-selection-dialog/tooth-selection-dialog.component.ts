import { Component, Input, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToothDisplay } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic } from 'src/app/tooth-categories/tooth-category.service';

@Component({
  selector: 'app-tooth-selection-dialog',
  templateUrl: './tooth-selection-dialog.component.html',
  styleUrls: ['./tooth-selection-dialog.component.css']
})
export class ToothSelectionDialogComponent implements OnInit {

  myForm: FormGroup;
  toothTypeDict = [
    { name: "Hàm trên", value: "upper_jaw" },
    { name: "Nguyên hàm", value: "whole_jaw" },
    { name: "Hàm dưới", value: "lower_jaw" },
    { name: "Chọn răng", value: "manual" },
  ];

  hamList: { [key: string]: {} };

  teethSelected: ToothDisplay[] = [];
  filteredToothCategories: any[] = [];
  listTeeths: any[] = [];
  filterTeeths: any[] = [];
  cateId: string;
  submitted: boolean = false;
  @Input() toothDataInfo: any;
  toothSource: any[] = [];
  toothRemove: any[] = [];
  get f() { return this.myForm.controls; }

  toothSelectedIds: string[] = [];

  getFormValue(key: string) {
    return this.myForm.get(key).value;
  }

  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      toothCategoryId: [this.toothDataInfo.toothCategory ? this.toothDataInfo.toothCategory.id : null],
      toothType: [this.toothDataInfo.toothType || "manual"],
    });

    this.toothSelectedIds = this.toothDataInfo.teeth.map(x => x.id);
    this.loadTeethMap(this.toothDataInfo.toothCategory);
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var teeth = this.listTeeths.filter(el => el.categoryId == categ.id);
    this.filterTeeths = [].concat(teeth);
  }

  onChangeToothCategory(value: any) {
    this.loadTeethMap(value);
    this.toothSelectedIds = [];
  }

  onChangeToothType(toothType) {
    if (toothType != "manual") {
      this.toothSelectedIds = [];
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
    if (this.getFormValue("toothType") == "manual") {
      if (this.isSelected(tooth)) {
        var index = this.getSelectedIndex(tooth);
        this.teethSelected.splice(index, 1);
      } else {
        this.teethSelected.push(tooth);
      }
      this.f.teeth.setValue(this.teethSelected);
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

  onSave() {
    this.submitted = true;
    if (this.myForm.invalid || (this.getFormValue('toothType') == 'manual'
      && this.toothSelectedIds.length == 0)) {
      return false;
    }

    var val = this.myForm.value;
    var result = {
      toothType: val.toothType,
      toothCategory: this.filteredToothCategories.find(x => x.id == val.toothCategoryId),
      teeth: this.listTeeths.filter(x => this.toothSelectedIds.indexOf(x.id) !== -1)
    };

    this.activeModal.close(result);
  }

  onCancel() {
    this.toothDataInfo.teeth = this.toothSource;
    this.activeModal.close(this.toothDataInfo);
  }
}
