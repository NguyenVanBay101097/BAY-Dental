import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyManageDetailSurveyImformationComponent } from './survey-manage-detail-survey-imformation.component';

describe('SurveyManageDetailSurveyImformationComponent', () => {
  let component: SurveyManageDetailSurveyImformationComponent;
  let fixture: ComponentFixture<SurveyManageDetailSurveyImformationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyManageDetailSurveyImformationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyManageDetailSurveyImformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
