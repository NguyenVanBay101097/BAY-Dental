import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryPaymentListV2Component } from './salary-payment-list-v2.component';

describe('SalaryPaymentListV2Component', () => {
  let component: SalaryPaymentListV2Component;
  let fixture: ComponentFixture<SalaryPaymentListV2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalaryPaymentListV2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryPaymentListV2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
