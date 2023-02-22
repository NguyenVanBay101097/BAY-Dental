import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineDiscountPopoverComponent } from './sale-order-line-discount-popover.component';

describe('SaleOrderLineDiscountPopoverComponent', () => {
  let component: SaleOrderLineDiscountPopoverComponent;
  let fixture: ComponentFixture<SaleOrderLineDiscountPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineDiscountPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineDiscountPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
