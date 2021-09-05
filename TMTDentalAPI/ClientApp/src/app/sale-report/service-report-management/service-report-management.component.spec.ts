import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceReportManagementComponent } from './service-report-management.component';

describe('ServiceReportManagementComponent', () => {
  let component: ServiceReportManagementComponent;
  let fixture: ComponentFixture<ServiceReportManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceReportManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceReportManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
