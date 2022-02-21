import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportAgeGenderComponent } from './partner-report-age-gender.component';

describe('PartnerReportAgeGenderComponent', () => {
  let component: PartnerReportAgeGenderComponent;
  let fixture: ComponentFixture<PartnerReportAgeGenderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportAgeGenderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportAgeGenderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
