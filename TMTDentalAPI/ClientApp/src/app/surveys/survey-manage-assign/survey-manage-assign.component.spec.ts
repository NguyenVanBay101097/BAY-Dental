import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyManageAssignComponent } from './survey-manage-assign.component';

describe('SurveyManageAssignComponent', () => {
  let component: SurveyManageAssignComponent;
  let fixture: ComponentFixture<SurveyManageAssignComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyManageAssignComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyManageAssignComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
