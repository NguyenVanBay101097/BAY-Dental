import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderApplyCouponComponent } from './sale-order-apply-coupon.component';

describe('SaleOrderApplyCouponComponent', () => {
  let component: SaleOrderApplyCouponComponent;
  let fixture: ComponentFixture<SaleOrderApplyCouponComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderApplyCouponComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderApplyCouponComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
