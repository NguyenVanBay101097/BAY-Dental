import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReceiptReportNoTreatmentComponent } from './customer-receipt-report-no-treatment.component';

describe('CustomerReceiptReportNoTreatmentComponent', () => {
  let component: CustomerReceiptReportNoTreatmentComponent;
  let fixture: ComponentFixture<CustomerReceiptReportNoTreatmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReceiptReportNoTreatmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReceiptReportNoTreatmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
