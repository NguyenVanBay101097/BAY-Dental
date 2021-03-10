import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyManageAssignEmployeeCreateDialogComponent } from './survey-manage-assign-employee-create-dialog.component';

describe('SurveyManageAssignEmployeeCreateDialogComponent', () => {
  let component: SurveyManageAssignEmployeeCreateDialogComponent;
  let fixture: ComponentFixture<SurveyManageAssignEmployeeCreateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyManageAssignEmployeeCreateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyManageAssignEmployeeCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
