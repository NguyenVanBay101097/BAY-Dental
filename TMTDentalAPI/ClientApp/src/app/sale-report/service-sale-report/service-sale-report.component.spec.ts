import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceSaleReportComponent } from './service-sale-report.component';

describe('ServiceSaleReportComponent', () => {
  let component: ServiceSaleReportComponent;
  let fixture: ComponentFixture<ServiceSaleReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceSaleReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceSaleReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
