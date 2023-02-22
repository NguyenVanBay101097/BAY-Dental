import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReceiptReportForTimeComponent } from './customer-receipt-report-for-time.component';

describe('CustomerReceiptReportForTimeComponent', () => {
  let component: CustomerReceiptReportForTimeComponent;
  let fixture: ComponentFixture<CustomerReceiptReportForTimeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReceiptReportForTimeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReceiptReportForTimeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
