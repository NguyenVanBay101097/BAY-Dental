import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SurveyConfigurationEvaluationComponent } from './survey-configuration-evaluation/survey-configuration-evaluation.component';

const routes: Routes = [
  { path: 'config', component: SurveyConfigurationEvaluationComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SurveysRoutingModule { }
