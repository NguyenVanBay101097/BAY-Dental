import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceReportTimeComponent } from './service-report-time.component';

describe('ServiceReportTimeComponent', () => {
  let component: ServiceReportTimeComponent;
  let fixture: ComponentFixture<ServiceReportTimeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceReportTimeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceReportTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
