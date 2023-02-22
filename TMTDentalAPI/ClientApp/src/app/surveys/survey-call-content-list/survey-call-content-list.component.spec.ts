import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyCallContentListComponent } from './survey-call-content-list.component';

describe('SurveyCallContentListComponent', () => {
  let component: SurveyCallContentListComponent;
  let fixture: ComponentFixture<SurveyCallContentListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyCallContentListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyCallContentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
