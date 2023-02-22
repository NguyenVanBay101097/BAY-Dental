import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyTagListComponent } from './survey-tag-list.component';

describe('SurveyTagListComponent', () => {
  let component: SurveyTagListComponent;
  let fixture: ComponentFixture<SurveyTagListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyTagListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyTagListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
