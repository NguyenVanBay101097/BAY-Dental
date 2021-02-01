import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SurveyAssignmentListComponent } from './survey-assignment-list/survey-assignment-list.component';
import { SurveyConfigurationEvaluationComponent } from './survey-configuration-evaluation/survey-configuration-evaluation.component';
import { SurveyManageAssignEmployeeComponent } from './survey-manage-assign-employee/survey-manage-assign-employee.component';
import { SurveyManageAssignComponent } from './survey-manage-assign/survey-manage-assign.component';
import { SurveyManageEmployeeComponent } from './survey-manage-employee/survey-manage-employee.component';

const routes: Routes = [
  { path: 'config', component: SurveyConfigurationEvaluationComponent },
  {
    path: 'manage', component: SurveyManageAssignComponent,
    children: [
      { path: '', redirectTo: 'employee-assign', pathMatch: 'full' },
      { path: 'employee-assign', component: SurveyManageAssignEmployeeComponent },
      { path: 'employees', component: SurveyManageEmployeeComponent },
    ]
  },
  {path: '' , component: SurveyAssignmentListComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SurveysRoutingModule { }
