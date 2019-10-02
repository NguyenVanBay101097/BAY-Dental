import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerInvoicePrintComponent } from './customer-invoice-print.component';

describe('CustomerInvoicePrintComponent', () => {
  let component: CustomerInvoicePrintComponent;
  let fixture: ComponentFixture<CustomerInvoicePrintComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerInvoicePrintComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerInvoicePrintComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
