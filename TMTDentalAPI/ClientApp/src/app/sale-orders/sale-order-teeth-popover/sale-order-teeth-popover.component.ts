import { Component, Input, OnInit, Output, ViewChild, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TooltipTemplateService } from '@progress/kendo-angular-charts';
import { MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { Observable, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { LaboOrderLineDefaultGet } from 'src/app/labo-order-lines/labo-order-line.service';
import { PartnerCategoryDisplay, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { PartnerAddRemoveTags, PartnerService } from 'src/app/partners/partner.service';
import { PartnerCategoriesService } from 'src/app/shared/services/partner-categories.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';

@Component({
  selector: 'app-sale-order-teeth-popover',
  templateUrl: './sale-order-teeth-popover.component.html',
  styleUrls: ['./sale-order-teeth-popover.component.css']
})
export class SaleOrderTeethPopoverComponent implements OnInit {
  formGroup: FormGroup
  title:'Thông tin bổ sung';
  hamList: { [key: string]: {} };
  teethSelected: ToothDisplay[] = [];
  listTeeths: ToothDisplay[] = [];
  filteredToothCategories: ToothCategoryBasic[] = [];
  toolCateg: ToothCategoryBasic = new ToothCategoryBasic();
  @Input() line: any;
  @Output() eventTeeth = new EventEmitter<any>();
  @ViewChild('popOver', { static: true }) public popover: any;

  constructor(
    private partnerCategoriesService: PartnerCategoriesService,
    private partnerCategoryService: PartnerCategoryService,
    private partnerService: PartnerService,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      note: null,
      diagnostic: null,
      toothCategory: null,
    })
    this.loadToothCategories();

    if (this.line) {
      setTimeout(() => {
        debugger;
        this.formGroup.patchValue(this.line);
        this.teethSelected = [...this.line.teeth];
        if (this.line.get('toothCategory').value) {
          this.loadTeethMap(this.line.get('toothCategory').value)
          this.formGroup.get('toothCategory').patchValue(this.line.get('toothCategory').value)
        }
        if (this.line.get('teeth')) {
          this.teethSelected = [...this.line.get('teeth').value];
        }
      });
    } else {
      setTimeout(() => {
        this.formGroup = this.fb.group({
          note: null,
          diagnostic: null,
          toothCategory: null,
        })
           
      });
    }
 
  }

  loadToothCategories() {
    return this.toothCategoryService.getAll().subscribe(
      result => {
        this.filteredToothCategories = result;
        if (this.line.get('toothCategory').value == null) {
          this.formGroup.get('toothCategory').patchValue(this.filteredToothCategories[1])
          this.onChangeToothCategory(this.filteredToothCategories[1]);
          this.line.get('toothCategoryId').patchValue(this.filteredToothCategories[1].id);
          this.line.get('toothCategory').patchValue(this.filteredToothCategories[1]);
        }
      }
    );
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

  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
      this.line.get('teeth').value.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
      this.line.get('teeth').push(this.fb.group(tooth));
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

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
      this.line.get('toothCategoryId').patchValue(value.id);
      this.line.get('toothCategory').patchValue(value);
    }
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(
      result => this.processTeeth(result)
    );
  }

  toggleWithTeeth(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      popover.open();
    }
  }

  onSave() {
    this.eventTeeth.emit(this.line);
  }

  public service$ = (text: string): any => {
    const val = new PartnerCategoryDisplay();
    val.name = text;
    return this.partnerCategoryService.create(val)
      .pipe(
        map((result: any) => {
          return {
            Id: result.id,
            Name: result.name
          }
        })
      );
  }

}
