import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SurveysRoutingModule } from './surveys-routing.module';
import { SurveyConfigurationEvaluationComponent } from './survey-configuration-evaluation/survey-configuration-evaluation.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SurveyService } from './survey.service';
import { SurveyConfigurationEvaluationDialogComponent } from './survey-configuration-evaluation-dialog/survey-configuration-evaluation-dialog.component';
import { NgbModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { SurveyManageAssignComponent } from './survey-manage-assign/survey-manage-assign.component';
import { SurveyManageAssignEmployeeComponent } from './survey-manage-assign-employee/survey-manage-assign-employee.component';
import { SurveyManageEmployeeComponent } from './survey-manage-employee/survey-manage-employee.component';
import { SharedModule } from '../shared/shared.module';
import { SurveyManageAssignEmployeeCreateDialogComponent } from './survey-manage-assign-employee-create-dialog/survey-manage-assign-employee-create-dialog.component';
import { SurveyAssignmentListComponent } from './survey-assignment-list/survey-assignment-list.component';
import { SurveyAssignmentFormComponent } from './survey-assignment-form/survey-assignment-form.component';
import { SurveyCallContentListComponent } from './survey-call-content-list/survey-call-content-list.component';
import { SurveyUserinputDialogComponent } from './survey-userinput-dialog/survey-userinput-dialog.component';
import { SurveyManageDetailComponent } from './survey-manage-detail/survey-manage-detail.component';
import { SurveyManageDetailCustomerComponent } from './survey-manage-detail-customer/survey-manage-detail-customer.component';
import { SurveyManageDetailSurveyImformationComponent } from './survey-manage-detail-survey-imformation/survey-manage-detail-survey-imformation.component';
import { SurveyTagListComponent } from './survey-tag-list/survey-tag-list.component';
import { SurveyTagDialogComponent } from './survey-tag-dialog/survey-tag-dialog.component';
import { SurveyManageListComponent } from './survey-manage-list/survey-manage-list.component';


@NgModule({
  declarations: [
    SurveyConfigurationEvaluationComponent,
    SurveyConfigurationEvaluationDialogComponent,
    SurveyManageAssignComponent,
    SurveyManageAssignEmployeeComponent,
    SurveyManageEmployeeComponent,
    SurveyManageAssignEmployeeCreateDialogComponent,
    SurveyAssignmentListComponent,
    SurveyAssignmentFormComponent,
    SurveyCallContentListComponent,
    SurveyUserinputDialogComponent,
    SurveyManageDetailComponent,
    SurveyManageDetailCustomerComponent,
    SurveyManageDetailSurveyImformationComponent,
    SurveyTagListComponent,
    SurveyTagDialogComponent,
    SurveyManageListComponent,
  ],
  imports: [
    CommonModule,
    SurveysRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    NgbModalModule,
    SharedModule,
    ReactiveFormsModule,
    DragDropModule
  ],
  providers: [SurveyService],
  entryComponents: [
    SurveyConfigurationEvaluationDialogComponent,
    SurveyManageAssignEmployeeCreateDialogComponent,
    SurveyUserinputDialogComponent,
    SurveyTagDialogComponent
  ]
})
export class SurveysModule { }
