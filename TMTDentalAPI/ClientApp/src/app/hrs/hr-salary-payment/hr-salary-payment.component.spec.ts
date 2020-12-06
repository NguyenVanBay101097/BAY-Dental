import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrSalaryPaymentComponent } from './hr-salary-payment.component';

describe('HrSalaryPaymentComponent', () => {
  let component: HrSalaryPaymentComponent;
  let fixture: ComponentFixture<HrSalaryPaymentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrSalaryPaymentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrSalaryPaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
