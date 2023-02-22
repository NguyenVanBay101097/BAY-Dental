import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleCouponProgramFilterActiveComponent } from './sale-coupon-program-filter-active.component';

describe('SaleCouponProgramFilterActiveComponent', () => {
  let component: SaleCouponProgramFilterActiveComponent;
  let fixture: ComponentFixture<SaleCouponProgramFilterActiveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleCouponProgramFilterActiveComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleCouponProgramFilterActiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
