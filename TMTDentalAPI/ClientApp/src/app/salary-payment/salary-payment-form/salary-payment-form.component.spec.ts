import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryPaymentFormComponent } from './salary-payment-form.component';

describe('SalaryPaymentFormComponent', () => {
  let component: SalaryPaymentFormComponent;
  let fixture: ComponentFixture<SalaryPaymentFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalaryPaymentFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryPaymentFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
