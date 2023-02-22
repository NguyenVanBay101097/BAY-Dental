import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyTagDialogComponent } from './survey-tag-dialog.component';

describe('SurveyTagDialogComponent', () => {
  let component: SurveyTagDialogComponent;
  let fixture: ComponentFixture<SurveyTagDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyTagDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyTagDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
