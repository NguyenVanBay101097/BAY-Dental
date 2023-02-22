import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyConfigurationEvaluationDialogComponent } from './survey-configuration-evaluation-dialog.component';

describe('SurveyConfigurationEvaluationDialogComponent', () => {
  let component: SurveyConfigurationEvaluationDialogComponent;
  let fixture: ComponentFixture<SurveyConfigurationEvaluationDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyConfigurationEvaluationDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyConfigurationEvaluationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
