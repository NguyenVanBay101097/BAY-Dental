import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleCouponListDialogComponent } from './sale-coupon-list-dialog.component';

describe('SaleCouponListDialogComponent', () => {
  let component: SaleCouponListDialogComponent;
  let fixture: ComponentFixture<SaleCouponListDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleCouponListDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleCouponListDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
