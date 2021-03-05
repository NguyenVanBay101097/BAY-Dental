import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyManageListComponent } from './survey-manage-list.component';

describe('SurveyManageListComponent', () => {
  let component: SurveyManageListComponent;
  let fixture: ComponentFixture<SurveyManageListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyManageListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyManageListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
