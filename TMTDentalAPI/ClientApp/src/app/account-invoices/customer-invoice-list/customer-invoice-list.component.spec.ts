import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerInvoiceListComponent } from './customer-invoice-list.component';

describe('CustomerInvoiceListComponent', () => {
  let component: CustomerInvoiceListComponent;
  let fixture: ComponentFixture<CustomerInvoiceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerInvoiceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerInvoiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
