import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReceiptStatePopoverComponent } from './customer-receipt-state-popover.component';

describe('CustomerReceiptStatePopoverComponent', () => {
  let component: CustomerReceiptStatePopoverComponent;
  let fixture: ComponentFixture<CustomerReceiptStatePopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReceiptStatePopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReceiptStatePopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
