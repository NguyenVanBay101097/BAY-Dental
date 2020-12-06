import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineDiscountOdataPopoverComponent } from './sale-order-line-discount-odata-popover.component';

describe('SaleOrderLineDiscountOdataPopoverComponent', () => {
  let component: SaleOrderLineDiscountOdataPopoverComponent;
  let fixture: ComponentFixture<SaleOrderLineDiscountOdataPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineDiscountOdataPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineDiscountOdataPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
