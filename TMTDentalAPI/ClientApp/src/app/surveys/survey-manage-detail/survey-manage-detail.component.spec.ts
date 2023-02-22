import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyManageDetailComponent } from './survey-manage-detail.component';

describe('SurveyManageDetailComponent', () => {
  let component: SurveyManageDetailComponent;
  let fixture: ComponentFixture<SurveyManageDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyManageDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyManageDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
