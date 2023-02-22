import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleCouponListComponent } from './sale-coupon-list.component';

describe('SaleCouponListComponent', () => {
  let component: SaleCouponListComponent;
  let fixture: ComponentFixture<SaleCouponListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleCouponListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleCouponListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
