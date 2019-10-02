import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerInvoiceCreateUpdateComponent } from './customer-invoice-create-update.component';

describe('CustomerInvoiceCreateUpdateComponent', () => {
  let component: CustomerInvoiceCreateUpdateComponent;
  let fixture: ComponentFixture<CustomerInvoiceCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerInvoiceCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerInvoiceCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
