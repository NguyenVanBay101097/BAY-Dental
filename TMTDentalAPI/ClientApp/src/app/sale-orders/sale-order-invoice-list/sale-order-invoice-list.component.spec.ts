import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderInvoiceListComponent } from './sale-order-invoice-list.component';

describe('SaleOrderInvoiceListComponent', () => {
  let component: SaleOrderInvoiceListComponent;
  let fixture: ComponentFixture<SaleOrderInvoiceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderInvoiceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderInvoiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
