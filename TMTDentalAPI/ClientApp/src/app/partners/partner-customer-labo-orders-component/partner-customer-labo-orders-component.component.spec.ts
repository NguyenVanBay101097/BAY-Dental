import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerLaboOrdersComponentComponent } from './partner-customer-labo-orders-component.component';

describe('PartnerCustomerLaboOrdersComponentComponent', () => {
  let component: PartnerCustomerLaboOrdersComponentComponent;
  let fixture: ComponentFixture<PartnerCustomerLaboOrdersComponentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerLaboOrdersComponentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerLaboOrdersComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
