import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyConfigurationEvaluationComponent } from './survey-configuration-evaluation.component';

describe('SurveyConfigurationEvaluationComponent', () => {
  let component: SurveyConfigurationEvaluationComponent;
  let fixture: ComponentFixture<SurveyConfigurationEvaluationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyConfigurationEvaluationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyConfigurationEvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
