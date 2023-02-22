import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplyDiscountSaleOrderLinePopoverComponent } from './apply-discount-sale-order-line-popover.component';

describe('ApplyDiscountSaleOrderLinePopoverComponent', () => {
  let component: ApplyDiscountSaleOrderLinePopoverComponent;
  let fixture: ComponentFixture<ApplyDiscountSaleOrderLinePopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplyDiscountSaleOrderLinePopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplyDiscountSaleOrderLinePopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
