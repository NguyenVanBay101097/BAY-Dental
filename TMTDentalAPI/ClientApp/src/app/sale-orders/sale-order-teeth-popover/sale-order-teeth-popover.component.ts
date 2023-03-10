import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { map } from 'rxjs/operators';
import { PartnerCategoryDisplay, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { EmployeesOdataService } from 'src/app/shared/services/employeeOdata.service';
import { ToothDisplay, ToothFilter } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic } from 'src/app/tooth-categories/tooth-category.service';

@Component({
  selector: 'app-sale-order-teeth-popover',
  templateUrl: './sale-order-teeth-popover.component.html',
  styleUrls: ['./sale-order-teeth-popover.component.css']
})
export class SaleOrderTeethPopoverComponent implements OnInit {
  formGroup: FormGroup;
  hamList: { [key: string]: {} };
  teethSelected: any[] = [];
  listTeeths: any[] = [];
  @Input() initialListTeeths: any = [];
  @Input() filteredToothCategories: any[] = [];
  @Input() saleOrderState = 'draft';
  toolCateg: any = new ToothCategoryBasic();
  @Input() line: any;
  @Output() eventTeeth = new EventEmitter<any>();
  @ViewChild('popOver', { static: true }) public popover: any;
  @ViewChild('assinstantCbx') assinstantCbx: ComboBoxComponent;
  filteredAssistants: any[] = [];
  initialListAssistants: any = [];
  @Input() lineValue: any;

  constructor(
    private partnerCategoryService: PartnerCategoryService,
    private employeeOdataService: EmployeesOdataService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      ToothCategory: null,
      ToothCategoryId: null,
      Diagnostic: null,
      Assistant: null,
      AssistantId: null,
    });
    this.reLoad();
    this.loadAssistants();
  }

  get ToothCategoryControl() { return this.formGroup.get('ToothCategory'); }
  get DiagnosticControl() { return this.formGroup.get('Diagnostic'); }
  get lineTeeth() { return this.line ? this.line.Teeth : []; }
  get lineDiagnostic() { return this.line ? this.line.Diagnostic : ''; }

  reLoad() {
    this.loadToothCategories();
    if (this.line.ToothCategory) {
      this.loadTeethMap(this.line.ToothCategory);
      this.formGroup.get('ToothCategory').patchValue(this.line.ToothCategory);
    }
    if (this.line.Diagnostic) {
      this.formGroup.get('Diagnostic').patchValue(this.line.Diagnostic);
    }
        
    if (this.line.Assistant) {
      this.formGroup.get('Assistant').patchValue(this.line.Assistant);
    }
    

    if (this.line.Teeth) {
      this.teethSelected = Object.assign([], this.line.Teeth);
    }
  }

  loadAssistants() {
    const state = {
      filter: {
        logic: 'and',
        filters: [
          { field: 'IsDoctor ', operator: 'eq', value: true },
          { field: 'Active ', operator: 'eq', value: true }
        ]
      }
    };
    const options = {
      select: 'Id,Name'
    };
    this.employeeOdataService.getFetch(state, options).subscribe(
      (result: any) => {
        this.initialListAssistants = result.data;
        this.filteredAssistants = this.initialListAssistants.slice(0, 20);
      }
    );
  }

  onAssintantFilter(value) {
    this.filteredAssistants = this.initialListAssistants.filter((s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1).slice(0, 20);
  }

  showInfo() {
    var list = [];
    if (this.lineTeeth.length) {
      list.push(this.lineTeeth.map(x => x.Name).join(','));
    }

    if (this.line.Diagnostic) {
      list.push(this.line.Diagnostic);
    }

    return list.join('; ');
  }


  loadToothCategories() {
    // return this.toothCategoryService.getAll().subscribe(
    //   result => {
    //     this.filteredToothCategories = result;
    //     if (this.line.toothCategory').value == null) {
    //       this.formGroup.get('toothCategory').patchValue(this.filteredToothCategories[1])
    //       this.onChangeToothCategory(this.filteredToothCategories[1]);
    //       this.line.toothCategoryId').patchValue(this.filteredToothCategories[1].id);
    //       this.line.toothCategory').patchValue(this.filteredToothCategories[1]);
    //     }
    //   }
    // );
    if (this.line.ToothCategory == null && this.filteredToothCategories.length > 0) {
      const cate = this.filteredToothCategories.find(x => x.Sequence === 1);
      this.formGroup.get('ToothCategory').patchValue(cate);
      this.onChangeToothCategory(cate);
      // this.line.ToothCategoryId').patchValue(cate.Id);
      // this.line.ToothCategory').patchValue(cate);
    }
  }

  isSelected(tooth: any) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].Id === tooth.Id) {
        return true;
      }
    }
    return false;
  }

  getSelectedIndex(tooth: any) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].Id === tooth.Id) {
        return i;
      }
    }

    return null;
  }

  onSelected(tooth: ToothDisplay) {
    if (this.saleOrderState !== 'draft') {
      return;
    }
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
    }
  }

  processTeeth(teeth: any[]) {
    this.hamList = {
      '0_up': { '0_right': [], '1_left': [] },
      '1_down': { '0_right': [], '1_left': [] }
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.Position === '1_left') {
        this.hamList[tooth.ViTriHam][tooth.Position].push(tooth);
      } else {
        this.hamList[tooth.ViTriHam][tooth.Position].unshift(tooth);
      }
    }
  }

  onChangeToothCategory(value: any) {
    if (value.Id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
      // this.line.ToothCategoryId').patchValue(value.Id);
      // this.line.ToothCategory').patchValue(value);
      this.formGroup.get('ToothCategory').setValue(value);
      // this.line.controls['Teeth'] = this.fb.array([]);
      // this.line.value.Teeth = [];
    }
  }

  loadTeethMap(categ: any) {
    const val = new ToothFilter();
    val.categoryId = categ.Id;
    const result = this.initialListTeeths.filter(x => x.CategoryId === categ.Id);
    this.processTeeth(result);
  }

  toggleWithTeeth(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      this.formGroup.reset();
      this.reLoad();
      popover.open();
    }
  }

  onSave() {
    this.line = this.formGroup.value;
    this.line.ToothCategoryId = this.line.ToothCategory.Id;
    this.line.Teeth = this.teethSelected;
    this.line.Assistant = this.line.Assistant;
    this.line.AssistantId = this.line.Assistant ? this.line.Assistant.Id : null;
    this.eventTeeth.emit(this.line);
    this.popover.close();
  }

  onDiagnosticChange(val) {
    // this.line.Diagnostic').patchValue(val);
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
