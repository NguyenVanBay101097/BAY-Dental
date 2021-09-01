import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReceiptReportForTimeDetailComponent } from './customer-receipt-report-for-time-detail.component';

describe('CustomerReceiptReportForTimeDetailComponent', () => {
  let component: CustomerReceiptReportForTimeDetailComponent;
  let fixture: ComponentFixture<CustomerReceiptReportForTimeDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReceiptReportForTimeDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReceiptReportForTimeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
