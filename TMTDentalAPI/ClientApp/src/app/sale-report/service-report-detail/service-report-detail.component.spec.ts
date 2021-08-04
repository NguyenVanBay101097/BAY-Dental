import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceReportDetailComponent } from './service-report-detail.component';

describe('ServiceReportDetailComponent', () => {
  let component: ServiceReportDetailComponent;
  let fixture: ComponentFixture<ServiceReportDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceReportDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
