import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SurveysRoutingModule } from './surveys-routing.module';
import { SurveyConfigurationEvaluationComponent } from './survey-configuration-evaluation/survey-configuration-evaluation.component';
import { MyCustomKendoModule } from '../shared/my-customer-kendo.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SurveyService } from './survey.service';
import { SurveyConfigurationEvaluationDialogComponent } from './survey-configuration-evaluation-dialog/survey-configuration-evaluation-dialog.component';
import { NgbModal, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [SurveyConfigurationEvaluationComponent, SurveyConfigurationEvaluationDialogComponent],
  imports: [
    CommonModule,
    SurveysRoutingModule,
    MyCustomKendoModule,
    FormsModule,
    NgbModalModule,
    ReactiveFormsModule,
  ],
  providers: [SurveyService],
  entryComponents: [
    SurveyConfigurationEvaluationDialogComponent
  ]
})
export class SurveysModule { }
