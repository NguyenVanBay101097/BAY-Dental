import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderPaymentComponent } from './sale-order-payment.component';

describe('SaleOrderPaymentComponent', () => {
  let component: SaleOrderPaymentComponent;
  let fixture: ComponentFixture<SaleOrderPaymentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderPaymentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderPaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
