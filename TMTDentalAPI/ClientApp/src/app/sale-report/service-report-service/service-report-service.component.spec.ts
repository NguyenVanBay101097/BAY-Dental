import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceReportServiceComponent } from './service-report-service.component';

describe('ServiceReportServiceComponent', () => {
  let component: ServiceReportServiceComponent;
  let fixture: ComponentFixture<ServiceReportServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceReportServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceReportServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
