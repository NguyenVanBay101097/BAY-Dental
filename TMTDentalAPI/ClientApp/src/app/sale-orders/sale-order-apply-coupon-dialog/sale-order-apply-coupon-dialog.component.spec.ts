import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderApplyCouponDialogComponent } from './sale-order-apply-coupon-dialog.component';

describe('SaleOrderApplyCouponDialogComponent', () => {
  let component: SaleOrderApplyCouponDialogComponent;
  let fixture: ComponentFixture<SaleOrderApplyCouponDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderApplyCouponDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderApplyCouponDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
