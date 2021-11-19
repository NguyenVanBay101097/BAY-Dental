import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderPaymentAdvancedDialogComponent } from './sale-order-payment-advanced-dialog.component';

describe('SaleOrderPaymentAdvancedDialogComponent', () => {
  let component: SaleOrderPaymentAdvancedDialogComponent;
  let fixture: ComponentFixture<SaleOrderPaymentAdvancedDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderPaymentAdvancedDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderPaymentAdvancedDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
