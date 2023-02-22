import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderPaymentDialogComponent } from './sale-order-payment-dialog.component';

describe('SaleOrderPaymentDialogComponent', () => {
  let component: SaleOrderPaymentDialogComponent;
  let fixture: ComponentFixture<SaleOrderPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
