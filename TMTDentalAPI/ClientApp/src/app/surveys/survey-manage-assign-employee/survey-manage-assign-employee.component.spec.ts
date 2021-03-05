import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyManageAssignEmployeeComponent } from './survey-manage-assign-employee.component';

describe('SurveyManageAssignEmployeeComponent', () => {
  let component: SurveyManageAssignEmployeeComponent;
  let fixture: ComponentFixture<SurveyManageAssignEmployeeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyManageAssignEmployeeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyManageAssignEmployeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
