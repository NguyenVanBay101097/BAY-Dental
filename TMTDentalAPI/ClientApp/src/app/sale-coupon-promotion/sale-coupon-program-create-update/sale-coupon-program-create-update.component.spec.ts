import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleCouponProgramCreateUpdateComponent } from './sale-coupon-program-create-update.component';

describe('SaleCouponProgramCreateUpdateComponent', () => {
  let component: SaleCouponProgramCreateUpdateComponent;
  let fixture: ComponentFixture<SaleCouponProgramCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleCouponProgramCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleCouponProgramCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
