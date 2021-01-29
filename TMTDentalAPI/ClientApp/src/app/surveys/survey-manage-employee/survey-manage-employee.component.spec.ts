import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyManageEmployeeComponent } from './survey-manage-employee.component';

describe('SurveyManageEmployeeComponent', () => {
  let component: SurveyManageEmployeeComponent;
  let fixture: ComponentFixture<SurveyManageEmployeeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyManageEmployeeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyManageEmployeeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
