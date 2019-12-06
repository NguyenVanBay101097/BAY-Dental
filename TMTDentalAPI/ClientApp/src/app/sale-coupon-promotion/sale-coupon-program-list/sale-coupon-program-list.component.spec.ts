import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleCouponProgramListComponent } from './sale-coupon-program-list.component';

describe('SaleCouponProgramListComponent', () => {
  let component: SaleCouponProgramListComponent;
  let fixture: ComponentFixture<SaleCouponProgramListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleCouponProgramListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleCouponProgramListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
