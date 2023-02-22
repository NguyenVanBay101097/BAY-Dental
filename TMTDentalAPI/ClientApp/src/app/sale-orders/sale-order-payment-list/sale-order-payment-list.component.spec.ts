import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderPaymentListComponent } from './sale-order-payment-list.component';

describe('SaleOrderPaymentListComponent', () => {
  let component: SaleOrderPaymentListComponent;
  let fixture: ComponentFixture<SaleOrderPaymentListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderPaymentListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderPaymentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
