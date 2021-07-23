import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReceiptReportTimeserviceComponent } from './customer-receipt-report-timeservice.component';

describe('CustomerReceiptReportTimeserviceComponent', () => {
  let component: CustomerReceiptReportTimeserviceComponent;
  let fixture: ComponentFixture<CustomerReceiptReportTimeserviceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReceiptReportTimeserviceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReceiptReportTimeserviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
