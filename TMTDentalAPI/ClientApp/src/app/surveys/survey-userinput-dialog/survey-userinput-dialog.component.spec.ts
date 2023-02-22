import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyUserinputDialogComponent } from './survey-userinput-dialog.component';

describe('SurveyUserinputDialogComponent', () => {
  let component: SurveyUserinputDialogComponent;
  let fixture: ComponentFixture<SurveyUserinputDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyUserinputDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyUserinputDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
