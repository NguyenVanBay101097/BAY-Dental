import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SurveyAssignmentFormComponent } from './survey-assignment-form/survey-assignment-form.component';
import { SurveyAssignmentListComponent } from './survey-assignment-list/survey-assignment-list.component';
import { SurveyConfigurationEvaluationComponent } from './survey-configuration-evaluation/survey-configuration-evaluation.component';
import { SurveyManageAssignEmployeeComponent } from './survey-manage-assign-employee/survey-manage-assign-employee.component';
import { SurveyManageAssignComponent } from './survey-manage-assign/survey-manage-assign.component';
import { SurveyManageDetailCustomerComponent } from './survey-manage-detail-customer/survey-manage-detail-customer.component';
import { SurveyManageDetailSurveyImformationComponent } from './survey-manage-detail-survey-imformation/survey-manage-detail-survey-imformation.component';
import { SurveyManageDetailComponent } from './survey-manage-detail/survey-manage-detail.component';
import { SurveyManageEmployeeComponent } from './survey-manage-employee/survey-manage-employee.component';
import { SurveyManageListComponent } from './survey-manage-list/survey-manage-list.component';
import { SurveyTagListComponent } from './survey-tag-list/survey-tag-list.component';

const routes: Routes = [
  { path: 'config', component: SurveyConfigurationEvaluationComponent },
  {
    path: 'manage', component: SurveyManageAssignComponent,
    children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { path: 'list', component: SurveyManageListComponent },
      { path: 'employee-assign', component: SurveyManageAssignEmployeeComponent },
      { path: 'employees', component: SurveyManageEmployeeComponent },
    ]
  },
  {
    path: 'form-manage/:id', component: SurveyManageDetailComponent,
    children: [
      { path: '', redirectTo: 'infor-care', pathMatch: 'full' },
      { path: 'infor-survey-evaluation', component: SurveyManageDetailSurveyImformationComponent },
      { path: 'infor-care', component: SurveyManageDetailCustomerComponent }
    ]
  },
  {
    path: 'survey-tag', component: SurveyTagListComponent
  },
  { path: 'list', component: SurveyAssignmentListComponent },
  {
    path: 'form',
    component: SurveyAssignmentFormComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SurveysRoutingModule { }
