import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyUserinputCreateDialogComponent } from './survey-userinput-create-dialog.component';

describe('SurveyUserinputCreateDialogComponent', () => {
  let component: SurveyUserinputCreateDialogComponent;
  let fixture: ComponentFixture<SurveyUserinputCreateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyUserinputCreateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyUserinputCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
