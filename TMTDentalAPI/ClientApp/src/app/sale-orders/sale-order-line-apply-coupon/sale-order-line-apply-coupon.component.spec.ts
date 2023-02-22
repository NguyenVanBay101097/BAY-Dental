import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineApplyCouponComponent } from './sale-order-line-apply-coupon.component';

describe('SaleOrderLineApplyCouponComponent', () => {
  let component: SaleOrderLineApplyCouponComponent;
  let fixture: ComponentFixture<SaleOrderLineApplyCouponComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineApplyCouponComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineApplyCouponComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
