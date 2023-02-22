import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleCouponProgramGenerateCouponsDialogComponent } from './sale-coupon-program-generate-coupons-dialog.component';

describe('SaleCouponProgramGenerateCouponsDialogComponent', () => {
  let component: SaleCouponProgramGenerateCouponsDialogComponent;
  let fixture: ComponentFixture<SaleCouponProgramGenerateCouponsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleCouponProgramGenerateCouponsDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleCouponProgramGenerateCouponsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
