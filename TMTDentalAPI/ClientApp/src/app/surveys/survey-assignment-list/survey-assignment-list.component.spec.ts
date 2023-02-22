import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyAssignmentListComponent } from './survey-assignment-list.component';

describe('SurveyAssignmentListComponent', () => {
  let component: SurveyAssignmentListComponent;
  let fixture: ComponentFixture<SurveyAssignmentListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyAssignmentListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyAssignmentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
