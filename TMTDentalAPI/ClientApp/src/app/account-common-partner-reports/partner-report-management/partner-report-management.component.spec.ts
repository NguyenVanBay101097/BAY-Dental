import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerReportManagementComponent } from './partner-report-management.component';

describe('PartnerReportManagementComponent', () => {
  let component: PartnerReportManagementComponent;
  let fixture: ComponentFixture<PartnerReportManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerReportManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerReportManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
