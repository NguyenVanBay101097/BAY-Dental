import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';

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
  cateId: string;
  submitted: boolean = false;
  @Input() toothDataInfo: any;
  toothRemove: any[] = [];
  get f() { return this.myForm.controls; }

  getFormValue(key: string) {
    return this.myForm.get(key).value;
  }

  constructor(
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      toothCategory: '',
      toothType: "manual",
      teeth: [],
    })
    if (this.toothDataInfo.toothType && this.toothDataInfo.toothCategory) {
      this.myForm.setValue(this.toothDataInfo);
      this.teethSelected = this.toothDataInfo.teeth;
      this.cateId = this.toothDataInfo.toothCategory ? this.toothDataInfo.toothCategory.id : '';
      this.loadTeethMap(this.toothDataInfo.toothCategory);
    }
    else {
      this.loadDefaultToothCategory().subscribe(result => {
        this.cateId = result.id;
        this.f.toothCategory.setValue(result);
        this.loadTeethMap(result);
      })
    }

    setTimeout(() => {
      this.loadToothCategories();
    });
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  loadToothCategories() {
    return this.toothCategoryService.getAll().subscribe(result => this.filteredToothCategories = result);
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
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

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.f.teeth.setValue(this.teethSelected);
      this.loadTeethMap(value);
      this.cateId = value.id;
      this.f.toothCategory.setValue(value);
    }
  }

  onChangeToothType(toothType) {
    if (toothType != "manual") {
      this.teethSelected = [];
      this.f.teeth.setValue(this.teethSelected);
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
    this.toothRemove.push(tooth.name);
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
    var val = this.myForm.value;
    val.toothCategoryId = val.toothCategory ? val.toothCategory.id : '';
    this.activeModal.close(val);
  }

  onCancel() {
    this.toothDataInfo.teeth = this.toothDataInfo.teeth.filter(item => !this.toothRemove.includes(item.name));
    this.activeModal.close(this.toothDataInfo);
  }
}
