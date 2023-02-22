import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { EmployeeSimple } from 'src/app/employees/employee';
import { EmployeeService } from 'src/app/employees/employee.service';
import { SurveyAssignmentDefaultGet, SurveyAssignmentDefaultGetPar, SurveyAssignmentService } from '../survey.service';

@Component({
  selector: 'app-survey-manage-assign-employee-create-dialog',
  templateUrl: './survey-manage-assign-employee-create-dialog.component.html',
  styleUrls: ['./survey-manage-assign-employee-create-dialog.component.css']
})
export class SurveyManageAssignEmployeeCreateDialogComponent implements OnInit {
  @ViewChild('empCbx', { static: true }) empCbx: ComboBoxComponent;
  filteredEmployees: EmployeeSimple[];
  surveyAssignments: SurveyAssignmentDefaultGet[];
  formGroup: FormGroup;
  title = "";
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private surveyService: SurveyAssignmentService,
    private employeeService: EmployeeService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      arrs: this.fb.array([])
    })
    this.loadDataFromApi();

    this.loadEmployees();
  }

  loadDataFromApi() {
    var val = new SurveyAssignmentDefaultGetPar();
    val.isRandomAssign = false;
    // this.surveyService.defaultGetList(val).subscribe(
    //   result => {
    //     this.surveyAssignments = result;
    //     this.surveyAssignments.forEach(item => {
    //       item.employee = null;
    //       this.arrs.push(this.fb.group(item));
    //     })
    //   }
    // )
  }

  get arrs() {
    return this.formGroup.get('arrs') as FormArray;
  }

  loadEmployees() {
    return this.searchEmployees().subscribe(result => this.filteredEmployees = result);
  }

  searchEmployees(search?: string) {
    var val = {
      search: search,
      isAllowSurvey: true
    }
    return this.employeeService.getEmployeeSimpleList(val);
  }

  onAutoAssign() {
    this.surveyAssignments = [];
    this.arrs.clear();
    var val = new SurveyAssignmentDefaultGetPar();
    val.isRandomAssign = true;
    // this.surveyService.defaultGetList(val).subscribe(
    //   result => {
    //     this.surveyAssignments = result;
    //     this.surveyAssignments.forEach(item => {
    //       this.arrs.push(this.fb.group(item));
    //     })
    //   }
    // )
  }

  onSave() {
    if (this.formGroup.invalid) { return false }
    var vals = this.formGroup.value ? this.formGroup.value.arrs : null;
    if (vals) {
      vals = vals.filter(x => x.employee != null);
      vals.forEach(item => {
        item.employeeId = item.employee ? item.employee.id : null;
      });
    }
    else { return; }
    if (vals && vals.length > 0) {
      this.surveyService.createListAssign(vals).subscribe(
        () => { }
      )
    }
    this.activeModal.close();
  }

}
