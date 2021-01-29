import { Component, OnInit, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { NotificationService } from '@progress/kendo-angular-notification';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { EmployeeSimple } from 'src/app/employees/employee';
import { PartnerFilter, PartnerService } from 'src/app/partners/partner.service';
import { SurveyAssignmentDefaultGet, SurveyService } from '../survey.service';

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
  constructor(
    private fb: FormBuilder,
    private activeModal: NgbActiveModal,
    private notificationService: NotificationService,
    private surveyService: SurveyService,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
    this.formGroup = this.fb.group({
      arrs: this.fb.array([])
    })
    this.loadDataFromApi();

    this.loadEmployees();
  }

  loadDataFromApi() {
    this.surveyService.defaultGetList().subscribe(
      result => {
        this.surveyAssignments = result;
        this.surveyAssignments.forEach(item => {
          item.employee = null;
          this.arrs.push(this.fb.group(item));
        })
      }
    )
  }

  get arrs() {
    return this.formGroup.get('arrs') as FormArray;
  }

  loadEmployees() {
    return this.searchEmployees().subscribe(result => this.filteredEmployees = result);
  }

  searchEmployees(search?: string) {
    var val = new PartnerFilter();
    val.search = search;
    val.employee = true;
    return this.partnerService.autocomplete2(val);
  }

  onSave() {
    if (this.formGroup.invalid) { return false }
    var vals = this.formGroup.value ? this.formGroup.value.arrs : null;
    if (vals) {
      vals.forEach(item => {
        item.employeeId = item.employee ? item.employee.id : null;
      });
    }
    else { return; }
    this.surveyService.createListAssign(vals).subscribe(
      result => {
        this.activeModal.close();
      }
    )
  }

}
