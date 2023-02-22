import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QuotationApplyCouponComponent } from './quotation-apply-coupon.component';

describe('QuotationApplyCouponComponent', () => {
  let component: QuotationApplyCouponComponent;
  let fixture: ComponentFixture<QuotationApplyCouponComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QuotationApplyCouponComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QuotationApplyCouponComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
