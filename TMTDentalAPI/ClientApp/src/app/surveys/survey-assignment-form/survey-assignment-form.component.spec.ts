import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyAssignmentFormComponent } from './survey-assignment-form.component';

describe('SurveyAssignmentFormComponent', () => {
  let component: SurveyAssignmentFormComponent;
  let fixture: ComponentFixture<SurveyAssignmentFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyAssignmentFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyAssignmentFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
