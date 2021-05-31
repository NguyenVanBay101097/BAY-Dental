import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QuotationLineApplyCouponComponent } from './quotation-line-apply-coupon.component';

describe('QuotationLineApplyCouponComponent', () => {
  let component: QuotationLineApplyCouponComponent;
  let fixture: ComponentFixture<QuotationLineApplyCouponComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QuotationLineApplyCouponComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QuotationLineApplyCouponComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
