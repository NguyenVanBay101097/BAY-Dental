import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReceiptReportOverviewComponent } from './customer-receipt-report-overview.component';

describe('CustomerReceiptReportOverviewComponent', () => {
  let component: CustomerReceiptReportOverviewComponent;
  let fixture: ComponentFixture<CustomerReceiptReportOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReceiptReportOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReceiptReportOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
