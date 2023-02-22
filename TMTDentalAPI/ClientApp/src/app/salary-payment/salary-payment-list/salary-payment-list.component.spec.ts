import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryPaymentListComponent } from './salary-payment-list.component';

describe('SalaryPaymentListComponent', () => {
  let component: SalaryPaymentListComponent;
  let fixture: ComponentFixture<SalaryPaymentListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalaryPaymentListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryPaymentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
