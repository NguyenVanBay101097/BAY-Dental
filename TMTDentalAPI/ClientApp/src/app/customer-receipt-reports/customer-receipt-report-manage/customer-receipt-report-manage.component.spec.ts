import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReceiptReportManageComponent } from './customer-receipt-report-manage.component';

describe('CustomerReceiptReportManageComponent', () => {
  let component: CustomerReceiptReportManageComponent;
  let fixture: ComponentFixture<CustomerReceiptReportManageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReceiptReportManageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReceiptReportManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
