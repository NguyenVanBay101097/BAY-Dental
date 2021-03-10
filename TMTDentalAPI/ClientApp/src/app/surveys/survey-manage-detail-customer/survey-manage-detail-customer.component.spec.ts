import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SurveyManageDetailCustomerComponent } from './survey-manage-detail-customer.component';

describe('SurveyManageDetailCustomerComponent', () => {
  let component: SurveyManageDetailCustomerComponent;
  let fixture: ComponentFixture<SurveyManageDetailCustomerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SurveyManageDetailCustomerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SurveyManageDetailCustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
